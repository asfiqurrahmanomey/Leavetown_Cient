using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Clients;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Services;
using Radzen;
using Leavetown.Client.Utilities.Settings;
using Blazor.GoogleTagManager;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.I18nText;
using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuoteService = Leavetown.Client.Services.QuoteService;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Shared.Clients.Contracts;
using Leavetown.Client.Response;

namespace Leavetown.Client
{
    public class ServiceRegistry
    {
        public static async Task RegisterServices(WebAssemblyHostBuilder builder)
        {            
            var http = new HttpClient()
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };

            using var response = await http.GetAsync("appsettings.whitelabel.json");
            using var stream = await response.Content.ReadAsStreamAsync();

            builder.Configuration.AddJsonStream(stream);

            var configuration = builder.Configuration.Get<Configuration>();

            builder.Services.AddHttpClient<ILeavetownClient, LeavetownClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient<IDestinationClient, DestinationClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient<IListingClient, ListingClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient<ICmsClient, CmsClient>(client => client.BaseAddress = new Uri(string.Concat(builder.HostEnvironment.BaseAddress, "api/")));
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<ICultureService, CultureService>();
            builder.Services.AddScoped<IRenderState, RenderState>();
            builder.Services.AddScoped<IMapBoxService, MapBoxService>();
            //builder.Services.AddScoped<INewsletterService, NewsletterService>();
            builder.Services.AddScoped<IResponse, ResponseStub>();

            RegisterCommonServices(builder.Services, configuration);
        }

        // Services common to client side and server side prerendering
        public static void RegisterCommonServices(IServiceCollection services, Configuration configuration)
        {
            services.AddSingleton(configuration);            

            services.AddHttpClient<IOpenSearchClient, OpenSearchClient>(client => client.BaseAddress = new Uri(configuration.OpenSearch.Url));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());
            services.AddScoped<ICurrencyClient, CurrencyClient>();
            services.AddScoped<IPricingAvailabilityClient, PricingAvailabilityClient>();
            services.AddScoped<IAmenityClient, AmenityClient>();
            services.AddScoped<IPropertyTypeClient, PropertyTypeClient>();

            services.AddScoped<IGuestFilterService, GuestFilterService>();
            services.AddScoped<IPetFilterService, PetFilterService>();
            services.AddScoped<ILocationFilterService, LocationFilterService>();
            services.AddScoped<IAvailabilityFilterService, AvailabilityFilterService>();
            services.AddScoped<IPriceFilterService, PriceFilterService>();
            services.AddScoped<ISortingService, SortFilterService>();
            services.AddScoped<IAmenitiesFilterService, AmenitiesFilterService>();
            services.AddScoped<IBedroomFilterService, BedroomFilterService>();
            services.AddScoped<IBathroomFilterService, BathroomFilterService>();
            services.AddScoped<IBedFilterService, BedFilterService>();
            services.AddScoped<IPropertyTypeFilterService, PropertyTypeFilterService>();
            services.AddScoped<IAmenityService, AmenityService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<DialogService>();
            services.AddScoped<TooltipService>();

            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();

            services.AddScoped<IPricingAvailabilityService, PricingAvailabilityService>();
            services.AddScoped<IQuoteService, QuoteService>();
            services.AddScoped<IDestinationService, DestinationService>();

            if (string.IsNullOrEmpty(configuration.WhiteLabelSettings.GoogleTagManagerContainerId) == false)
            {
                services.AddGoogleTagManager(options =>
                {
                    options.GtmId = configuration.WhiteLabelSettings.GoogleTagManagerContainerId;
                });
            }

            services.AddI18nText(options =>
            {
                options.PersistanceLevel = PersistanceLevel.Session;
                options.GetInitialLanguageAsync = (_, _) => ValueTask.FromResult(CultureInfo.CurrentUICulture.Name);
            });
        }
    }
}
