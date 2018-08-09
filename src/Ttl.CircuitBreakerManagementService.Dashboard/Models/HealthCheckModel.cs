using System.Reflection;
using Newtonsoft.Json;

namespace Ttl.CircuitBreakerManagementService.Dashboard.Models
{
    public class HealthCheckModel
    {
        public HealthCheckModel() : this(Assembly.GetExecutingAssembly().GetName().Version.ToString())
        {
        }

        public HealthCheckModel(string applicationSemVer)
        {
            ServiceName = "Circuit Breaker";
            ApplicationId = "40388";
            Version = applicationSemVer;
        }

        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; private set; }

        [JsonProperty(PropertyName = "applicationId")]
        public string ApplicationId { get; private set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; private set; }
    }
}