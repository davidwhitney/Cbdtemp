using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ttl.CircuitBreakerManagementService.Dashboard.Models;

namespace Ttl.CircuitBreakerManagementService.Dashboard.Controllers
{
    public class DiagnosticsController : Controller
    {
        [Route("diagnostics/healthcheck")]
        [Route("diagnostics/installationcheck")]
        public ContentResult HealthCheck()
        {
            var model = new HealthCheckModel();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(model),
                ContentType = "application/json"
            };
        }
    }
}