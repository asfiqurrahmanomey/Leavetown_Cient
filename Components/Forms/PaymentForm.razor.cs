using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components.Forms.Contracts;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Models.Projections;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Stripe;
using Leavetown.Shared.Models.PartnerApi;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels;

namespace Leavetown.Client.Components.Forms;

public partial class PaymentForm : IFormComponent<BookingViewModel>, IDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ILeavetownClient LeavetownClient { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
    [Inject] private IExchangeRateService ExchangeRateService { get; set; } = default!;
    [Inject] private ILogger<PaymentForm> Logger { get; set; } = default!;
    [Inject] private IRenderState RenderState { get; set; } = default!;

    [Parameter] public string? Title { get; set; } = "";
    [Parameter] public BookingViewModel Data { get; set; } = new();
    [Parameter] public ListingDetailsViewModel Listing { get; set; } = new();
    [Parameter] public QuoteModel Quote { get; set; } = new();
    [Parameter] public string ReturnUrl { get; set; } = "";
    [CascadingParameter] public Configuration Configuration { get; set; } = default!;
    [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
    [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    [Parameter] public string Culture { get; set; } = "";

    private List<CountryModel> _ddlCountriesData = new();

    private bool _billingAddressFormDisplayed = false;
    private string _billingAddressFormId = "billing-address-form";
    private bool _disposed = false;
    private CurrencyModel? _currency;
    private CurrencyModel? _bookingCurrency;
    private string? _salesChannel;
    private string? _externalChannel;
    private bool _disposedValue;
    private List<string>? _errorMessageCodes;
    private bool _bookingPending = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        CurrencyService.CurrencyChanged += OnCurrencyChanged;

        await JSRuntime.SetDotNetReference(this);

        var countries = await LeavetownClient.GetCountryModelsAsync(Culture);
        if (countries != null) _ddlCountriesData.AddRange(countries);

        _currency = await CurrencyService.GetLocalCurrencyAsync();

        _bookingCurrency = CurrencyService.GetCurrencyFromCode(Quote.CurrencyCode);

        await SetStripeAccountAsync();
        _salesChannel = Configuration.SalesChannel;
        _externalChannel = Configuration.WhiteLabelSettings.WhiteLabelIdentifier;
        await UpdateStripeFormAsync();
        StateHasChanged();
    }

    [JSInvokable(nameof(SubmitToken))]
    public void SubmitToken(StripeCreateCardResponse? response)
    {
        try
        {                
            if (response == null) throw new ArgumentNullException($"Stripe create card response is null.");

            if (response.Error != null)
            {
                InvokeAsync(async () =>
                {
                    await JSRuntime.InvokeVoidAsync("showOrHideStripeError", response.Error);                        
                    _bookingPending = false;
                    StateHasChanged();
                });
                   
                return;
            }

            if (response.Token == null) throw new StripeException($"Stripe create card token is null.");

            Logger.LogInformation("{Token}", response.Token.Id);

            BookingRequestModel bookingRequestModel = new()
            {
                AddressLine1 = Data.BillingAddressDetails?.Address1?.Trim(),
                AddressLine2 = Data.BillingAddressDetails?.Address2?.Trim(),
                CheckInDate = Data.CheckIn,
                CheckOutDate = Data.CheckOut,
                City = Data.BillingAddressDetails?.City,
                CountryCode = Data.BillingAddressDetails?.Country?.Code ?? "",
                CreditCardToken = response!.Token!.Id,
                CurrencyCode = Listing.CurrencyCode,
                ExpectedTotal = Quote.Total,
                // Payment amount is equal to either Deposit amount, if deposit date does not equal balance date,
                //  or Balance amount + Deposit Amount if deposit date does equal balance date
                DepositAmount = Quote?.PaymentAmount ?? 0,
                GuestEmail = Data.Email?.Trim(),
                GuestPhoneNumber = Data.PhoneNumber?.Trim(),
                GuestFirstName = Data.FirstName?.Trim(),
                GuestLastName = Data.LastName?.Trim(),
                NumberAdults = Data.NumberOfAdults,
                NumberChildren = Data.NumberOfChildren,
                NumberPets = Data.NumberOfPets,
                PostalCode = Data.BillingAddressDetails?.PostalCode?.Trim(),
                ProvinceStateCode = Data.BillingAddressDetails?.ProvinceState?.Trim(),
                RoomTypeId = Data.ListingId,
                SalesChannel = _salesChannel,
                ExternalChannel = _externalChannel
            };

            InvokeAsync(async () =>
            {
                var result = await LeavetownClient.SendBookingRequestAsync(bookingRequestModel);
                if (result == null)
                {
                    throw new Exception("Failed to receive response from Api Client. Result is null.");
                }
                else if (!result.Successful)
                {
                    Logger.LogError("Booking unsuccessful");
                    _errorMessageCodes = result.Errors.ToList();
                    await JSRuntime.InvokeVoidAsync("setConfirmAndPayButtonStatus");
                }
                else if (result.Successful)
                {
                    NavigationManager.NavigateTo($"/{Culture}/checkout/{result.EntityId}/confirmed{NavigationManager.GetQuery()}");
                }

                _bookingPending = false;
                StateHasChanged();
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private async Task SetStripeAccountAsync() => await JSRuntime.InvokeVoidAsync("setStripeAccount", StripeSettings.GetStripeApiKey(_bookingCurrency?.Code));

    private async Task UpdateStripeFormAsync()
    {
        await JSRuntime.InvokeVoidAsync("initialize");
    }

    private void DisplayBillingAddressForm()
    {
        _billingAddressFormDisplayed = !_billingAddressFormDisplayed;
        Data.BillingAddressDetails = Data.ContactAddressDetails.ShallowCopy();
    }

    private void NavigateToReturnUrl()
    {
        NavigationManager.NavigateTo(ReturnUrl);
        Data.BillingAddressDetails = Data.ContactAddressDetails;
    }

    private void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e) =>
        InvokeAsync(async () =>
        {
            _currency = e.CurrencyModel;
            await UpdateStripeFormAsync();
            StateHasChanged();
        });

    private void OnTermAndConditionsCheckboxChecked(bool isChecked) => InvokeAsync(() =>
    {
        JSRuntime.InvokeVoidAsync("onTermsAndConditionsChange", isChecked);
        StateHasChanged();
    });


    private void OnSubmitButtonClicked()
    {
        _bookingPending = true;
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CurrencyService.CurrencyChanged -= OnCurrencyChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
