using System;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    public class CircuitBreakerStateNotifiedEventArgs : EventArgs
    {
        public CircuitBreakerStateNotifiedEventArgs(NotifyOfCircuitStateRequest stateRequest)
        {
            StateRequest = stateRequest;
        }

        public NotifyOfCircuitStateRequest StateRequest { get; private set; }
    }
}