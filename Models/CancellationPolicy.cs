using Leavetown.Shared.Constants;

namespace Leavetown.Client.Models
{
    public sealed class CancellationPolicy
    {
        public static RefundAmount Relaxed { get; } = new(1, null);
        public static RefundAmount Moderate { get; } = new(5, 1);
        public static RefundAmount Firm { get; } = new(14, 7);
        public static RefundAmount Strict { get; } = new(48, 30, 30);
        public static RefundAmount NoRefund { get; } = new(48, 60, null);

        public static RefundAmount Get(CancellationPolicyCode code) => code switch
        {
            CancellationPolicyCode.Relaxed => Relaxed,
            CancellationPolicyCode.Moderate => Moderate,
            CancellationPolicyCode.Firm => Firm,
            CancellationPolicyCode.Strict => Strict,
            CancellationPolicyCode.NoRefund => NoRefund,
            _ => NoRefund, // This should never happen
        };
    }
}
