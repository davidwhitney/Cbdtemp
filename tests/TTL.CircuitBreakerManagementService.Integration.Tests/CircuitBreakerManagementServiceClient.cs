using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Ttl.CircuitBreakerManagementService.Contract;

namespace Ttl.CircuitBreakerManagementService.IntegrationTests
{
    public class CircuitBreakerManagementServiceClient : ClientBase<ICircuitBreakerManagementService>,
                                                         ICircuitBreakerManagementService
    {
        public CircuitBreakerManagementServiceClient(Binding binding, EndpointAddress endpoint)
            : base(binding, endpoint)
        {
        }

        public NotifyOfCircuitStateResponse NotifyOfCircuitState(NotifyOfCircuitStateRequest request)
        {
            return Channel.NotifyOfCircuitState(request);
        }        
    }
}