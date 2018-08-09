using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ttl.CircuitBreakerManagementService.Contract;
using log4net;
using log4net.Repository;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public class CircuitBreakerSet : ICircuitBreakers
    {
        private readonly object _circuitBreakerCollectionMonitor = new object(); // for updates to both _circuitBreakers     

        private readonly IDictionary<string, CircuitBreakerState> _circuitBreakers = new Dictionary<string, CircuitBreakerState>();
        private readonly MessageMergingLogic _messageMergingLogic = new MessageMergingLogic();

        private static readonly ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        private static readonly ILog _logStateChanges = LogManager.GetLogger(logRepository.Name, "circuit.changes");
        private static readonly ILog _logStatus = LogManager.GetLogger(logRepository.Name, "circuit.status");

        public void RemoveExpiredCircuitBreakers(DateTime expiryDateTime)
        {
            lock (_circuitBreakerCollectionMonitor)
            {
                var deadCircuitBreakers =
                    _circuitBreakers.Where(pair => pair.Value.LastModifiedDate <= expiryDateTime)
                                    .Select(pair => pair.Key)
                                    .ToList();

                foreach (var breakerId in deadCircuitBreakers)
                {
                    _circuitBreakers.Remove(breakerId);
                }
            }
        }

        /// <summary>
        /// Handles inbound messages that need to be 'merged' into the central state repository. Old messages will be skipped.
        /// </summary>
        /// <param name="statusUpdate"></param>
        /// <returns></returns>
        public CircuitBreakerState ProcessCircuitStateMessage(CircuitBreakerState statusUpdate)
        {
            if (statusUpdate == null) throw new ArgumentNullException("statusUpdate");

            var circuitBreakerId = statusUpdate.CircuitBreakerId;

            CircuitBreakerState dashboardBreakerState;
            CircuitBreakerState resultingState;

            lock (_circuitBreakerCollectionMonitor)
            {
                bool circuitExists = _circuitBreakers.TryGetValue(circuitBreakerId, out dashboardBreakerState);

                resultingState = circuitExists ? _messageMergingLogic.Merge(statusUpdate, dashboardBreakerState) : new CircuitBreakerState(statusUpdate);

                _circuitBreakers[circuitBreakerId] = resultingState;
            }

            bool isNewCircuit = (dashboardBreakerState == null);

            if (isNewCircuit) LogHelper.Log(_logStateChanges, statusUpdate, true);
            else
            {
                if (dashboardBreakerState.Status != resultingState.Status) LogHelper.Log(_logStateChanges, statusUpdate, false);
            }

            LogHelper.Log(_logStatus, statusUpdate, false);

            return resultingState;

        }

        public CircuitBreakerState SetCircuitState(string circuitBreakerId, CircuitBreakerStatus status, DateTime lastModifiedDate)
        {
            lock (_circuitBreakerCollectionMonitor)
            {
                CircuitBreakerState dashboardBreakerState;
                if (!_circuitBreakers.TryGetValue(circuitBreakerId, out dashboardBreakerState)) return null;

                dashboardBreakerState.Status = status;
                dashboardBreakerState.LastModifiedDate = lastModifiedDate;

                return dashboardBreakerState;
            }
        }

        /// <summary>
        /// produces a snapshot of what is currently in the collection
        /// </summary>
        /// <returns></returns>
        public string[] Circuits()
        {
            lock (_circuitBreakerCollectionMonitor)
            {
                return _circuitBreakers.Keys.ToArray();// maybe a clone not needed. Should really experiment with concurrency behaviour of .Keys.                 
            }
        }

        public CircuitBreakerState TryGetCircuit(string circuitId)
        {
            CircuitBreakerState retval;

            lock (_circuitBreakerCollectionMonitor)
            {
                _circuitBreakers.TryGetValue(circuitId, out retval);
            }

            return retval;
        }
    }
}