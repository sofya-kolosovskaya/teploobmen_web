
using Microsoft.AspNetCore.Mvc;

namespace HeatExchangeApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Перенаправляем на страницу входа или калькулятор
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Calculation");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}