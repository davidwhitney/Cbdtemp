using System.ServiceModel;

namespace Ttl.CircuitBreakerManagementService.Contract
{
    [ServiceKnownType(typeof (NotifyOfCircuitStateRequest))]
    [ServiceKnownType(typeof (NotifyOfCircuitStateResponse))]
    [ServiceContract]
    public interface ICircuitBreakerManagementService
    {
        [OperationContract(Name = "NotifyOfCircuitState")]
        NotifyOfCircuitStateResponse NotifyOfCircuitState(NotifyOfCircuitStateRequest request);
    }
}