using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Text;

namespace Leavetown.Client.Services
{
    public class CultureService : ICultureService
    {
        private ILocalStorageService _localStorageService;
        private IJSRuntime _jSRuntime;
        private NavigationManager _navigationManager;
        private Toolbelt.Blazor.I18nText.I18nText _I18nText;

        public CultureService(ILocalStorageService localStorageService, IJSRuntime jSRuntime, NavigationManager navigationManager, Toolbelt.Blazor.I18nText.I18nText I18nText)
        {
            _localStorageService = localStorageService;
            _jSRuntime = jSRuntime;
            _navigationManager = navigationManager;
            _I18nText = I18nText;

            _navigationManager.LocationChanged += OnLocationChanged;
        }

        public async Task<string> GetCultureAsync()
        {
            var storedCulture = await _localStorageService.GetStorageValueAsync<string>("culture");

            if (storedCulture == null)
            {
                var browserCulture = (await _jSRuntime.InvokeAsync<string>("getBrowserCulture")).Split('-')[0];
                return browserCulture;
            }

            return storedCulture;
        }

        public async Task SetCultureAsync(Uri uri)
        {
            string? languageCode = uri.GetLanguageCodeFromUri();

            if (languageCode != null)
            {
                await UpdateCultureAsync(languageCode);
            }
            else
            {
                var culture = await GetCultureAsync();
                var updatedUri = BuildUrlWithCulture(culture, uri);

                _navigationManager.NavigateTo(updatedUri);
            }
        }

        private async Task UpdateCultureAsync(string languageCode)
        {
            await _localStorageService.SetStorageValueAsync("culture", languageCode);
            await _I18nText.SetCurrentLanguageAsync(languageCode);
        }
        
        private string BuildUrlWithCulture(string culture, Uri? uri)
        {
            if (uri == null) return _navigationManager.Uri;

            StringBuilder stringBuilder = new();
            stringBuilder.Append(_navigationManager.BaseUri);
            stringBuilder.Append(culture);
            foreach(var segment in uri.Segments)
            {
                stringBuilder.Append(segment);
            }

            stringBuilder.Append(uri.Query);

            return stringBuilder.ToString();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) =>
            Task.Run((Func<Task?>)(async () => await this.SetCultureAsync(new Uri(e.Location))));
    }
}
