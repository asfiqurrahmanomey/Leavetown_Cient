namespace Leavetown.Client.Models
{
    public partial class RefundAmount
    {
        public int? FullRefundDaysRequired { get; }
        public int? FullRefundGraceHours { get; }
        public int? FullRefundGraceDaysRequired { get; }
        public int? HalfRefundDaysRequired { get; }

        public RefundAmount(int? fullRefundDaysRequired, int? halfRefundDaysRequired)
            : this(fullRefundDaysRequired, null, null, halfRefundDaysRequired)
        {
        }

        public RefundAmount(int fullRefundGraceHours, int fullRefundGraceDaysRequired, int? halfRefundDaysRequired)
            : this(null, fullRefundGraceHours, fullRefundGraceDaysRequired, halfRefundDaysRequired)
        {
        }

        private RefundAmount(int? fullRefundDaysRequired, int? fullRefundGraceHours, int? fullRefundGraceDaysRequired, int? halfRefundDaysRequired)
        {
            FullRefundDaysRequired = fullRefundDaysRequired;
            FullRefundGraceHours = fullRefundGraceHours;
            FullRefundGraceDaysRequired = fullRefundGraceDaysRequired;
            HalfRefundDaysRequired = halfRefundDaysRequired;
        }
    }
}
