using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [DataContract]
    public class CircuitBreakerSetStateRequest
    {
        /// <summary>
        /// A guid that identifies the circuitbreaker instance.
        /// </summary>
        [DataMember]
        public string CircuitBreakerId { get; set; }

        [DataMember]
        public CircuitBreakerStatus Status { get; set; }

        [DataMember]
        public Dictionary<string, string> PropertyBag { get; set; }
    }
}