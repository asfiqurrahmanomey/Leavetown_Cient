using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Helpers;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;
using Leavetown.Shared.Models.ViewModels.Contracts;
using Configuration = Leavetown.Client.Utilities.Settings.Configuration;

namespace Leavetown.Client.Services;

public class QuoteService : IQuoteService, IDisposable
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private ILeavetownClient _leavetownClient;
    private string? _salesChannel;
    private bool disposedValue;

    public QuoteService(ILeavetownClient leavetownClient, Configuration configuration)
    {
        _leavetownClient = leavetownClient;
        _salesChannel = configuration.SalesChannel;
    }

    public async Task<QuoteModel> UpdateQuoteAsync(int listingId, IQuoteListingParams quoteListingParams, DateTime checkInTime, DateTime checkOutTime, int adultCount, int childCount, int petCount)
    {
        if (_salesChannel == null) return new QuoteModel
        {
            BookingErrorCodes = new string[] { "BookingErrorSalesChannelNull" }
        };

        var request = new QuoteRequestModel
        {
            CheckInDate = checkInTime,
            CheckOutDate = checkOutTime,
            NumberAdults = adultCount,
            NumberChildren = childCount,
            NumberPets = petCount,
            RoomTypeId = listingId,
            SalesChannel = _salesChannel
        };

        var quote = await _leavetownClient.SendQuoteRequestAsync(request);

        // Setting PrePromotionalTotal separately here to avoid having to refactor
        //  Cirrus quote engine to include this value.
        quote.PrePromotionalTotal = CalculateQuoteTotal(
            quote!.OriginalRentalTotal,
            quoteListingParams,
            checkInTime.StayNights(checkOutTime),
            adultCount,
            petCount);

        return quote;
    }

    public decimal CalculateQuoteTotal(decimal rentalTotal, IQuoteListingParams listingParams, int nights, int adults, int pets)
    {
        var serviceFee = (rentalTotal * listingParams.ServiceFeeRate).RoundToEven(2);
        var extraAdultFees = CalculateExtraAdultTotal(listingParams.StandardOccupants, listingParams.ExtraAdultFee, adults, nights);
        var petFees = CalculatePetTotal(listingParams.PetFeePerNight, listingParams.PetFeeMax, pets, nights);
        var cleaningFee = listingParams.CleaningFee;
        var autohostFee = listingParams.AutohostFee;
        var preTaxTotal = rentalTotal + serviceFee + extraAdultFees + petFees + cleaningFee + autohostFee;
        var totalFlatTax = (listingParams.FlatTax * nights).RoundToEven(2);
        var taxesAndFees = ((preTaxTotal * listingParams.TaxRate) + totalFlatTax).RoundToEven(2);
        var total = preTaxTotal + taxesAndFees;
        return total;
    }

    private static decimal CalculateExtraAdultTotal(int standardOccupants, decimal extraAdultFee, int adults, int nights)
    {
        int extraAdults = Math.Max(adults - standardOccupants, 0);
        return (extraAdults * extraAdultFee * nights).RoundToEven(2);
    }

    private static decimal CalculatePetTotal(decimal feePerPetPerNight, decimal feePerPetMax, int pets, int nights)
    {            
        decimal feeNormal = feePerPetPerNight * pets * nights;
        decimal feeMax = feePerPetMax * pets;
        return (feeMax > 0m ? Math.Min(feeNormal, feeMax) : feeNormal).RoundToEven(2);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }        
}
