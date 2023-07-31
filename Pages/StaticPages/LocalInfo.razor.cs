using Microsoft.AspNetCore.Components;
using Leavetown.Shared.Models.LeavetownCms;
using Leavetown.Shared.Clients.Contracts;

namespace Leavetown.Client.Pages.StaticPages
{
    public partial class LocalInfo
    {
        [Parameter] public string? Culture { get; set; }
        [Inject] ICmsClient CmsClient { get; set; } = default!;

        private ContentPageModel _pageData = new ContentPageModel();
      
        protected override async Task OnInitializedAsync()
        {            
            _pageData = await GetPageModel(async () => (await CmsClient.GetContentPageData("local-info")) ?? throw new Exception("No page data found"));
        }        
    }
}
