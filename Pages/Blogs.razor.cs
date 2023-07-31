using Leavetown.Shared.Models.LeavetownCms;
using Leavetown.Client.Utilities.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Leavetown.Shared.Clients.Contracts;

namespace Leavetown.Client.Pages
{
    public partial class Blogs
    {        
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ICmsClient CmsClient { get; set; } = default!;
        [Parameter] public string? Culture { get; set; }
        [Parameter] public string CategoryUrl { get; set; } = null!;
        [Parameter] public string Author { get; set; } = null!;
        [Parameter] public int PageId { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

        [CascadingParameter] public Configuration Configuration { get; set; } = default!;
        private List<BlogPostModel> _blogs = new();
        private List<BlogCategoryModel> _categories = new();
        private BlogPostModel _featured = new();
        private int _pageItemsLimit = 0;

        private int _itemsCount;        

        protected override async Task OnInitializedAsync()
        {
            await UpdateData();            
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => OnLocationChangedHandler();

        private async void OnLocationChangedHandler()
        {
            await UpdateData();            
        }

        private async Task UpdateData()
        {
            _pageItemsLimit = Configuration.WhiteLabelSettings.BlogsPageItemCount;
            if (_pageItemsLimit == 0)
            {
                throw new ArgumentException("BlogsPageItemCount should be a valid number, greater than 0");
            }
            var pageModel = await GetPageModel(async () => new
            {
                blogs = await CmsClient.GetBlogPostsAsync(0 + (PageId * _pageItemsLimit), _pageItemsLimit, CategoryUrl, Author),
                categories = _categories.Any() ? _categories : await CmsClient.GetBlogCategoriesAsync()
            });
            _blogs = pageModel.blogs;
            _categories = pageModel.categories;
            _featured = _blogs.FirstOrDefault() ?? new();
            _itemsCount = await CmsClient.GetBlogPostsCount(CategoryUrl, Author);            
            StateHasChanged();
        }

        private void RedirectWithFilters(int pageId)
        {
            if (NavigationManager.Uri.Contains("page"))
            {
                NavigationManager.NavigateTo(NavigationManager.Uri.Replace($"/page/{PageId}", pageId == 0 ? String.Empty : $"/page/{pageId}"));
            }
            else
            {
                NavigationManager.NavigateTo(string.Concat(NavigationManager.Uri, $"/page/{pageId}"));
            }
        }

        public override void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            base.Dispose();
        }        
    }
}
