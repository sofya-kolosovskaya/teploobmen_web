// Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using HeatExchangeApp.Models;
using HeatExchangeApp.Data;
using System.Linq;
using System;

namespace HeatExchangeApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Простая проверка (в реальном проекте нужна проверка пароля)
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                // В демо-версии просто проверяем существование пользователя
                // В реальном проекте: проверка хеша пароля
                if (user.PasswordHash == HashPassword(password) || password == "demo123") // demo пароль для теста
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id);

                    return RedirectToAction("Index", "Calculation");
                }
            }

            // Если пользователя нет, создаем нового (для демо)
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Error = "Введите имя пользователя";
                return View();
            }

            var newUser = new User
            {
                Username = username,
                Email = $"{username}@example.com",
                PasswordHash = HashPassword(password ?? "demo123")
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetString("Username", newUser.Username);
            HttpContext.Session.SetInt32("UserId", newUser.Id);

            return RedirectToAction("Index", "Calculation");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View();
            }

            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Пользователь с таким именем уже существует";
                return View();
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Calculation");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Простое хеширование (для демо)
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}