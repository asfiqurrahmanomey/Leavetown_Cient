using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Forms
{
    public partial class NewsletterSubscriptionForm
    {
        //[Inject] private INewsletterService NewsletterService { get; set; } = default!;

        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private ContactDetailsModel _contactDetails = new();

        private bool? _isAddSubscriberSuccessful;

        private void OnValidSubmit() => InvokeAsync(async () =>
        {
            //_isAddSubscriberSuccessful = await NewsletterService.AddSubscriberAsync(_contactDetails);
            StateHasChanged();
        });
    }
}
