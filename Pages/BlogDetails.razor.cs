using Leavetown.Shared.Clients.Contracts;
using Leavetown.Shared.Models.LeavetownCms;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages
{
    public partial class BlogDetails
    {
        [Parameter] public string BlogUrl { get; set; } = default!;
        [Parameter] public string? Culture { get; set; }
        [Inject] ICmsClient CmsClient { get; set; } = default!;

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private BlogPostModel _blogModel = new BlogPostModel();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _blogModel = await GetPageModel(async () => (await CmsClient.GetBlogPostByUrlAsync(BlogUrl)) ?? throw new Exception("Blog url invalid. No blog found"));
            } catch (Exception)
            {
                // redirect to another page which we know doesnt exist, so that the standard 404 kicks in
                NavigationManager.NavigateTo($"/{Culture}/blog-post-not-found?url={BlogUrl}");
            }
        }        
    }
}
