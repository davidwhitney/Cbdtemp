using System;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Contract;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    [TestFixture]
    class MessageMergingLogicTests
    {
        private readonly MessageMergingLogic _merger = new MessageMergingLogic();

        [Test]
        public void when_circuit_is_in_automatic_mode_an_update_without_state_change_then_the_message_sequence_number_is_adopted()
        {
            var repomsg = Helper.MakeBreakerState(0, CircuitBreakerStatus.Closed);
            var message = Helper.MakeBreakerState(1, CircuitBreakerStatus.Closed);


            var result = _merger.Merge(message, repomsg);

            result.Matches(1, CircuitBreakerStatus.Closed);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(CircuitBreakerStatus.Closed));
        }

        [Test]
        public void when_circuit_is_in_manual_mode_an_update_without_state_change_then_the_message_sequence_number_is_adopted_but_stays_in_manual_mode_but_lastReceived_indicates_value_received()
        {
            var repomsg = Helper.MakeBreakerState(0, CircuitBreakerStatus.ForcedClosed);
            var message = Helper.MakeBreakerState(1, CircuitBreakerStatus.Closed);

            var result = _merger.Merge(message, repomsg);

            result.Matches(1, CircuitBreakerStatus.ForcedClosed);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(CircuitBreakerStatus.Closed));
        }

        [Test]
        public void when_circuit_is_in_manual_mode_an_update_without_state_change_then_the_message_sequence_number_is_adopted_but_stays_in_manual_mode_but_lastReceived_indicates_value_received_converging_on_manual_state()
        {
            var repomsg = Helper.MakeBreakerState(0, CircuitBreakerStatus.ForcedClosed);
            var message = Helper.MakeBreakerState(1, CircuitBreakerStatus.Closed);
            var final = Helper.MakeBreakerState(2, CircuitBreakerStatus.ForcedClosed);            


            var result = _merger.Merge(message, repomsg);
            result = _merger.Merge(final, result);
            
            result.Matches(2, CircuitBreakerStatus.ForcedClosed);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(CircuitBreakerStatus.ForcedClosed));
        }


        [Test]
        public void when_circuit_is_in_automatic_mode_an_update_with_state_change_then_the_message_sequence_number_and_state_are_adopted
        (
         [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed)] CircuitBreakerStatus repoState,
         [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed)] CircuitBreakerStatus msgState)
        {
            var repomsg = Helper.MakeBreakerState(0, repoState);
            var message = Helper.MakeBreakerState(1, msgState);

            var result = _merger.Merge(message, repomsg);

            result.Matches(1, msgState);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(msgState));
        }


        [Test]
        public void when_circuit_is_in_manual_mode_any_incoming_message_is_ignored_and_manual_state_maintained
        (
         [Values(CircuitBreakerStatus.ForcedOpen, CircuitBreakerStatus.ForcedClosed)] CircuitBreakerStatus repoState,
         [Values(CircuitBreakerStatus.Open, CircuitBreakerStatus.HalfOpen, CircuitBreakerStatus.Closed, CircuitBreakerStatus.ForcedOpen, CircuitBreakerStatus.ForcedClosed)] CircuitBreakerStatus msgState)
        {

            var repomsg = Helper.MakeBreakerState(0, repoState);
            var message = Helper.MakeBreakerState(1, msgState);

            var result = _merger.Merge(message, repomsg);

            result.Matches(1, repoState);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(msgState));

        }

        [Test]
        public void when_update_message_and_repo_state_have_same_message_number_then_the_repo_state_is_kept_but_the_messagenumber_is_forwarded_by_1000_and_the_last_seen_is_updated_to_the_state_of_message()
        {
            var repomsg = Helper.MakeBreakerState(1, CircuitBreakerStatus.Closed);
            var message = Helper.MakeBreakerState(1, CircuitBreakerStatus.ForcedOpen);

            var result = _merger.Merge(message, repomsg);

            result.Matches(repomsg.MessageSequenceNumber + 1000,  CircuitBreakerStatus.Closed);
            Assert.That(result.LastReceivedStatus, Is.EqualTo(message.Status));
        }
    }

    static class CircuitBreakerStateExtensionMethods
    {
        public static void Matches(this CircuitBreakerState result, long messageSequenceNumber, CircuitBreakerStatus status)
        {
            Assert.That(result.MessageSequenceNumber, Is.EqualTo(messageSequenceNumber));
            Assert.That(result.Status, Is.EqualTo(status));
            
        }
    }
}
