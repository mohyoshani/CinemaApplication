using CinemaApplication.Utility;
using CinemaApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaApplication.Areas.Customer.Controllers
{
    [Area(nameof(SD.Customer))]
    public class HomeController : Controller
    {
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
    }
}
