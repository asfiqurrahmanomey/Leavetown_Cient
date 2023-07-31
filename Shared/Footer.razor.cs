using Microsoft.AspNetCore.Components;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Clients.Contracts;

namespace Leavetown.Client.Shared
{
    public partial class Footer
    {
        [CascadingParameter] public Configuration Configuration { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

        private string _culture = "";
        //[Inject] ICmsClient CmsClient { get; set; } = default!;

        private string _externalMarkup = default!;

        string footerCopyright => ResourcesShared.Copyright != null ? string.Format(ResourcesShared.Copyright, DateTime.Now.Year) : "";

        protected override async Task OnInitializedAsync()
        {
            _culture = await CultureService.GetCultureAsync();
            await base.OnInitializedAsync();
            //if (Configuration.WhiteLabelSettings.UseExternalMarkupForFooter)
            //{
            //    _externalMarkup = await GetComponentMarkup(async () => (await CmsClient.GetComponentMarkup("page-footer")) ?? throw new Exception("No page data found"));
            //}
        }

        [Inject] private PersistentComponentState ApplicationState { get; set; } = default!;
        private PersistingComponentStateSubscription _persistingSubscription;

        protected async Task<string> GetComponentMarkup(Func<Task<string>> loadComponentMarkup)
        {
            string markup = default!;
            const string key = "component-data";

            Task PersistComponentMarkup()
            {
                ApplicationState.PersistAsJson(key, markup);
                return Task.CompletedTask;
            }

            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistComponentMarkup);

            if (ApplicationState.TryTakeFromJson<string>(key, out var m))
            {
                markup = m!;
            }
            else
            {
                markup = await loadComponentMarkup();
            }

            return markup;
        }
    }
}
