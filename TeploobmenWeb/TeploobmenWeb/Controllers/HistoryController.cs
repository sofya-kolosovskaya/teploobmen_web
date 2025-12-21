using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HeatExchangeApp.Data;
using HeatExchangeApp.Services;

namespace HeatExchangeApp.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CalculationService _calcService;

        public HistoryController(AppDbContext context, CalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        public IActionResult Index()
        {
            var history = _context.CalculationHistories
                .OrderByDescending(h => h.CreatedAt)
                .Take(50)
                .ToList();

            return View(history);
        }

        public IActionResult Details(int id)
        {
            var history = _context.CalculationHistories.Find(id);
            if (history == null)
            {
                return NotFound();
            }

            var result = _calcService.LoadFromHistory(history);
            return View("~/Views/Calculation/Result.cshtml", result);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _context.CalculationHistories.Find(id);
            if (item != null)
            {
                _context.CalculationHistories.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Очистка всей истории
        [HttpPost]
        public IActionResult ClearAll()
        {
            var allItems = _context.CalculationHistories.ToList();
            _context.CalculationHistories.RemoveRange(allItems);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}