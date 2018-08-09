using System;
using NUnit.Framework;

namespace Ttl.CircuitBreakerManagementService.IntegrationTests
{
    [TestFixture]
    public class CircuitBreakerManagementServiceTest : CircuitBreakerManagementServiceTestBase
    {
        [Test]
        public void Calling_the_service_returns_the_sequence_number_passed_in()
        {
            // using the synchronous, rather than the async client is okay. Async behaviour is purely a client side matter, these tests are about testing the service itself and it makes for easier code.

            int randomSequenceNumber = new Random().Next(5, 40000);
            var result = Client.NotifyOfCircuitState(MakeStatusRequest(randomSequenceNumber));

            Assert.That(result.MessageSequenceNumber, Is.EqualTo(randomSequenceNumber));
        }

        [Test]
        public void Service_is_running_as_a_singleton_because_it_recognizes_and_ignores_old_messages()
        {
            var result = Client.NotifyOfCircuitState(MakeStatusRequest(10));
            Assert.That(result.MessageSequenceNumber, Is.EqualTo(10));

            result = Client.NotifyOfCircuitState(MakeStatusRequest(2));
            Assert.That(result.MessageSequenceNumber, Is.EqualTo(10));
        }

        [Test]
        public void Service_will_clear_down_expired_circuitbreakers()
        {

        }              
    }
}