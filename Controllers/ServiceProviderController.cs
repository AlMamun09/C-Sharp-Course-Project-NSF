using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeighborhoodServiceFinder.Controllers
{
    [Authorize(Roles = "ServiceProvider, Admin")] // Only Service Providers (and Admins) can access this
    public class ServiceProviderController : Controller
    {
        // This action will show the main provider dashboard page.
        public IActionResult Index()
        {
            return View();
        }
    }
}