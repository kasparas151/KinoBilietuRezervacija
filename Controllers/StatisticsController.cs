using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KinoBilietuRezervacija.Data;
using KinoBilietuRezervacija.Models;

namespace KinoBilietuRezervacija.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly KinoDbContext _context;

        public StatisticsController(KinoDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bilietuKiekis = await _context.Tickets
                .Include(t => t.Seansas)
                .ThenInclude(s => s.Filmas)
                .GroupBy(t => t.Seansas.Filmas.Pavadinimas)
                .Select(g => new
                {
                    Filmas = g.Key,
                    BilietuSkaicius = g.Count(),
                    Pajamos = g.Sum(t => t.Seansas.Kaina)
                }).ToListAsync();

            ViewData["Statistika"] = bilietuKiekis;
            return View();
        }
        [HttpGet("Statistics/Filter")]
        public async Task<IActionResult> Index(DateTime? nuo, DateTime? iki)
        {
            var seansai = _context.Tickets
                .Include(t => t.Seansas)
                .ThenInclude(s => s.Filmas)
                .Include(t => t.User)
                .AsQueryable();

            // Filtravimas pagal datą
            if (nuo.HasValue)
                seansai = seansai.Where(t => t.Seansas.DataLaikas >= nuo.Value);

            if (iki.HasValue)
                seansai = seansai.Where(t => t.Seansas.DataLaikas <= iki.Value);


            var bilietuKiekis = await seansai
                .GroupBy(t => t.Seansas.Filmas.Pavadinimas)
                .Select(g => new
                {
                    Filmas = g.Key,
                    BilietuSkaicius = g.Count(),
                    Pajamos = g.Sum(t => t.Seansas.Kaina)
                }).ToListAsync();

            var visoBilietu = await seansai.CountAsync();
            var seansuSkaicius = await seansai.Select(t => t.Seansas.ID).Distinct().CountAsync();
            var vidurkis = seansuSkaicius > 0 ? (double)visoBilietu / seansuSkaicius : 0;

            var topKlientas = await seansai
                .GroupBy(t => t.User.Email)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Vartotojas = g.Key, Kiekis = g.Count() })
                .FirstOrDefaultAsync();

            ViewData["Statistika"] = bilietuKiekis;
            ViewData["Vidurkis"] = vidurkis;
            ViewData["TopKlientas"] = topKlientas;
            ViewData["Nuo"] = nuo?.ToString("yyyy-MM-dd");
            ViewData["Iki"] = iki?.ToString("yyyy-MM-dd");

            return View();
        }

    }
}
