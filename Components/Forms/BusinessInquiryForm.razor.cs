using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Email;
using Leavetown.Shared.Models.Projections;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Forms
{
    public partial class BusinessInquiryForm
    {
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public Configuration Configuration { get; set; } = default!;

        [Inject] ILeavetownClient _leavetownClient { get; set; } = null!;

        private BusinessInquiryRequest _inquiryRequest = new();
        private bool _showSuccessMessage = false;
        private bool _isBusy = false;

        // RadzenDropDown requires object types to work properly. This list is built to utilize its TextProperty and ValueProperty Parameters
        private List<object> _businessTypeList = new()
        {
            new { Name = "Property Manager", Value = "Property Manager"},
            new { Name = "Property Owner", Value = "Property Owner"},
            new { Name = "Other", Value = "Other"}
        };

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender) StateHasChanged();
        }

        private void ClearForm()
        {
            _inquiryRequest = new BusinessInquiryRequest();

            StateHasChanged();
        }

        public async Task OnInquirySubmitAsync_Valid()
        {
            _isBusy = true;
            _inquiryRequest.RequestDate = DateTime.UtcNow;
            await _leavetownClient.SendBusinessContactUsEmailRequestAsync(new BusinessContactUsEmailModel()
            {
                To = Configuration.WhiteLabelSettings.InquiryFormEmailBusiness ?? "",
                HostDomain = Configuration.WhiteLabelSettings.WhiteLabelIdentifier ?? "",
                TemplateData = _inquiryRequest,
                TemplateName = ResourcesShared.EmailTemplateBusinessContactUs
            });
            _isBusy = false;
            _showSuccessMessage = true;
        }

    }
}