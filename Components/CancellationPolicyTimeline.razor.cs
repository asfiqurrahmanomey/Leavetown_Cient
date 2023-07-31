using Microsoft.AspNetCore.Components;
using Leavetown.Client.Models;
using Leavetown.Shared.Constants;
using Leavetown.Client.i18ntext.Extensions;

namespace Leavetown.Client.Components
{
    public partial class CancellationPolicyTimeline
    {
        [Parameter] public CancellationPolicyCode PolicyCode { get; set; }
        [Parameter] public DateTime? Checkin { get; set; }
        [Parameter] public DateTime? Checkout { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private RefundAmount _refundAmount = new(null, null);
        private bool _checkinHasValue;

        private DateTime? _halfRefundDeadline;
        private DateTime? _fullRefundDeadline;

        private bool _isHalfRefundVisible;

        private bool _isFullRefundAvailable;
        private bool _isHalfRefundAvailable;

        private bool _isFullRefundActive;
        private bool _isHalfRefundActive;
        private bool _isNoRefundActive;

        private string? _fullRefundTimelineText;
        private string? _fullRefundPolicyText;

        private string? _halfRefundTimelineText;
        private string? _halfRefundPolicyText;
        private string? _noRefundPolicyText;

        protected override async Task OnParametersSetAsync()
        {
            _refundAmount = GetRefundAmount();
            _checkinHasValue = AssertCheckinHasValue();

            _halfRefundDeadline = GetHalfRefundDeadline(_refundAmount, _checkinHasValue);
            _fullRefundDeadline = GetFullRefundDeadline(_refundAmount, _checkinHasValue, _halfRefundDeadline);

            _isHalfRefundVisible = AssertHalfRefundIsVisible(_refundAmount, _checkinHasValue, _halfRefundDeadline, _fullRefundDeadline);

            _isHalfRefundAvailable = AssertHalfRefundIsAvailable(_halfRefundDeadline);
            _isFullRefundAvailable = AssertFullRefundIsAvailable(_fullRefundDeadline);
            
            _isFullRefundActive = AssertFullRefundIsActive(_checkinHasValue, _isFullRefundAvailable);
            _isHalfRefundActive = AssertHalfRefundIsActive(_checkinHasValue, _isHalfRefundAvailable, _isFullRefundAvailable);
            _isNoRefundActive = AssertNoRefundActive(_checkinHasValue, _isHalfRefundAvailable, _isFullRefundAvailable);

            _halfRefundTimelineText = GetHalfRefundTimelineText(_refundAmount, _halfRefundDeadline);
            _halfRefundPolicyText = GetHalfRefundPolicyText(_refundAmount, _halfRefundDeadline);

            _fullRefundTimelineText = GetFullRefundTimelineText(_refundAmount, _isNoRefundActive, _halfRefundDeadline, _fullRefundDeadline);
            _fullRefundPolicyText = GetFullRefundPolicyText(_refundAmount, _checkinHasValue, _halfRefundDeadline, _fullRefundDeadline);

            _noRefundPolicyText = GetNoRefundPolicyText(_refundAmount, _halfRefundDeadline, _fullRefundDeadline);

            await base.OnParametersSetAsync();
        }

        private bool AssertCheckinHasValue() =>
            Checkin.HasValue
            && Checkin != DateTime.MinValue;

        private RefundAmount GetRefundAmount() => CancellationPolicy.Get(PolicyCode);

        private DateTime? GetHalfRefundDeadline(RefundAmount refundAmount, bool checkinHasValue)
        {
            if (!checkinHasValue
                || !refundAmount.HalfRefundDaysRequired.HasValue) return null;

            DateTime deadline = Checkin!.Value.AddDays(-refundAmount.HalfRefundDaysRequired.Value);
            return deadline;
        }

        private DateTime? GetFullRefundDeadline(RefundAmount refundAmount, bool checkinHasValue, DateTime? halfRefundDeadline)
        {
            if (!checkinHasValue) return null;

            DateTime? deadline = null;

            if (refundAmount.FullRefundDaysRequired.HasValue)
            {
                deadline = Checkin!.Value.AddDays(-refundAmount.FullRefundDaysRequired.Value);
            }
            else if (refundAmount.FullRefundGraceHours.HasValue)
            {
                var dates = new List<DateTime>
                (
                    new[]
                    {
                        DateTime.Now.AddHours(refundAmount.FullRefundGraceHours.Value),
                        Checkin!.Value
                    }
                );

                if (refundAmount.FullRefundGraceDaysRequired.HasValue)
                {
                    dates.Add(Checkin!.Value.AddDays(-refundAmount.FullRefundGraceDaysRequired.Value));
                }

                if (halfRefundDeadline.HasValue)
                {
                    dates.Add(halfRefundDeadline.Value);
                }

                deadline = dates.Min();
            }

            return deadline;
        }

        private bool AssertHalfRefundIsVisible(RefundAmount refundAmount, bool checkinHasValue, DateTime? halfRefundDeadline, DateTime? fullRefundDeadline)
        {
            bool isVisible = false;
            if (!checkinHasValue)
            {
                isVisible = refundAmount.HalfRefundDaysRequired.HasValue;
            }
            else if (halfRefundDeadline.HasValue
                        && (halfRefundDeadline < DateTime.Now
                            || (fullRefundDeadline.HasValue
                                && fullRefundDeadline.Value < halfRefundDeadline.Value)))
            {
                isVisible = true;
            }

            return isVisible;
        }

        private bool AssertHalfRefundIsAvailable(DateTime? halfRefundDeadline) => halfRefundDeadline.GetValueOrDefault() > DateTime.Now;
        private bool AssertFullRefundIsAvailable(DateTime? fullRefundDeadline) => fullRefundDeadline.GetValueOrDefault() > DateTime.Now;
        
        private bool AssertFullRefundIsActive(bool checkinHasValue, bool isFullRefundAvailable) => checkinHasValue && isFullRefundAvailable;
        private bool AssertHalfRefundIsActive(bool checkinHasValue, bool isHalfRefundAvailable, bool isFullRefundAvailable) => checkinHasValue && isHalfRefundAvailable && !isFullRefundAvailable;
        private bool AssertNoRefundActive(bool checkinHasValue, bool isHalfRefundAvailable, bool isFullRefundAvailable) => checkinHasValue && !isHalfRefundAvailable && !isFullRefundAvailable ;

        private string? GetFullRefundTimelineText(RefundAmount refundAmount, bool isNoRefundActive, DateTime? halfRefundDeadline, DateTime? fullRefundDeadline)
        {
            string? text = null;

            if (refundAmount.FullRefundGraceHours.HasValue)
            {
                if (isNoRefundActive
                    || !halfRefundDeadline.HasValue
                    || DateTime.Now.AddHours(refundAmount.FullRefundGraceHours.Value) < halfRefundDeadline.Value)
                {
                    text = string.Format(ResourcesShared.CancellationTimelineAfterBooking, ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value));
                }
                else
                {
                    text = $"{halfRefundDeadline:MMM dd}";
                }
            }
            else if (fullRefundDeadline.HasValue)
            {
                text = $"{fullRefundDeadline:MMM dd}";
            }
            else if (refundAmount.FullRefundDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationTimelineBeforeCheckin, ResourcesShared.GetDaysValue(refundAmount.FullRefundDaysRequired.Value));
            }

