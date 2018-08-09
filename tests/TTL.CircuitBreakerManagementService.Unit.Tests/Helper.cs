using System;
using Ttl.CircuitBreakerManagementService.Contract;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    internal class Helper
    {
        public static CircuitBreakerState MakeBreakerState(int messageSequenceNumber)
        {
            return new CircuitBreakerState(Guid.NewGuid().ToString("N"), "machine", 1)
                {
                    MessageSequenceNumber = messageSequenceNumber,
                    Status = CircuitBreakerStatus.Closed
                };
        }

        public static CircuitBreakerState MakeBreakerState(string id, int messageSequenceNumber)
        {
            return new CircuitBreakerState(id, "machine", 1)
                {
                    MessageSequenceNumber = messageSequenceNumber,
                    Status = CircuitBreakerStatus.Closed
                };
        }

        public static CircuitBreakerState MakeBreakerState(int messageSequenceNumber, CircuitBreakerStatus status)
        {
            return new CircuitBreakerState("someBreaker", "machine", 111)
            {
                MessageSequenceNumber = messageSequenceNumber,
                Status = status
            };
        }

        public static string AddRandomCircuit(CircuitBreakerSet set)
        {
            CircuitBreakerState input = MakeBreakerState(messageSequenceNumber: 15);
            var circuitId = input.CircuitBreakerId;
            set.ProcessCircuitStateMessage(input);
            return circuitId;
        }

        public static string UpdateSpecificCircuit(CircuitBreakerSet set, string circuitId, int sequenceNumber)
        {
            CircuitBreakerState input = MakeBreakerState(messageSequenceNumber: sequenceNumber, id: circuitId);
            set.ProcessCircuitStateMessage(input);
            return circuitId;
        }
    }
}