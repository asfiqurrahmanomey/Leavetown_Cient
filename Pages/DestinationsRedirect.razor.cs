using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Leavetown.Client.Pages
{
    public partial class DestinationsRedirect
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter] public string? Culture { get; set; }
        [Parameter] public string? PrimaryDestinationToken { get; set; }
        [Parameter] public string? SecondaryDestinationToken { get; set; }
        [Parameter] public string? TertiaryDestinationToken { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = await CultureService.GetCultureAsync();
            }

            TextInfo textInfo = new CultureInfo(Culture, false).TextInfo;

            if (TertiaryDestinationToken != null) NavigationManager.NavigateTo($"/{Culture}/accommodations/?location={textInfo.ToTitleCase(TertiaryDestinationToken)}");
            if (SecondaryDestinationToken != null) NavigationManager.NavigateTo($"/{Culture}/accommodations/?location={textInfo.ToTitleCase(SecondaryDestinationToken)}");
            if (PrimaryDestinationToken != null) NavigationManager.NavigateTo($"/accommodations/?location={textInfo.ToTitleCase(PrimaryDestinationToken)}");

            NavigationManager.NavigateTo($"/{Culture}/accommodations");
        }
    }
}
