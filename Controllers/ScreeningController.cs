using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KinoBilietuRezervacija.Data;
using KinoBilietuRezervacija.Models;
using Microsoft.AspNetCore.Authorization;

namespace KinoBilietuRezervacija.Controllers
{
    public class ScreeningController : Controller
    {
        private readonly KinoDbContext _context;

        public ScreeningController(KinoDbContext context)
        {
            _context = context;
        }

        // GET: Screening
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var screenings = _context.Screening.Include(s => s.Filmas);
            return View(await screenings.ToListAsync());
        }

        // GET: Screening/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FilmoID"] = new SelectList(_context.Movies, "ID", "Pavadinimas");
            return View();
        }

        // POST: Screening/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Screening screening)
        {
            if (ModelState.IsValid)
            {
                _context.Add(screening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState klaidos:");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"❗ {entry.Key}: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["FilmoID"] = new SelectList(_context.Movies, "ID", "Pavadinimas", screening.FilmoID);
            return View(screening);
        }

        // GET: Screening/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var screening = await _context.Screening.FindAsync(id);
            if (screening == null) return NotFound();

            ViewData["FilmoID"] = new SelectList(_context.Movies, "ID", "Pavadinimas", screening.FilmoID);
            return View(screening);
        }

        // POST: Screening/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Screening screening)
        {
            if (id != screening.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(screening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmoID"] = new SelectList(_context.Movies    , "ID", "Pavadinimas", screening.FilmoID);
            return View(screening);
        }

        // GET: Screening/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var screening = await _context.Screening
                .Include(s => s.Filmas)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (screening == null) return NotFound();

            return View(screening);
        }

        // POST: Screening/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var screening = await _context.Screening.FindAsync(id);
            if (screening != null)
            {
                _context.Screening.Remove(screening);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Screening/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var screening = await _context.Screening
                .Include(s => s.Filmas)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (screening == null) return NotFound();

            return View(screening);
        }
    }
}
