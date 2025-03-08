using Microsoft.AspNetCore.Mvc;

namespace Scraper.Controllers
{
    public class ScrapperController : Controller
    {
        public IActionResult Scrap()
        {
            return View();
        }
    }
}
