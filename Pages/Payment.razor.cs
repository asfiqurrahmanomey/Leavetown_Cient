using System.Web;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Pages;

public partial class Payment
{
    [Parameter] public string? Culture { get; set; }
    [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    [CascadingParameter] public Configuration Configuration { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!; 
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
    [Inject] private ILogger<Payment> Logger { get; set; } = default!;
    [Inject] private ILeavetownClient LeavetownClient { get; set; } = default!;

    private CurrencyModel? _currency = default!;
    private string _heading = string.Empty;
    private string _subHeading = string.Empty;
    private string _imageUrl = string.Empty;
    private string _submitButtonText = string.Empty;
    private string _clientKey = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hideSkeleton = true;
    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(NavigationManager.GetQuery());
                var currencyCode = query["c"] ?? "";
                var paymentIntentId = query["pid"] ?? "";
                _currency = CurrencyService.GetCurrencyFromCode(currencyCode);
                var privateKey = StripeSettings.GetStripeApiKey(currencyCode);
                var payment = await LeavetownClient.GetPayment(paymentIntentId, currencyCode);

                if (payment == null)
                {
                    Logger.LogError($"Error on payment page: could not find payment intent with {paymentIntentId} for currency {currencyCode}");
                    throw new Exception();
                }

                _clientKey = payment.ClientSecret;
                _heading = payment.Description;
                _subHeading = payment.Title;
                _imageUrl = payment.ImageUrl;
                _submitButtonText = $"{ResourcesShared.PleasePay}  {_currency?.Symbol}{(payment.Amount / 100):##,#.00}";
                _isLoading = false;

                StateHasChanged();

                await LoadStripePaymentFields(privateKey);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Error attempting to create the payment link page.");
                _errorMessage = ResourcesShared.InvalidPaymentRequest;
                _hideSkeleton = false;
                StateHasChanged();
            }
        }
    }

    private async Task LoadStripePaymentFields(string privateKey) =>
        await JSRuntime.InvokeVoidAsync("loadStripePaymentFields", privateKey, _clientKey, _currency?.Code, ResourcesShared.PaymentPageGenericError);
}
