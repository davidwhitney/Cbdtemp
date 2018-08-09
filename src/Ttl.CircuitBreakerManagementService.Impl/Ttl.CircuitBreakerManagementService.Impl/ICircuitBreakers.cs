using System;
using Ttl.CircuitBreakerManagementService.Contract;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public interface ICircuitBreakers
    {
        string[] Circuits();

        CircuitBreakerState TryGetCircuit(string circuitId);

        CircuitBreakerState ProcessCircuitStateMessage(CircuitBreakerState statusUpdate);

        CircuitBreakerState SetCircuitState(string circuitBreakerId, CircuitBreakerStatus status, DateTime lastModifiedDate);

        void RemoveExpiredCircuitBreakers(DateTime expiryDateTime);
    }
}