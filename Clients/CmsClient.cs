using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Clients.Contracts;
using Leavetown.Shared.Models.LeavetownCms;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace Leavetown.Client.Clients
{
    public class CmsClient : ICmsClient
    {
        private readonly HttpClient _httpClient;
         
        public CmsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetBlogPostsCount(string? categoryUrl = null, string? author = null)
        {
            var filters = new Dictionary<string, string>() { };
            if (!string.IsNullOrWhiteSpace(categoryUrl))
                filters.Add("categories.CategoryUrl", $"{categoryUrl}");
            if (!string.IsNullOrWhiteSpace(author))
                filters.Add("Author", $"{author}");

            var url = QueryHelpers.AddQueryString(string.Concat(_httpClient.BaseAddress, "blogposts/count"), filters);
            return await GetAsync<int>(url);
        }

        public async Task<BlogPostModel> GetBlogPostByUrlAsync(string blogUrl)
        {
            var url = string.Concat(_httpClient.BaseAddress, "blogposts/", blogUrl);
  
            var blogPost = await GetAsync<BlogPostModel>(url);
            
            // throw this exception here to handle the fact that we never receive a null response
            if (string.IsNullOrEmpty(blogPost.BlogUrl)) throw new Exception($"No blog with BlogUrl={blogUrl}");

            return blogPost ?? throw new Exception($"No blog with BlogUrl={blogUrl}");
        }

        public async Task<List<BlogPostModel>> GetBlogPostsAsync(int startIndex, int limit, string? categoryUrl = null, string? author = null)
        {
            var filters = new Dictionary<string, string>()
            {
                {"_sort","OriginalPostCreatedAt:DESC" },
                {"_start", $"{startIndex}"},
                {"_limit", $"{limit}"}
            };

            if (!string.IsNullOrWhiteSpace(categoryUrl))
                filters.Add("categories.CategoryUrl", $"{categoryUrl}");
            if(!string.IsNullOrWhiteSpace(author))
                filters.Add("Author", $"{author}");

            var url = QueryHelpers.AddQueryString(string.Concat(_httpClient.BaseAddress, "blogposts"), filters);
            return await GetAsync<List<BlogPostModel>>(url)  ?? new List<BlogPostModel>();
        }

        public async Task<List<BlogCategoryModel>> GetBlogCategoriesAsync()
        {
            return await GetAsync<List<BlogCategoryModel>>(string.Concat(_httpClient.BaseAddress, "blogposts/categories"))
                ?? new List<BlogCategoryModel>();
        }

        public async Task<ContentPageModel> GetContentPageData(string pageContentType)
        {
            var pageContentModel = await GetAsync<ContentPageModel>(string.Concat(_httpClient.BaseAddress, "contentpage/", pageContentType));
            return pageContentModel ?? new ContentPageModel();
        }

        public async Task<string> GetComponentMarkup(string pageContentType)
        {
            return await GetAsync<string>(string.Concat(_httpClient.BaseAddress, "contentpage/", pageContentType)) ?? string.Empty;
        }

        protected async Task<TResult?> GetAsync<TResult>(string uri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TResult>(responseContent, new JsonSerializerOptions());
        }
    }
}
