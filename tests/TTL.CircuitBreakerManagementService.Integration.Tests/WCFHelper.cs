using System.ServiceModel;
using System.ServiceModel.Description;

namespace Ttl.CircuitBreakerManagementService.IntegrationTests
{
    public class WCFHelper
    {
        public static void AddMetaDataEndpoint(ServiceHost host)
        {
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (smb == null) smb = new ServiceMetadataBehavior();

            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);

            // Add MEX endpoint
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
        }
    }
}