using Microsoft.AspNetCore.Mvc;

namespace Bank.Identity.WebUI.Controllers
{
    public class Identity : Controller
    {
        [Route("/")]
        public IActionResult Login()
        {
            ViewBag.Title = "Login";
            return View();
        }
    }
}
