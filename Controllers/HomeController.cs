using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Models;
using NeighborhoodServiceFinder.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NeighborhoodServiceFinder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CloudinaryService _cloudinaryService; // Add this

    // Updated constructor
    public HomeController(ILogger<HomeController> logger, CloudinaryService cloudinaryService)
    {
        _logger = logger;
        _cloudinaryService = cloudinaryService; // Add this
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // This action just shows the empty upload form.
    [HttpGet]
    public IActionResult UploadTest()
    {
        return View();
    }

    // This action runs when the form is submitted.
    [HttpPost]
    public async Task<IActionResult> UploadTest(IFormFile file)
    {
        if (file != null)
        {
            // Call our service to upload the file.
            var uploadResult = await _cloudinaryService.UploadProfileImageAsync(file);

            // The result contains the URL of the uploaded image.
            // We pass this URL back to the view to be displayed.
            ViewData["ImageUrl"] = uploadResult.SecureUrl.ToString();
        }

        return View();
    }
}
