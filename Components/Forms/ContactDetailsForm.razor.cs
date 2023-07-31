using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components.Forms.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Models;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components.Forms
{
    public partial class ContactDetailsForm : IFormComponent<BookingViewModel>
    {
        [Inject] ILeavetownClient LeavetownClient { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public string? Title { get; set; } = "";
        [Parameter] public BookingViewModel Data { get; set; } = new();
        [Parameter] public string ReturnUrl { get; set; } = "";
        [Parameter] public EventCallback OnContactDetailsSubmit { get; set; }
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [Parameter] public string Culture { get; set; } = default!;

        protected List<CountryModel> _ddlCountriesData = new();

        protected override async Task OnInitializedAsync()
        {
            var countries = await LeavetownClient.GetCountryModelsAsync(Culture);

            if (countries != null) _ddlCountriesData.AddRange(countries);
        }

        private void NavigateToReturnUrl()
        {
            NavigationManager.NavigateTo(ReturnUrl);
            Data.BillingAddressDetails = Data.ContactAddressDetails;
        }

        private void OnValidSubmit() => InvokeAsync(async () =>
        {
            await OnContactDetailsSubmit.InvokeAsync();
        });

        private void OnInvalidSubmit() => InvokeAsync(async () =>
        {
            await JSRuntime.InvokeVoidAsync("scrollToFirstError");
        });
    }
}
