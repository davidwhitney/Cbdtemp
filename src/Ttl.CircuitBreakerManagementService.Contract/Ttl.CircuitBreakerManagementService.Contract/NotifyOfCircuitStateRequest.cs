using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [DataContract]
    public class NotifyOfCircuitStateRequest
    {

        public NotifyOfCircuitStateRequest()
        {
            InformationalPropertyBag = new Dictionary<string, string>();
            UpdateablePropertyBag = new Dictionary<string, string>();
        }

        [DataMember]
        public long RequestMessageSequenceNumber { get; set; }

        /// <summary>
        /// A guid that identifies the circuitbreaker instance.
        /// </summary>
        [DataMember]
        public string CircuitBreakerId { get; set; }


        [DataMember]
        public CircuitBreakerStatus Status { get; set; }

        /// <summary>
        /// Machine name where the circuitbreaker is running. 
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// Process ID where the circuitbreaker is running. 
        /// </summary>
        [DataMember]
        public int Pid { get; set; }

        [DataMember]
        public Dictionary<string, string> InformationalPropertyBag { get; set; }

        [DataMember]
        public Dictionary<string, string> UpdateablePropertyBag { get; set; }
    }
}