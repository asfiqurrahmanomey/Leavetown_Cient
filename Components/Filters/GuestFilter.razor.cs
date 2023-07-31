using Leavetown.Client.Components.Filters.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Components.Filters
{
    public partial class GuestFilter : IFilterComponent, IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IPetFilterService PetFilterService { get; set; } = default!;
        [Inject] private IGuestFilterService GuestFilterService { get; set; } = default!;

        [Parameter] public string Id { get; set; } = "guests-popover";
        [Parameter] public bool IsCentered { get; set; } = true;

        [Parameter]
        public GuestFilterModel GuestValue
        {
            get => _guestValue;
            set
            {
                _guestValue = value;
                SetCounters();
            }
        }

        [Parameter] public Action<GuestFilterModel> GuestChanged { get; set; } = (args) => { };
        [Parameter] public EventCallback<GuestFilterModel> GuestValueChanged { get; set; } = default!;

        [Parameter]
        public PetFilterModel PetValue
        {
            get => _petValue;
            set
            {
                _petValue = value;
                SetCounters();
            }
        }
        [Parameter] public Action<PetFilterModel> PetChanged { get; set; } = (args) => { };
        [Parameter] public EventCallback<PetFilterModel> PetValueChanged { get; set; } = default!;

        [Parameter] public bool IncludeChildren { get; set; } = true;
        [Parameter] public bool IncludePets { get; set; } = true;
        [Parameter] public bool UpdateUrlOnChange { get; set; } = true;
        [Parameter] public int MinAdults { get; set; } = 0;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
        [Parameter] public int PetMax { get; set; } = _maxValuePerGuestType;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private Popover _guestsPopover = new();
        private GuestFilterModel _guestValue = new();
        private PetFilterModel _petValue = new();

        private Counter _adultCounter = new();
        private Counter _childrenCounter = new();
        private Counter _infantCounter = new();
        private Counter _petCounter = new();

        private Debouncer _debouncer = new();

        private const int _maxValuePerGuestType = 24;
        private bool _disposedValue;

        private string FilterTitle
        {
            get
            {
                var filterRepresentations = new List<string>();
                if (_guestValue.GuestCount == 0)
                    filterRepresentations.Add(ResourcesShared.Guests);
                else
                    filterRepresentations.Add(FilterRepresentationHelper.GetFilterRepresentation(_guestValue, ResourcesShared));

                if (_petValue.PetCount > 0)
                    filterRepresentations.Add(FilterRepresentationHelper.GetFilterRepresentation(_petValue, ResourcesShared));

                return string.Join(", ", filterRepresentations.Where(x => !string.IsNullOrEmpty(x)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            await UpdateFromUrlAsync(NavigationManager.Uri);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender) SetCounters();
        }

        private async Task UpdateFromUrlAsync(string uri)
        {
            if (uri == null || !UpdateUrlOnChange) return;

            Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(new Uri(uri).Query);
            var dict = new Dictionary<string, StringValues>();

            if (queryValues.TryGetValue(nameof(FilterType.Guests).ToLower(), out StringValues guestsValue))
            {
                dict.Add(nameof(FilterType.Guests).ToLower(), guestsValue);
            }
            if (queryValues.TryGetValue(nameof(FilterType.Adults).ToLower(), out StringValues adultsValue))
            {
                dict.Add(nameof(FilterType.Adults).ToLower(), adultsValue);
            }
            if (queryValues.TryGetValue(nameof(FilterType.Children).ToLower(), out StringValues childrenValue))
            {
                dict.Add(nameof(FilterType.Children).ToLower(), childrenValue);
            }
            queryValues.TryGetValue(nameof(FilterType.Pets).ToLower(), out StringValues petsValue);
            
            var guestValue = await GuestFilterService.ParseAsync(dict);
            if (!guestValue.Equals(GuestValue))
            {
                GuestValue = guestValue;
                GuestChanged.Invoke(GuestValue);
                await GuestValueChanged.InvokeAsync(GuestValue);
            }

            var petValue = PetFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Pets).ToLower(), petsValue));
            if (!petValue.Equals(PetValue))
            {
                PetValue = petValue;
                PetChanged.Invoke(PetValue);
                await PetValueChanged.InvokeAsync(PetValue);
            }

            StateHasChanged();
        }

        public void Reset(FilterType? filterType = null)
        {
            switch (filterType)
            {
                case FilterType.Guests:
                    ResetGuestsFilter();
                    break;
                case FilterType.Pets:
                    ResetPetsFilter();
                    break;
                default:
                    break;
            }
        }

        public void Expand() => _guestsPopover.ToggleCardVisibility();

        private void ResetGuestsFilter()
        {
            GuestValue = new GuestFilterModel();
            _adultCounter.Reset();
            _childrenCounter.Reset();
            _infantCounter.Reset();
            StateHasChanged();
            GuestChanged.Invoke(GuestValue);
            OnGuestValueChanged();
        }

        private void ResetPetsFilter()
        {
            PetValue = new PetFilterModel();
            _petCounter.Reset();
            StateHasChanged();
            PetChanged.Invoke(PetValue);
            OnPetValueChanged();
        }

        public void SetCounters()
        {
            _adultCounter.Set(_guestValue.AdultCount);
            _childrenCounter.Set(_guestValue.ChildCount);
            _petCounter.Set(_petValue.PetCount);
            StateHasChanged();
        }

        private Task OnGuestValueChanged()
        {
            GuestValue = new GuestFilterModel
            {
                GuestCount = _adultCounter.Value + _childrenCounter.Value,
                AdultCount = _adultCounter.Value,
                ChildCount = _childrenCounter.Value,
            };
            if (UpdateUrlOnChange) NavigationManager.AddQueryParameter(GuestValue);
            GuestChanged.Invoke(GuestValue);
            return GuestValueChanged.InvokeAsync(GuestValue);
        }

        private Task OnPetValueChanged()
        {
            PetValue = new PetFilterModel { PetCount = _petCounter.Value };
            if (UpdateUrlOnChange) NavigationManager.AddQueryParameter(PetValue);
            PetChanged.Invoke(PetValue);

            return PetValueChanged.InvokeAsync(PetValue);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(async () => await UpdateFromUrlAsync(e.Location));

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NavigationManager.LocationChanged -= OnLocationChanged;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
