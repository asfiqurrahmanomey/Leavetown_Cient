using Leavetown.Shared.Clients.Contracts;
using Leavetown.Shared.Models.LeavetownCms;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages.StaticPages
{
    public partial class AboutUs : BaseStaticGalleryPage
    {
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        [Inject] ICmsClient CmsClient { get; set; } = default!;

        private ContentPageModel _pageData = new ContentPageModel();
        protected override async Task OnInitializedAsync()
        {
            _pageData = await GetPageModel(async () => (await CmsClient.GetContentPageData("about-us")) ?? throw new Exception("No page data found"));
        }
    }
}
