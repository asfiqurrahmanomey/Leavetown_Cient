using Leavetown.Client.Components.Filters;
using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Components
{
    public partial class SortingDropDown : IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISortingService SortingService { get; set; } = default!;

        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private Dictionary<string, SortingCriteria> _sortingDropDownData = new Dictionary<string, SortingCriteria>();
        private SortingCriteria _selectedSortingOption = SortingOptions.Relevance;
        private bool _disposedValue;

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            InitializeData();
            UpdateFromUrl(NavigationManager.Uri);
        }

        private void InitializeData()
        {
            var allSortingOptions = SortingService.GetAllOptions();
            foreach (var option in allSortingOptions)
            {
                var stringRepresentation = FilterRepresentationHelper.GetSortingCriteriaRepresentation(option, ResourcesShared);
                _sortingDropDownData.Add(stringRepresentation, option);
            }
        }

        private void UpdateFromUrl(string uri)
        {
            var query = QueryHelpers.ParseQuery(new Uri(uri).Query);

            if (!query.TryGetValue(nameof(FilterType.SortBy).ToSnakeCase(), out StringValues sortingOptionTag))
            {
                _selectedSortingOption = SortingOptions.Relevance;
                StateHasChanged();
                return;
            }

            if (string.IsNullOrEmpty(sortingOptionTag.ToString())) return;

            _selectedSortingOption = SortingService.GetAllOptions()
                .SingleOrDefault(x => x.Tag != null && x.Tag.Equals(sortingOptionTag.ToString())) ?? SortingOptions.Relevance;
            StateHasChanged();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => UpdateFromUrl(e.Location);

        public void OnSortSelectionChanged()
        {
            _selectedSortingOption ??= SortingOptions.Relevance;
            NavigationManager.AddQueryParameter(_selectedSortingOption);
            StateHasChanged();
        }

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