            return text;
        }

        private string? GetFullRefundPolicyText(RefundAmount refundAmount, bool checkinHasValue, DateTime? halfRefundDeadline, DateTime? fullRefundDeadline)
        {
            string? text = null;

            if (checkinHasValue)
            {
                if (halfRefundDeadline.HasValue
                    && refundAmount.FullRefundGraceHours.HasValue)
                {
                    text = string.Format(ResourcesShared.CancellationPolicyWithinAfterBookingAndBefore, halfRefundDeadline.Value.ToString("MMM dd, yyyy"), ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value));
                }
                else if (fullRefundDeadline.HasValue)
                {
                    if (refundAmount.FullRefundGraceHours.HasValue)
                    {
                        text = string.Format(ResourcesShared.CancellationPolicyWithinAfterBookingAndBefore, fullRefundDeadline.Value.ToString("MMM dd, yyyy"), ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value));
                    }
                    else
                    {
                        text = string.Format(ResourcesShared.CancellationPolicyCancelBefore, fullRefundDeadline.Value.ToString("MMM dd, yyyy"));
                    }
                }
            }
            else
            {
                if (refundAmount.FullRefundDaysRequired.HasValue)
                {
                    text = string.Format(ResourcesShared.CancellationPolicyMoreThanBeforeCheckIn, ResourcesShared.GetDaysValue(refundAmount.FullRefundDaysRequired.Value));
                }
                else if (refundAmount.FullRefundGraceHours.HasValue
                            && refundAmount.FullRefundGraceDaysRequired.HasValue)
                {
                    text = string.Format(ResourcesShared.CancellationPolicyWithinAfterBookingAndBeforeCheckIn, ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value), ResourcesShared.GetDaysValue(refundAmount.FullRefundGraceDaysRequired.Value));
                }
            }

            return text;
        }

        private string? GetHalfRefundTimelineText(RefundAmount refundAmount, DateTime? halfRefundDeadline)
        {
            string? text = null;

            if (halfRefundDeadline.HasValue)
            {
                text = $"{halfRefundDeadline:MMM dd}";
            }
            else if (refundAmount.HalfRefundDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationTimelineBeforeCheckin, ResourcesShared.GetDaysValue(refundAmount.HalfRefundDaysRequired.Value));
            }

            return text;
        }

        private string? GetHalfRefundPolicyText(RefundAmount refundAmount, DateTime? halfRefundDeadline)
        {
            string? text = null;

            if (halfRefundDeadline.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyCancelBefore, halfRefundDeadline.Value.ToString("MMM dd, yyyy"));
            }
            else if (refundAmount.HalfRefundDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyMoreThanBeforeCheckIn, ResourcesShared.GetDaysValue(refundAmount.HalfRefundDaysRequired.Value));
            }

            return text;
        }

        private string? GetNoRefundPolicyText(RefundAmount refundAmount, DateTime? halfRefundDeadline, DateTime? fullRefundDeadline)
        {
            string? text = null;

            if (halfRefundDeadline.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyCancelAfter, halfRefundDeadline.Value.ToString("MMM dd, yyyy"));
            }
            else if (refundAmount.HalfRefundDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyLessThanBeforeCheckIn, ResourcesShared.GetDaysValue(refundAmount.HalfRefundDaysRequired.Value));
            }
            else if (fullRefundDeadline.HasValue)
            {
                if (refundAmount.FullRefundGraceHours.HasValue)
                {
                    text = string.Format(ResourcesShared.CancellationPolicyMoreThanAfterBookingOrAfter, fullRefundDeadline.Value.ToString("MMM dd, yyyy"), ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value));
                }
                else
                {
                    text = string.Format(ResourcesShared.CancellationPolicyCancelAfter, fullRefundDeadline.Value.ToString("MMM dd, yyyy"));
                }
            }
            else if (refundAmount.FullRefundDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyLessThanBeforeCheckIn, ResourcesShared.GetDaysValue(refundAmount.FullRefundDaysRequired.Value));
            }
            else if (refundAmount.FullRefundGraceHours.HasValue
                        && refundAmount.FullRefundGraceDaysRequired.HasValue)
            {
                text = string.Format(ResourcesShared.CancellationPolicyMoreThanAfterBookingOrLessThanBeforeCheckin, ResourcesShared.GetHoursValue(refundAmount.FullRefundGraceHours.Value), ResourcesShared.GetDaysValue(refundAmount.FullRefundGraceDaysRequired.Value));
            }

            return text;
        }
    }
}
