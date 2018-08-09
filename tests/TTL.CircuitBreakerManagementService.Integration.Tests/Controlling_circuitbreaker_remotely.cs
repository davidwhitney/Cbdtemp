//using NUnit.Framework;
//using TTL.CircuitBreakerLibrary;
//using Ttl.CircuitBreakerManagementService.Contract;
//using Ttl.CircuitBreakerManagementService.IntegrationTests.TestClientApp;

//namespace Ttl.CircuitBreakerManagementService.IntegrationTests
//{
//    [TestFixture]
//    class Controlling_circuitbreaker_remotely : CircuitBreakerManagementServiceTestBase
//    {
//        [Test]
//        public void By_default_circuit_is_closed()
//        {
//            var t = new TestApp();
//            t.Setup(MessagingServiceEndpointAddress);
//            var result = t.StringDoubler("a");
//            Assert.That(result, Is.EqualTo("aa"));
//        }

//        [Test]
//        public void Can_ForceCircuitOpen()
//        {
//            var t = TestSingleMessage(new CircuitBreakerSetStateRequest
//                {                    
//                    Status = CircuitBreakerStatus.ForcedOpen                    
//                });

//            Assert.Throws<CircuitBrokenException>(() => t.StringDoubler("a"));
//        }

//        [Test]
//        public void Can_ForceCloseCircuit()
//        {
//            var t = TestSingleMessage(new CircuitBreakerSetStateRequest
//            {
//                Status = CircuitBreakerStatus.ForcedClosed                
//            });

//            var result = "";
//            Assert.DoesNotThrow(() => result = t.StringDoubler("a"));
//            Assert.That(result, Is.EqualTo("aa"));
//        }

//        private TestApp TestSingleMessage(CircuitBreakerSetStateRequest notifyOfCircuitStateRequest)
//        {
//            var t = new TestApp();
//            t.Setup(MessagingServiceEndpointAddress);

//            string circuitBreakerId = t.CircuitID;

//            notifyOfCircuitStateRequest.CircuitBreakerId = circuitBreakerId;
            
//            DashboardService.SetCircuitState(notifyOfCircuitStateRequest);

//            t.MonitoredBreaker.Monitor.StartNotification();
//            t.CallbackWaitHandle.WaitOne(5000); // wait for the first notification.

//            return t;
//        }
//    }
//}
