using System.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Ttl.CircuitBreakerManagementService.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel
                {
                    IsAdminRole = false
                };
            return View(viewModel);
        }
    }
}
