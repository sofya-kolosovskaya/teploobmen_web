
using Microsoft.AspNetCore.Mvc;

namespace HeatExchangeApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.Error = "Введите имя пользователя";
                return View();
            }

            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index", "Calculation");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login");
        }
    }
}