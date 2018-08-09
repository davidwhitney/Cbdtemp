using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [DataContract]
    public class NotifyOfCircuitStateResponse
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public CircuitBreakerStatus Status { get; set; }

        [DataMember]
        public long MessageSequenceNumber { get; set; }

        [DataMember]
        public Dictionary<string, string> UpdateablePropertyBag { get; set; }
    }
}