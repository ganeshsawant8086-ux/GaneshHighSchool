using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganesh1.Models;

namespace Ganesh1.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentDbContext _context;

        public HomeController(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.OrderByDescending(s => s.ID).ToListAsync();
            var passouts = await _context.Passouts.OrderByDescending(p => p.Year).ToListAsync();
            var teachers = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
            var stocks = await _context.Stocks.OrderBy(s => s.Category).ThenBy(s => s.ItemName).ToListAsync();

            var avgPercentage = passouts.Any() ? Math.Round(passouts.Average(p => p.Percentage), 1) : 0m;
            var totalStockUnits = stocks.Sum(s => s.Quantity);
            var lowStockCount = stocks.Count(s => s.Quantity <= s.MinThreshold);
            var totalStockValue = stocks.Sum(s => s.Quantity * s.UnitPrice);

            var model = new DashboardViewModel
            {
                Students = students,
                Passouts = passouts,
                Teachers = teachers,
                Stocks = stocks,
                TotalStudents = students.Count,
                TotalTeachers = teachers.Count,
                TotalPassouts = passouts.Count,
                AveragePassPercentage = avgPercentage,
                ActiveClasses = 10,
                TotalStockItems = stocks.Count,
                TotalStockUnits = totalStockUnits,
                LowStockAlerts = lowStockCount,
                TotalInventoryValue = totalStockValue
            };

            return View(model);
        }

        // Quick AJAX endpoints for instant reactive interactions
        [HttpPost]
        public async Task<IActionResult> QuickAddStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please fill in all required student details." });
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Student registered successfully!", data = student });
        }

        [HttpPost]
        public async Task<IActionResult> QuickAddPassout([FromBody] Passout passout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please fill in all required passout details." });
            }

            _context.Passouts.Add(passout);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Passout record added successfully!", data = passout });
        }

        [HttpPost]
        public async Task<IActionResult> QuickAddTeacher([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please fill in all required teacher details." });
            }

            teacher.JoiningDate = DateTime.Today;
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Teacher registered successfully!", data = teacher });
        }

        [HttpPost]
        public async Task<IActionResult> QuickDeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound(new { success = false, message = "Student not found." });

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Student removed successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> QuickDeletePassout(int id)
        {
            var passout = await _context.Passouts.FindAsync(id);
            if (passout == null) return NotFound(new { success = false, message = "Passout record not found." });

            _context.Passouts.Remove(passout);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Passout record removed successfully." });
        }

        // ==========================================
        // SCHOOL CLERK STOCK & INVENTORY ENDPOINTS
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> QuickAddStock([FromBody] Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please fill in all required stock item details." });
            }

            if (stock.Quantity <= 0)
            {
                stock.Status = "Out of Stock";
            }
            else if (stock.Quantity <= stock.MinThreshold)
            {
                stock.Status = "Low Stock";
            }
            else
            {
                stock.Status = "In Stock";
            }

            stock.LastRestockedDate = DateTime.Now;
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "School material added to stock successfully!", data = stock });
        }

        [HttpPost]
        public async Task<IActionResult> QuickAdjustStock(int id, int change)
        {
            var item = await _context.Stocks.FindAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Material item not found in stock." });

            item.Quantity = Math.Max(0, item.Quantity + change);

            if (item.Quantity <= 0)
            {
                item.Status = "Out of Stock";
            }
            else if (item.Quantity <= item.MinThreshold)
            {
                item.Status = "Low Stock";
            }
            else
            {
                item.Status = "In Stock";
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = $"Stock updated: {item.ItemName} is now {item.Quantity} {item.Unit}.",
                newQuantity = item.Quantity,
                newStatus = item.Status,
                totalValue = item.Quantity * item.UnitPrice
            });
        }

        [HttpPost]
        public async Task<IActionResult> QuickRestockItem(int id, int addQuantity)
        {
            if (addQuantity <= 0) return BadRequest(new { success = false, message = "Please provide a valid restock quantity." });

            var item = await _context.Stocks.FindAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Material item not found in stock." });

            item.Quantity += addQuantity;
            item.LastRestockedDate = DateTime.Now;

            if (item.Quantity <= item.MinThreshold)
            {
                item.Status = "Low Stock";
            }
            else
            {
                item.Status = "In Stock";
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = $"Restocked {addQuantity} {item.Unit} for {item.ItemName}.",
                newQuantity = item.Quantity,
                newStatus = item.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> QuickDeleteStock(int id)
        {
            var item = await _context.Stocks.FindAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Material item not found in stock." });

            _context.Stocks.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = $"Stock item '{item.ItemName}' deleted successfully." });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
