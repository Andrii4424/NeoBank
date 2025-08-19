using Microsoft.AspNetCore.Mvc;

namespace Bank.Identity.WebUI.Controllers
{
    public class Identity : Controller
    {
        [Route("/")]
        public IActionResult SignIn()
        {
            ViewBag.Title = "Sign In";
            return View();
        }

        [Route("/sign-up")]
        public IActionResult SignUp()
        {
            ViewBag.Title = "Sign Up";
            return View();
        }
    }
}
