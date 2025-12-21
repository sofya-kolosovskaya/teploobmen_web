using Microsoft.AspNetCore.Mvc;
using HeatExchangeApp.Models;
using HeatExchangeApp.Services;
using HeatExchangeApp.Data;

namespace HeatExchangeApp.Controllers
{
    public class CalculationController : Controller
    {
        private readonly CalculationService _calcService;
        private readonly AppDbContext _context;

        public CalculationController(CalculationService calcService, AppDbContext context)
        {
            _calcService = calcService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new CalculationInput());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(CalculationInput input, string calcName = "Новый расчет")
        {
            Console.WriteLine("=== НАЧАЛО РАСЧЕТА ===");

            // Если модель null, берем из формы
            if (input == null || input.H0 == 0)
            {
                Console.WriteLine("Берем данные из формы...");
                input = new CalculationInput
                {
                    H0 = GetDouble("H0", 5.0),
                    TPrime = GetDouble("TPrime", 700),
                    TDoublePrime = GetDouble("TDoublePrime", 20),
                    Wg = GetDouble("Wg", 0.78),
                    Cg = GetDouble("Cg", 1.32),
                    Gm = GetDouble("Gm", 1.72),
                    Cm = GetDouble("Cm", 1.49),
                    Av = GetDouble("Av", 2460),
                    D = GetDouble("D", 2.2),
                    PointsCount = GetInt("PointsCount", 11)
                };

                if (string.IsNullOrEmpty(calcName))
                {
                    calcName = Request.Form["calcName"].ToString();
                    if (string.IsNullOrEmpty(calcName))
                        calcName = "Новый расчет";
                }
            }

            Console.WriteLine($"Расчет: {calcName}, H0={input.H0}");

            // 1. Выполняем расчет
            var result = _calcService.Calculate(input, calcName);
            result.Input = input;

            // 2. Сохраняем в историю
            try
            {
                var username = HttpContext.Session?.GetString("Username") ?? "Гость";
                var history = _calcService.CreateHistory(input, result, username);

                _context.CalculationHistories.Add(history);
                _context.SaveChanges();

                Console.WriteLine($"✅ Сохранено в историю с ID: {history.Id}");
                ViewBag.SavedId = history.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сохранения: {ex.Message}");
            }

            // 3. Показываем результат
            return View("Result", result);
        }

        // Тестовый метод
        public IActionResult Test()
        {
            var testInput = new CalculationInput();
            var result = _calcService.Calculate(testInput, "Тестовый расчет");
            return View("Result", result);
        }

        // Просмотр из истории
        public IActionResult ViewResult(int id)
        {
            var history = _context.CalculationHistories.Find(id);
            if (history == null)
            {
                return NotFound();
            }

            var result = _calcService.LoadFromHistory(history);
            return View("Result", result);
        }

        // Вспомогательные методы
        private double GetDouble(string name, double defaultValue)
        {
            if (Request.Form.TryGetValue(name, out var value))
            {
                if (double.TryParse(value.ToString().Replace(',', '.'), out var result))
                    return result;
            }
            return defaultValue;
        }

        private int GetInt(string name, int defaultValue)
        {
            if (Request.Form.TryGetValue(name, out var value))
            {
                if (int.TryParse(value.ToString(), out var result))
                    return result;
            }
            return defaultValue;
        }
    }
}