using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Shared
{
    public partial class Header
    {
        [Inject] public ILocalStorageService LocalStorageService { get; set; } = default!;

        [CascadingParameter] public Configuration Configuration { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;


        private string _culture = "";

        private bool _collapseNavMenu = true;

        protected override async Task OnInitializedAsync()
        {
            _culture = await CultureService.GetCultureAsync();
        }

        private void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }
    }
}
