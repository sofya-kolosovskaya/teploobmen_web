// Services/AuthService.cs
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using HeatExchangeApp.Data;
using HeatExchangeApp.Models;

namespace HeatExchangeApp.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Хеширование пароля с солью
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            // Добавляем "соль" для безопасности
            var saltedPassword = password + "|" + DateTime.Now.Ticks.ToString();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedInput = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hashedInput == storedHash;
        }

        // Регистрация пользователя
        public bool Register(User user, string password)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return false;

            if (_context.Users.Any(u => u.Email == user.Email))
                return false;

            user.PasswordHash = HashPassword(password);
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        // Авторизация
        public User Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            if (VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }

        // Получить пользователя по ID
        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }
    }
}