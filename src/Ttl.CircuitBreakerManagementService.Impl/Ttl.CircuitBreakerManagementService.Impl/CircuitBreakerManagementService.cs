using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using Ttl.CircuitBreakerManagementService.Contract;
using log4net;
using log4net.Repository;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public class CircuitBreakerManagementService : ICircuitBreakerManagementService, ICircuitBreakerDashboardService, IDisposable
    {
        private readonly ICircuitBreakers _circuitBreakers;
        private readonly ICollectGarbageBreakers _cleanupTimer;
        private const int OneMinuteInMilliseconds = 60 * 1000;
        
        /// <summary>
        /// initializes service with 1 minute polling interval and 1 minute expiry for unseen breakers.
        /// </summary>
        public CircuitBreakerManagementService()
        {
            _circuitBreakers = new CircuitBreakerSet();
            _cleanupTimer = new TimedCleanupCircuitBreakerSet(OneMinuteInMilliseconds, OneMinuteInMilliseconds, _circuitBreakers);
        }

        public CircuitBreakerManagementService(ICircuitBreakers circuitBreakers, ICollectGarbageBreakers garbageCollector)
        {
            if (circuitBreakers == null) throw new ArgumentNullException("circuitBreakers");
            if (garbageCollector == null) throw new ArgumentNullException("garbageCollector");

            _circuitBreakers = circuitBreakers;
            _cleanupTimer = garbageCollector;
        }

        public event EventHandler<CircuitBreakerStateNotifiedEventArgs> CircuitBreakerStateNotified;

        public NotifyOfCircuitStateResponse NotifyOfCircuitState(NotifyOfCircuitStateRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            
            var updateRequest = MapNotificationToCircuitBreakerState(request);
                        
            var updateResponse = _circuitBreakers.ProcessCircuitStateMessage(updateRequest);

            var notifyOfCircuitStateResponse = MapCircuitBreakerStateToResponse(updateResponse);

            if (CircuitBreakerStateNotified != null)
                CircuitBreakerStateNotified(request, new CircuitBreakerStateNotifiedEventArgs(request));

            return notifyOfCircuitStateResponse;
        }

      

        private static CircuitBreakerState MapNotificationToCircuitBreakerState(NotifyOfCircuitStateRequest request)
        {
            CircuitBreakerState updateRequest = new CircuitBreakerState(request.CircuitBreakerId, request.MachineName, request.Pid, request.Status, request.RequestMessageSequenceNumber);

            updateRequest.UpdateablePropertyBag = new Dictionary<string, string>(request.UpdateablePropertyBag);
            updateRequest.InformationalPropertyBag = new Dictionary<string, string>(request.InformationalPropertyBag);
            return updateRequest;
        }

        public string[] GetAllCircuitIds()
        {
            return _circuitBreakers.Circuits();
        }

        public DashboardServiceCircuitStateResponse TryGetCircuit(string circuitId)
        {
            if (circuitId == null) throw new ArgumentNullException("circuitId");

            var cb = _circuitBreakers.TryGetCircuit(circuitId);
            return MapCircuitBreakerStateToDashboardServiceReponse(cb);
        }

        public DashboardServiceCircuitStateResponse SetCircuitState(CircuitBreakerSetStateRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var response = _circuitBreakers.SetCircuitState(request.CircuitBreakerId, request.Status, SystemTime.Now());
            return MapCircuitBreakerStateToDashboardServiceReponse(response);
        }

        private DashboardServiceCircuitStateResponse MapCircuitBreakerStateToDashboardServiceReponse(CircuitBreakerState circuit)
        {
            return new DashboardServiceCircuitStateResponse
                {
                    Id = circuit.CircuitBreakerId,
                    LastModifiedDate = circuit.LastModifiedDate,
                    LastReceivedStatus = circuit.LastReceivedStatus,
                    MachineName = circuit.MachineName,
                    MessageSequenceNumber = circuit.MessageSequenceNumber,
                    ProcessId = circuit.ProcessId,
                    Status = circuit.Status,
                    UpdateablePropertyBag = circuit.UpdateablePropertyBag,
                    ReadonlyPropertyBag = circuit.InformationalPropertyBag
                };
        }

        public void Dispose()
        {
            _cleanupTimer.Dispose();
        }

        private static NotifyOfCircuitStateResponse MapCircuitBreakerStateToResponse(CircuitBreakerState response)
        {
            return new NotifyOfCircuitStateResponse
            {
                Id = response.CircuitBreakerId,
                Status = response.Status,
                MessageSequenceNumber = response.MessageSequenceNumber,
                UpdateablePropertyBag = response.UpdateablePropertyBag
            };
        }
    }

    public interface ICollectGarbageBreakers : IDisposable
    {
    }
}