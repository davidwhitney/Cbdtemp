//using System.Collections.Generic;
//using System.Configuration;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using Ttl.CircuitBreakerManagementService.Contract;

//namespace Ttl.CircuitBreakerManagementService.Dashboard
//{
//    [HubName("managementHub")]
//    public class ManagementServiceHub : Hub
//    {
//        public Impl.CircuitBreakerManagementService ManagementService { get; set; }

//        private bool IsAdminRole
//        {
//            get { return false; }
//        }

//        public void ForceClosed(string id)
//        {
//            SetState(id, CircuitBreakerStatus.ForcedClosed);
//        }

//        public void ForceOpen(string id)
//        {
//            SetState(id, CircuitBreakerStatus.ForcedOpen);
//        }

//        public void ForceHalfOpen(string id)
//        {
//            SetState(id, CircuitBreakerStatus.ForcedHalfOpen);
//        }

//        public void UpdatePropertyBag(string id, Dictionary<string, string> properties)
//        {
//            if (!IsAdminRole)
//                return;

//            var circuit = ManagementService.TryGetCircuit(id);
//            if (circuit != null)
//                ManagementService.SetCircuitState(new CircuitBreakerSetStateRequest { CircuitBreakerId = id, PropertyBag = properties });
//        }

//        private void SetState(string id, CircuitBreakerStatus circuitBreakerStatus)
//        {
//            if (!IsAdminRole)
//                return;

//            var circuit = ManagementService.TryGetCircuit(id);
//            if (circuit != null)
//                ManagementService.SetCircuitState(new CircuitBreakerSetStateRequest { CircuitBreakerId = id, Status = circuitBreakerStatus });
//        }
//    }
//}