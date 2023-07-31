using Leavetown.Shared.Clients.Contracts;
using Leavetown.Shared.Models.LeavetownCms;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages.StaticPages
{
    public partial class OtherProperties
    {
        [Parameter] public string? Culture { get; set; }

        [Inject] ICmsClient CmsClient { get; set; } = default!;

        private ContentPageModel _pageData = new ContentPageModel();
        protected override async Task OnInitializedAsync()
        {            
            _pageData = await GetPageModel(async () => (await CmsClient.GetContentPageData("other-properties")) ?? throw new Exception("No page data found"));
        }        
    }
}
