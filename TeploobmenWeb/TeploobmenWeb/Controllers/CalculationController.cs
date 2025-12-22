using Microsoft.AspNetCore.Mvc;
using HeatExchangeApp.Models;
using HeatExchangeApp.Services;
using HeatExchangeApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HeatExchangeApp.Controllers
{
    public class CalculationController : Controller
    {
        private readonly CalculationService _calcService;
        private readonly AppDbContext _context;
        private readonly ILogger<CalculationController> _logger;

        public CalculationController(CalculationService calcService, AppDbContext context, ILogger<CalculationController> logger = null)
        {
            _calcService = calcService;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new CalculationInput());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(CalculationInput input, string calcName = "Новый расчет")
        {
            _logger?.LogInformation("=== НАЧАЛО АСИНХРОННОГО РАСЧЕТА ===");

            // Если модель null, берем из формы
            if (input == null || input.H0 == 0)
            {
                _logger?.LogDebug("Берем данные из формы...");
                input = new CalculationInput
                {
                    H0 = await GetDoubleAsync("H0", 5.0),
                    TPrime = await GetDoubleAsync("TPrime", 700),
                    TDoublePrime = await GetDoubleAsync("TDoublePrime", 20),
                    Wg = await GetDoubleAsync("Wg", 0.78),
                    Cg = await GetDoubleAsync("Cg", 1.32),
                    Gm = await GetDoubleAsync("Gm", 1.72),
                    Cm = await GetDoubleAsync("Cm", 1.49),
                    Av = await GetDoubleAsync("Av", 2460),
                    D = await GetDoubleAsync("D", 2.2),
                    PointsCount = await GetIntAsync("PointsCount", 11)
                };

                if (string.IsNullOrEmpty(calcName))
                {
                    calcName = Request.Form["calcName"].ToString();
                    if (string.IsNullOrEmpty(calcName))
                        calcName = "Новый расчет";
                }
            }

            _logger?.LogInformation("Расчет: {CalcName}, H0={H0}", calcName, input.H0);

            var calculationTask = Task.Run(() => _calcService.Calculate(input, calcName));
            var result = await calculationTask;
            result.Input = input;

            try
            {
                var username = HttpContext.Session?.GetString("Username") ?? "Гость";
                var history = _calcService.CreateHistory(input, result, username);

                await _context.CalculationHistories.AddAsync(history);
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Сохранено в историю с ID: {HistoryId}", history.Id);
                ViewBag.SavedId = history.Id;
                ViewBag.Message = $"Расчёт успешно сохранён (ID: {history.Id})";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка сохранения расчета");
                ViewBag.Error = $"Ошибка сохранения: {ex.Message}";
            }


            return View("Result", result);
        }

 
        public async Task<IActionResult> Test()
        {
            var testInput = new CalculationInput();
            var result = await Task.Run(() => _calcService.Calculate(testInput, "Тестовый расчет"));
            return View("Result", result);
        }

 
        public async Task<IActionResult> ViewResult(int id)
        {
            var history = await _context.CalculationHistories
                .AsNoTracking() // Для только чтения - оптимизация
                .FirstOrDefaultAsync(h => h.Id == id);

            if (history == null)
            {
                return NotFound();
            }

            var result = _calcService.LoadFromHistory(history);
            return View("Result", result);
        }

        public async Task<IActionResult> History(int page = 1, int pageSize = 10)
        {
            var username = HttpContext.Session?.GetString("Username") ?? "Гость";

            var totalCount = await _context.CalculationHistories
                .Where(h => h.Username == username)
                .CountAsync();

            var history = await _context.CalculationHistories
                .Where(h => h.Username == username)
                .OrderByDescending(h => h.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(history);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var history = await _context.CalculationHistories.FindAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            var username = HttpContext.Session?.GetString("Username");
            if (history.Username != username)
            {
                return Forbid();
            }

            _context.CalculationHistories.Remove(history);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Расчёт успешно удалён";
            return RedirectToAction(nameof(History));
        }


        private async Task<double> GetDoubleAsync(string name, double defaultValue)
        {
            if (Request.HasFormContentType && Request.Form.TryGetValue(name, out var value))
            {
                if (double.TryParse(value.ToString().Replace(',', '.'), out var result))
                    return result;
            }
            return await Task.FromResult(defaultValue);
        }

        private async Task<int> GetIntAsync(string name, int defaultValue)
        {
            if (Request.HasFormContentType && Request.Form.TryGetValue(name, out var value))
            {
                if (int.TryParse(value.ToString(), out var result))
                    return result;
            }
            return await Task.FromResult(defaultValue);
        }
    }
}