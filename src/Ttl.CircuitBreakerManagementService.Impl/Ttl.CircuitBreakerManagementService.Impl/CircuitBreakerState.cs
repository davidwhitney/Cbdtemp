using System;
using System.Collections.Generic;
using Ttl.CircuitBreakerManagementService.Contract;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public class CircuitBreakerState
    {
        public CircuitBreakerState(string circuitBreakerId, string machineName, int pid)
        {
            CircuitBreakerId = circuitBreakerId;
            MachineName = machineName;
            ProcessId = pid;
            LastModifiedDate = SystemTime.Now();
            MessageSequenceNumber = long.MinValue;
            Status = CircuitBreakerStatus.Closed;
            InformationalPropertyBag = new Dictionary<string, string>();
            UpdateablePropertyBag = new Dictionary<string, string>();
        }

        public CircuitBreakerState(string circuitBreakerId, string machineName, int pid, CircuitBreakerStatus status, long messageSequenceNumber)
            : this(circuitBreakerId, machineName, pid)
        {
            Status = status;
            MessageSequenceNumber = messageSequenceNumber;
            InformationalPropertyBag = new Dictionary<string, string>();
            UpdateablePropertyBag = new Dictionary<string, string>();
        }


        public CircuitBreakerState(CircuitBreakerState source)
        {
            CircuitBreakerId = source.CircuitBreakerId;
            MachineName = source.MachineName;
            ProcessId = source.ProcessId;

            MessageSequenceNumber = source.MessageSequenceNumber;
            Status = source.Status;
            LastReceivedStatus = source.LastReceivedStatus;
            LastModifiedDate = SystemTime.Now();
            InformationalPropertyBag = new Dictionary<string, string>();
            UpdateablePropertyBag = new Dictionary<string, string>();
        }

        #region Immutable properties

        public string CircuitBreakerId { get; private set; }
        public string MachineName { get; private set; }
        public int ProcessId { get; private set; }      

        #endregion

        public long MessageSequenceNumber { get; set; }
        public CircuitBreakerStatus Status { get; set; }
        public CircuitBreakerStatus LastReceivedStatus { get; set; }
        public DateTime LastModifiedDate { get; set; } // only mutable for test purposes
        
        public Dictionary<string, string> InformationalPropertyBag { get; set; }
        public Dictionary<string, string> UpdateablePropertyBag { get; set; }
    }
}