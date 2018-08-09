using System;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Contract;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    [TestFixture]
    class CircuitBreakerSet_SetCircuitState_Tests
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
        public void when_a_circuitbreaker_does_not_exist_then_setting_a_state_returns_null()
        {
            // the rationale behind this logic is that the circuit has been garbage collected because it was offline long enough.
            // therefore a 'set' command from the UI should not create it, but ignore it. A null is returned rather than an exception.

            CircuitBreakerState result = _set.SetCircuitState(Guid.NewGuid().ToString("N"), CircuitBreakerStatus.HalfOpen, SystemTime.Now());

            Assert.That(result, Is.Null);
            Assert.That(_set.Circuits(), Is.Not.Contains("a"));
        }

        [Test]
        public void when_a_circuitbreaker_exists_then_setting_forces_the_state_to_be_that_of_the_message(
            [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed, CircuitBreakerStatus.ForcedClosed, CircuitBreakerStatus.ForcedOpen)]
            CircuitBreakerStatus startingState,

            [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed, CircuitBreakerStatus.ForcedClosed, CircuitBreakerStatus.ForcedOpen)]
            CircuitBreakerStatus desiredState)
        {

            CircuitBreakerState state = Helper.MakeBreakerState(1, startingState);

            CircuitBreakerState originalState = _set.ProcessCircuitStateMessage(state);

            CircuitBreakerState result = _set.SetCircuitState(originalState.CircuitBreakerId, desiredState, SystemTime.Now());

            CircuitBreakerState fetchedState = _set.TryGetCircuit(originalState.CircuitBreakerId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(desiredState));
            Assert.That(result.MessageSequenceNumber, Is.EqualTo(originalState.MessageSequenceNumber)); // important that the sequence number does not change -> otherwise the next inbound messages will be discards by the dashboard.
            

            Assert.That(fetchedState, Is.Not.Null);
            Assert.That(fetchedState.Status, Is.EqualTo(desiredState));
            Assert.That(fetchedState.MessageSequenceNumber, Is.EqualTo(originalState.MessageSequenceNumber));             
            Assert.That(_set.Circuits(), Contains.Item(originalState.CircuitBreakerId));
        }

        //[Test]
        //public void when_a_circuitbreaker_exists_then_setting_a_automatic_state_forces_the_state_to_be_that_of_the_message(
        //    [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed, CircuitBreakerStatus.ForcedClosed, CircuitBreakerStatus.ForcedOpen)]
        //    CircuitBreakerStatus startingState,

        //    [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed, CircuitBreakerStatus.ForcedClosed, CircuitBreakerStatus.ForcedOpen)]
        //    CircuitBreakerStatus desiredState)
        //{
        //    // the rationale here is that on the next update message, the state will flip to whatever it is in the breaker.

        //    CircuitBreakerState state = Helper.MakeBreakerState(1, startingState);

        //    CircuitBreakerState originalState = _set.ProcessCircuitStateMessage(state);            
        //    CircuitBreakerState result = _set.SetCircuitState(originalState.CircuitBreakerId,false,desiredState, SystemTime.Now());

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Status, Is.EqualTo(desiredState));            
        //    Assert.That(result.MessageSequenceNumber, Is.EqualTo(originalState.MessageSequenceNumber)); // important that the sequence number does not change -> otherwise the next inbound messages will be discards by the dashboard.
        //    Assert.That(_set.Circuits(), Contains.Item(originalState.CircuitBreakerId));
        //}
    }
}