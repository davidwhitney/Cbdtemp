using System;
using System.ServiceModel;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [ServiceContract]
    [ServiceKnownType(typeof(NotifyOfCircuitStateResponse))]
    public interface ICircuitBreakerDashboardService
    {
        [OperationContract]
        string[] GetAllCircuitIds();

        [OperationContract]
        DashboardServiceCircuitStateResponse TryGetCircuit(string circuitId);

        [OperationContract]
        DashboardServiceCircuitStateResponse SetCircuitState(CircuitBreakerSetStateRequest request);

        event EventHandler<CircuitBreakerStateNotifiedEventArgs> CircuitBreakerStateNotified;
    }
}