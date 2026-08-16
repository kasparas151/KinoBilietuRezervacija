using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using KinoBilietuRezervacija.Data;
using KinoBilietuRezervacija.Models;

namespace KinoBilietuRezervacija.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly KinoDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(KinoDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var ticketsQuery = _context.Tickets
                .Include(t => t.Seansas)
                    .ThenInclude(s => s.Filmas)
                .AsQueryable();

            if (!isAdmin)
            {
                ticketsQuery = ticketsQuery.Where(t => t.UserId == userId);
            }

            var tickets = await ticketsQuery.ToListAsync();
            return View(tickets);
        }

        // GET: Ticket/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Seansas)
                .ThenInclude(s => s.Filmas)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // GET: Ticket/Create
        [Authorize]
        public IActionResult Create(int? screeningId)
        {
            ViewBag.SeansoID = new SelectList(
                _context.Screening.Include(s => s.Filmas)
                    .Select(s => new
                    {
                        ID = s.ID,
                        Display = s.Filmas.Pavadinimas + " - " + s.DataLaikas.ToString("yyyy-MM-dd HH:mm")
                    }),
                "ID", "Display", screeningId);

            return View();
        }



        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,SeansoID,VietosNumeris")] Ticket ticket)
        {
            bool isSeatTaken = await _context.Tickets
                .AnyAsync(t => t.SeansoID == ticket.SeansoID && t.VietosNumeris == ticket.VietosNumeris);

            if (isSeatTaken)
            {
                ModelState.AddModelError("", "Ši vieta jau užimta.");
                ViewBag.SeansoID = new SelectList(
                    _context.Screening.Include(s => s.Filmas)
                        .Select(s => new {
                            ID = s.ID,
                            Display = s.Filmas.Pavadinimas + " - " + s.DataLaikas.ToString("yyyy-MM-dd HH:mm")
                        }), "ID", "Display", ticket.SeansoID);

                return View(ticket);
            }

            ticket.UserId = _userManager.GetUserId(User);
            ticket.PirkejoVardas = User.Identity.Name ?? "Nenurodytas";
            ticket.MokejimoBusena = "Neapmokėta";

            _context.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Ticket/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewData["SeansoID"] = new SelectList(_context.Screening.Include(s => s.Filmas)
                .Select(s => new {
                    ID = s.ID,
                    Display = s.Filmas.Pavadinimas + " - " + s.DataLaikas.ToString("yyyy-MM-dd HH:mm")
                }), "ID", "Display", ticket.SeansoID);

            return View(ticket);
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["SeansoID"] = new SelectList(_context.Screening, "ID", "ID", ticket.SeansoID);
            return View(ticket);
        }

        // GET: Ticket/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Seansas)
                .ThenInclude(s => s.Filmas)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.ID == id);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Pay(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.MokejimoBusena = "Apmokėta";
            _context.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
