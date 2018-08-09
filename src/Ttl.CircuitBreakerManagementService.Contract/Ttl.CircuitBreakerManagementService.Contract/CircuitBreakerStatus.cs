using System.Runtime.Serialization;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [DataContract]
    public enum CircuitBreakerStatus
    {
        [EnumMember] Open,
        [EnumMember] HalfOpen,
        [EnumMember] Closed,
        [EnumMember] ForcedOpen,
        [EnumMember] ForcedHalfOpen,
        [EnumMember] ForcedClosed
    }
}