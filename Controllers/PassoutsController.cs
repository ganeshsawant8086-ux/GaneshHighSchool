using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganesh1.Models;

namespace Ganesh1.Controllers
{
    public class PassoutsController : Controller
    {
        private readonly StudentDbContext _context;

        public PassoutsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: /Passouts
        public async Task<IActionResult> Index()
        {
            var list = await _context.Passouts.OrderByDescending(p => p.Year).ToListAsync();
            return View(list);
        }

        // GET: /Passouts/Create
        public IActionResult Create() => View(new Passout());

        // POST: /Passouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Passout p)
        {
            if (!ModelState.IsValid) return View(p);
            _context.Passouts.Add(p);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Passout record for {p.FullName} ({p.Year}) added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Passouts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Passouts.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Passouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Passout model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Passout record updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Passouts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Passouts.FindAsync(id);
            if (item != null)
            {
                _context.Passouts.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Passout record removed successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
