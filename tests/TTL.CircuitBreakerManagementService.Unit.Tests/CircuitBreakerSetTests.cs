using System.Collections.Generic;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    [TestFixture]
    internal class CircuitBreakerSetTests
    {
        private CircuitBreakerSet _set;

        [SetUp]
        public void Setup()
        {
            _set = new CircuitBreakerSet();
        }

        [TearDown]
        public void Teardown()
        {
            SystemTime.IsNow();
        }

        [Test]
        public void adding_an_unseen_circuit_shows_up_in_the_enumeration_of_circuits()
        {
            var circuitId = Helper.AddRandomCircuit(_set);

            ICollection<string> initialSet = _set.Circuits();
            Assert.That(initialSet, Contains.Item(circuitId));
            Assert.That(initialSet.Count, Is.EqualTo(1));

            var circuitId2 = Helper.AddRandomCircuit(_set);
            ICollection<string> updatedSet = _set.Circuits();
            Assert.That(updatedSet, Contains.Item(circuitId));
            Assert.That(updatedSet, Contains.Item(circuitId2));
            Assert.That(updatedSet.Count, Is.EqualTo(2));
        }

        [Test]
        public void can_concurrently_add_circuits()
        {
            for (int i = 0; i < 10000; i++)
            {
                Helper.AddRandomCircuit(_set);
            }
        }

        [Test]
        public void enumeration_of_circuits_starts_off_empty()
        {
            ICollection<string> circuitIds = _set.Circuits();
            Assert.That(circuitIds.Count, Is.EqualTo(0));
        }

        [Test]
        public void adding_an_unseen_circuit_returns_a_passthrough_message_stamped_with_current_time()
        {
            SystemTime.Is(millisecond: 100);
            CircuitBreakerState input = Helper.MakeBreakerState(messageSequenceNumber: 15);

            SystemTime.Is(millisecond: 200);
            CircuitBreakerState result = _set.ProcessCircuitStateMessage(input);


            Assert.That(result.CircuitBreakerId, Is.EqualTo(input.CircuitBreakerId), "CircuitBreakerId");
            Assert.That(result.MessageSequenceNumber, Is.EqualTo(input.MessageSequenceNumber), "RequestMessageSequenceNumber");
            Assert.That(result.ProcessId, Is.EqualTo(input.ProcessId), "ProcessID");
            Assert.That(result.Status, Is.EqualTo(input.Status), "Status");
            Assert.That(result.MachineName, Is.EqualTo(input.MachineName), "MachineName");
            Assert.That(result.LastModifiedDate, Is.EqualTo(input.LastModifiedDate.AddMilliseconds(100)), "LastModifiedDate");            
            Assert.That(result.LastModifiedDate, Is.EqualTo(SystemTime.Now()), "LastModifiedDate should be now");
        }

        [Test]
        public void an_older_message_for_existing_breaker_is_ignored_by_update_and_returns_the_more_recent_state()
        {
            SystemTime.Is(second: 1, millisecond: 100);

            CircuitBreakerState original = Helper.MakeBreakerState("a", messageSequenceNumber: 15);
            CircuitBreakerState originalResult = _set.ProcessCircuitStateMessage(original);
            CircuitBreakerState ignoredResult =
                _set.ProcessCircuitStateMessage(Helper.MakeBreakerState("a", messageSequenceNumber: 14));


            Assert.That(ignoredResult.CircuitBreakerId, Is.EqualTo(originalResult.CircuitBreakerId), "CircuitBreakerId");
            Assert.That(ignoredResult.MessageSequenceNumber, Is.EqualTo(originalResult.MessageSequenceNumber), "RequestMessageSequenceNumber");
            Assert.That(ignoredResult.ProcessId, Is.EqualTo(originalResult.ProcessId), "ProcessID");
            Assert.That(ignoredResult.Status, Is.EqualTo(originalResult.Status), "Status");
            Assert.That(ignoredResult.MachineName, Is.EqualTo(originalResult.MachineName), "MachineName");            
            Assert.That(ignoredResult.LastModifiedDate, Is.EqualTo(SystemTime.Now()), "LastModifiedDate should be now");
        }

        [Test]
        public void a_newer_message_number_for_existing_breaker_is_processed_for_update()
        {
            SystemTime.Is(second: 1, millisecond: 100);

            CircuitBreakerState original = Helper.MakeBreakerState("a", messageSequenceNumber: 15);
            CircuitBreakerState originalResult = _set.ProcessCircuitStateMessage(original);

            SystemTime.Is(second: 2, millisecond: 200);
            CircuitBreakerState newResult =
                _set.ProcessCircuitStateMessage(Helper.MakeBreakerState("a", messageSequenceNumber: 16));


            Assert.That(newResult.CircuitBreakerId, Is.EqualTo(originalResult.CircuitBreakerId), "CircuitBreakerId");
            Assert.That(newResult.MessageSequenceNumber, Is.EqualTo(16), "RequestMessageSequenceNumber");
            Assert.That(newResult.ProcessId, Is.EqualTo(originalResult.ProcessId), "ProcessID");
            Assert.That(newResult.Status, Is.EqualTo(originalResult.Status), "Status");
            Assert.That(newResult.MachineName, Is.EqualTo(originalResult.MachineName), "MachineName");            
            Assert.That(newResult.LastModifiedDate, Is.EqualTo(SystemTime.Now()), "LastModifiedDate should be now");
        }
    }
}