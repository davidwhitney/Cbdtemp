using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [DataContract]
    public class DashboardServiceCircuitStateResponse
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public CircuitBreakerStatus Status { get; set; }

        [DataMember]
        public long MessageSequenceNumber { get; set; }

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public int ProcessId { get; set; }

        [DataMember]
        public CircuitBreakerStatus LastReceivedStatus { get; set; }

        [DataMember]
        public DateTime LastModifiedDate { get; set; }

        [DataMember]
        public Dictionary<string, string> UpdateablePropertyBag { get; set; }
        
        [DataMember]
        public Dictionary<string, string> ReadonlyPropertyBag { get; set; }
    }
}