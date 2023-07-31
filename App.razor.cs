using Microsoft.AspNetCore.Components;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings;
using System.Text.Json;

namespace Leavetown.Client
{
    public partial class App : IDisposable
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] public Configuration Configuration { get; set; } = default!;
        [Inject] public IDestinationService DestinationService { get; set; } = default!;
        [Inject] public IAmenityService AmenityService { get; set; } = default!;
        [Inject] public IPropertyTypeService PropertyTypeService { get; set; } = default!;
        [Inject] public PersistentComponentState ApplicationState { get; set; } = default!;

        private I18nText.SharedResources _sharedResources = new();
        private I18nText.MicroSiteSpecificResources _microSiteResources = new();
        private PersistingComponentStateSubscription _persistingSubscription;
        private const string KeyMicroSiteResources = "microSite";
        private const string KeySharedResources = "sharedResources";

        protected override async Task OnInitializedAsync()
        {
            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistI18Resources);

            if (ApplicationState.TryTakeFromJson<string>(KeyMicroSiteResources, out var jsonForMicroSite))
            {
                _microSiteResources = JsonSerializer.Deserialize<I18nText.MicroSiteSpecificResources>(jsonForMicroSite!, new JsonSerializerOptions { IncludeFields = true })!;
            }
            else
            {
                _microSiteResources = await I18nTextHelper.GetTextTableAsync<I18nText.MicroSiteSpecificResources>(this);
            }

            if (ApplicationState.TryTakeFromJson<string>(KeySharedResources, out var jsonForShared))
            {
                _sharedResources = JsonSerializer.Deserialize<I18nText.SharedResources>(jsonForShared!, new JsonSerializerOptions { IncludeFields = true })!;
            }
            else
            {
                _sharedResources = await I18nTextHelper.GetTextTableAsync<I18nText.SharedResources>(this);
            }

            await CurrencyService.SetLocalCurrencyAsync();
            await CultureService.SetCultureAsync(new Uri(NavigationManager.Uri));            
            await AmenityService.GetAmenitiesAsync();
            await PropertyTypeService.GetPropertyTypesAsync();
        }

        private Task PersistI18Resources()
        {
            PersistMicroSiteResources();
            PersistSharedResources();
            return Task.CompletedTask;
        }
        private Task PersistMicroSiteResources()
        {
            var json = JsonSerializer.Serialize(_microSiteResources, new JsonSerializerOptions { IncludeFields = true });
            ApplicationState.PersistAsJson(KeyMicroSiteResources, json);
            return Task.CompletedTask;
        }

        private Task PersistSharedResources()
        {
            var json = JsonSerializer.Serialize(_sharedResources, new JsonSerializerOptions { IncludeFields = true });
            ApplicationState.PersistAsJson(KeySharedResources, json);
            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            _persistingSubscription.Dispose();
        }            
    }
}
