using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace Leavetown.Client.Pages;

public partial class PaymentThankYou
{
    [Parameter] public string? Culture { get; set; }
    [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ILogger<Payment> Logger { get; set; } = default!;
    [Inject] private ILeavetownClient LeavetownClient { get; set; } = default!;

    private string _heading = string.Empty;
    private string _subHeading = string.Empty;
    private string _imageUrl = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(NavigationManager.GetQuery());
                var currencyCode = query["c"] ?? "";
                var paymentIntentId = query["payment_intent"] ?? "";
                var payment = await LeavetownClient.GetPayment(paymentIntentId, currencyCode);

                if (payment == null)
                {
                    Logger.LogError($"Error on payment page: could not find payment intent with {paymentIntentId} for currency {currencyCode}");
                    throw new Exception();
                }

                _heading = payment.Description;
                _subHeading = payment.Title;
                _imageUrl = payment.ImageUrl;
                _isLoading = false;

                StateHasChanged();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Error attempting to create the payment link page.");
                _errorMessage = ResourcesShared.InvalidPaymentRequest;
                StateHasChanged();
            }
        }
    }
}