using System;
using System.Collections.Generic;
using System.ServiceModel;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Contract;

namespace Ttl.CircuitBreakerManagementService.IntegrationTests
{
    public class CircuitBreakerManagementServiceTestBase
    {
        public const string MessagingServiceEndpointAddress = "http://localhost:8012/";

        private ServiceHost _host;
        protected Impl.CircuitBreakerManagementService DashboardService;
        protected CircuitBreakerManagementServiceClient Client;
      

        protected void SetupInfrastructure()
        {
            Uri baseAddress = new Uri(MessagingServiceEndpointAddress);
            DashboardService = new Impl.CircuitBreakerManagementService();
            _host = new ServiceHost(DashboardService, baseAddress);

            // only needed because adding the meta data enpoint removes the application endpoint.
            _host.AddServiceEndpoint(typeof (ICircuitBreakerManagementService), new BasicHttpBinding(), "");

            _host.Open();
 
            Client = new CircuitBreakerManagementServiceClient(new BasicHttpBinding(), new EndpointAddress(MessagingServiceEndpointAddress));
        }        

        protected void TeardownInfrastructure()
        {
            _host.Close();
        }

        [SetUp]
        public virtual void SetUp() // we need a new service for every test, because the service will remember past messages.
        {
            SetupInfrastructure();
        }

        [TearDown]
        public virtual void Teardown()
        {
            TeardownInfrastructure();
        }

        protected static NotifyOfCircuitStateRequest MakeStatusRequest(long randomSequenceNumber)
        {
            return new NotifyOfCircuitStateRequest
                {
                CircuitBreakerId = "123",
                MachineName = "myMachine",                
                Pid = 1,
                RequestMessageSequenceNumber = randomSequenceNumber,
                Status = CircuitBreakerStatus.Open
            };
        }

    }

    [TestFixture]
    public class ManualOverrideTests : CircuitBreakerManagementServiceTestBase
    {
        [Test]
        public void when_toggling_manual_override_to_off_clients_are_informed_on_the_next_poll()
        {
            var result = Client.NotifyOfCircuitState(MakeStatusRequest(1));

        }
    }
}