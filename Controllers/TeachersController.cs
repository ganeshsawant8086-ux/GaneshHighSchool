using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganesh1.Models;

namespace Ganesh1.Controllers
{
    public class TeachersController : Controller
    {
        private readonly StudentDbContext _context;

        public TeachersController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: /Teachers
        public async Task<IActionResult> Index()
        {
            var list = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
            return View(list);
        }

        // GET: /Teachers/Create
        public IActionResult Create()
        {
            return View(new Teacher { JoiningDate = DateTime.Today });
        }

        // POST: /Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Faculty member {teacher.FullName} registered successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: /Teachers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Teachers.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Teacher record updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
            }
            return View(teacher);
        }

        // POST: /Teachers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Teachers.FindAsync(id);
            if (item != null)
            {
                _context.Teachers.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Teacher record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
