using Leavetown.Client.Components.Filters;
using Leavetown.Client.Utilities.Settings;
using Microsoft.AspNetCore.Components;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Projections;
using Leavetown.Shared.Email;

namespace Leavetown.Client.Components.Forms
{
    public partial class InquiryForm
    {
        [Parameter] public int? PropertyId { get; set; }
        [Parameter] public AvailabilityFilterModel Availability { get; set; } = new AvailabilityFilterModel();
        [Inject] ILeavetownClient _leavetownClient { get; set; } = null!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public Configuration Configuration { get; set; } = default!;

        private InquiryRequest _inquiryRequest = new();
        private AvailabilityFilter? _inquiryAvailabilityFilter = new();
        private bool _showSuccessMessage = false;
        private bool _isValid = true;
        private bool _isBusy=false;

        private void ClearForm()
        {
            _inquiryRequest = new InquiryRequest();
            _inquiryAvailabilityFilter?.Reset();
            StateHasChanged();
        }

        public async Task OnInquirySubmitAsync_Valid()
        {
            _inquiryRequest.PropertyId = PropertyId;
            _inquiryRequest.RequestDate = DateTime.UtcNow;

            if (_inquiryRequest.Dates.Start == DateTime.MinValue || 
                _inquiryRequest.Dates.End == DateTime.MinValue || 
                Configuration.WhiteLabelSettings.InquiryFormEmailCustomer == null ||
                Configuration.WhiteLabelSettings.WhiteLabelIdentifier == null)
            {
                _isValid = false;
                return;
            }

            _isBusy = true;
            _isValid = true;

            await _leavetownClient.SendContactUsEmailRequestAsync(new CustomerContactUsEmailModel()
            {
                To = Configuration.WhiteLabelSettings.InquiryFormEmailCustomer,
                HostDomain = Configuration.WhiteLabelSettings.WhiteLabelIdentifier,
                TemplateData = _inquiryRequest,
                TemplateName = ResourcesShared.EmailTemplateContactUs
            });
            _showSuccessMessage = true;
            _isBusy = false;
        }
        public void OnInquirySubmitAsync_Invalid()
        {
            if (_inquiryRequest.Dates.Start == DateTime.MinValue || _inquiryRequest.Dates.End == DateTime.MinValue)
            {
                _isValid = false;
                return;
            }
        }
    }

    
}
