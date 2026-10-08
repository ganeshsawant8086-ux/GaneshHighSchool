using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganesh1.Models;

namespace Ganesh1.Controllers
{
    public class TeacherPaymentsController : Controller
    {
        private readonly StudentDbContext _context;

        public TeacherPaymentsController(StudentDbContext context)
        {
            _context = context;
        }

        // ======================================================================
        // GET: /TeacherPayments (Master Dashboard & Interactive Inspector)
        // ======================================================================
        public async Task<IActionResult> Index(int? selectedId)
        {
            try
            {
                var teachers = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
                if (!teachers.Any())
                {
                    return View(new TeacherPaymentsIndexViewModel());
                }

                var allPayments = await _context.TeacherPayments.ToListAsync();

                var items = teachers.Select(t =>
                {
                    var history = allPayments
                        .Where(p => p.TeacherId == t.Id)
                        .OrderByDescending(p => p.Year)
                        .ThenByDescending(p => p.Month)
                        .ThenByDescending(p => p.PaymentDate)
                        .ToList();

                    var latest = history.FirstOrDefault();

                    // If teacher has no payment record yet, create an informative draft representation
                    int workDays = latest?.TotalWorkingDays ?? 26;
                    int worked = latest?.DaysPresent ?? 25;
                    int absent = latest?.DaysAbsent ?? 1;
                    int hol = latest?.Holidays ?? 0;
                    decimal daily = latest?.DailySalary ?? (t.YearsOfExperience >= 10 ? 1600m : t.YearsOfExperience >= 5 ? 1400m : 1200m);
                    decimal ded = latest?.Deductions ?? 1500m;
                    decimal gross = worked * daily;
                    decimal finalSal = latest?.LastPaymentAmount ?? Math.Max(0, gross - ded);
                    double attPct = workDays > 0 ? Math.Round(((double)worked / workDays) * 100.0, 1) : 100.0;

                    return new TeacherPaymentItem
                    {
                        TeacherId = t.Id,
                        FullName = t.FullName,
                        Subject = t.SubjectExpertise,
                        Qualification = t.Qualification,
                        ExperienceYears = t.YearsOfExperience,
                        Email = t.Email,
                        Phone = t.Phone,
                        JoiningDate = t.JoiningDate,
                        ProfessionalDescription = t.ProfessionalDescription,

                        // Core Metrics requested by User
                        TotalWorkingDays = workDays,
                        TotalDaysWorked = worked,
                        Absences = absent,
                        Holidays = hol,
                        DailySalary = daily,
                        GrossSalary = gross,
                        SalaryDeductions = ded,
                        FinalSalary = finalSal,
                        AttendancePercentage = attPct,

                        LastPaymentDate = latest?.PaymentDate,
                        LastPaymentAmount = latest?.LastPaymentAmount ?? (latest != null ? finalSal : null),
                        PaymentMode = latest?.PaymentMode ?? "Direct Bank Transfer (NEFT)",
                        TransactionReference = latest?.TransactionReference ?? $"TXN-GHS-{t.Id}2026",
                        PaymentStatus = latest?.PaymentStatus ?? (latest != null ? "Paid" : "Pending Record"),
                        PaymentCount = history.Count,
                        LatestPayment = latest,
                        History = history
                    };
                }).ToList();

                // Select teacher: either requested selectedId, or Meera Deshmukh (Id=5 if exists), or first
                TeacherPaymentItem? selected = null;
                if (selectedId.HasValue)
                {
                    selected = items.FirstOrDefault(i => i.TeacherId == selectedId.Value);
                }
                if (selected == null)
                {
                    selected = items.FirstOrDefault(i => i.FullName.Contains("Meera", StringComparison.OrdinalIgnoreCase)) 
                               ?? items.FirstOrDefault();
                }

                var vm = new TeacherPaymentsIndexViewModel
                {
                    Teachers = items,
                    SelectedTeacher = selected,
                    AllTeachers = teachers
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load payments: " + ex.Message;
                return View(new TeacherPaymentsIndexViewModel());
            }
        }

        // ======================================================================
        // GET: /TeacherPayments/Details/5 (Dedicated Voucher & Payslip View)
        // ======================================================================
        public async Task<IActionResult> Details(int? id)
        {
            var allTeachers = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
            if (!allTeachers.Any()) return RedirectToAction(nameof(Index));

            int teacherId = id ?? allTeachers.First().Id;
            var teacher = allTeachers.FirstOrDefault(t => t.Id == teacherId);
            if (teacher == null) return NotFound();

            var paymentHistory = await _context.TeacherPayments
                .Where(p => p.TeacherId == teacherId)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ThenByDescending(p => p.PaymentDate)
                .ToListAsync();

            var lastPayment = paymentHistory.FirstOrDefault();
            var lastMonth = DateTime.Today.AddMonths(-1);
            var lastMonthRecord = paymentHistory
                .FirstOrDefault(p => p.Month == lastMonth.Month && p.Year == lastMonth.Year) 
                ?? lastPayment;

            // Analytics
            var currentYear = DateTime.Today.Year;
            var currentYearPayments = paymentHistory.Where(p => p.Year == currentYear).ToList();
            if (!currentYearPayments.Any()) currentYearPayments = paymentHistory;

            decimal totalPaidYtd = currentYearPayments.Sum(p => p.LastPaymentAmount ?? p.NetSalary);
            decimal totalDeductionsYtd = currentYearPayments.Sum(p => p.Deductions);
            int totalPresentYtd = currentYearPayments.Sum(p => p.DaysPresent);
            int totalWorkingYtd = currentYearPayments.Sum(p => p.TotalWorkingDays);
            double attendancePct = totalWorkingYtd > 0 ? Math.Round((double)totalPresentYtd / totalWorkingYtd * 100.0, 1) : 100.0;
            decimal avgMonthly = currentYearPayments.Any() ? Math.Round(totalPaidYtd / currentYearPayments.Count, 2) : 0m;

            var vm = new PaymentViewModel
            {
                Teacher = teacher,
                LastPayment = lastPayment,
                LastMonthRecord = lastMonthRecord,
                PaymentHistory = paymentHistory,
                AllTeachers = allTeachers,
                TotalEarningsYtd = totalPaidYtd,
                TotalDeductionsYtd = totalDeductionsYtd,
                TotalDaysPresentYtd = totalPresentYtd,
                TotalWorkingDaysYtd = totalWorkingYtd,
                AttendanceRateYtd = attendancePct,
                AverageMonthlyNetSalary = avgMonthly
            };

            return View(vm);
        }

        // ======================================================================
        // GET: /TeacherPayments/GetTeacherPaymentSummary?id=5 (Instant AJAX API)
        // ======================================================================
        [HttpGet]
        public async Task<IActionResult> GetTeacherPaymentSummary(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound(new { success = false, message = "Teacher not found." });

            var latest = await _context.TeacherPayments
                .Where(p => p.TeacherId == id)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ThenByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync();

            int workDays = latest?.TotalWorkingDays ?? 26;
            int worked = latest?.DaysPresent ?? 25;
            int absent = latest?.DaysAbsent ?? 1;
            int hol = latest?.Holidays ?? 0;
            decimal daily = latest?.DailySalary ?? (teacher.YearsOfExperience >= 10 ? 1600m : 1400m);
            decimal ded = latest?.Deductions ?? 1500m;
            decimal gross = worked * daily;
            decimal finalSal = latest?.LastPaymentAmount ?? Math.Max(0, gross - ded);
            double attPct = workDays > 0 ? Math.Round(((double)worked / workDays) * 100.0, 1) : 100.0;

            return Ok(new
            {
                success = true,
                teacher = new
                {
                    id = teacher.Id,
                    fullName = teacher.FullName,
                    subject = teacher.SubjectExpertise,
                    qualification = teacher.Qualification,
                    experience = teacher.YearsOfExperience,
                    email = teacher.Email,
                    phone = teacher.Phone,
                    joiningDate = teacher.JoiningDate.ToString("dd MMM yyyy")
                },
                payment = new
                {
                    monthName = latest?.MonthName ?? "Current Cycle",
                    year = latest?.Year ?? DateTime.Today.Year,
                    totalWorkingDays = workDays,
                    totalDaysWorked = worked,
                    absences = absent,
                    holidays = hol,
                    dailySalary = daily,
                    grossSalary = gross,
                    salaryDeductions = ded,
                    finalSalary = finalSal,
                    attendancePercentage = attPct,
                    lastPaymentDate = latest?.PaymentDate.ToString("dd MMM yyyy") ?? "Pending",
                    lastPaymentAmount = latest?.LastPaymentAmount ?? finalSal,
                    paymentMode = latest?.PaymentMode ?? "Direct Bank Transfer (NEFT)",
                    transactionReference = latest?.TransactionReference ?? "TXN-GHS-2026",
                    paymentStatus = latest?.PaymentStatus ?? "Paid"
                }
            });
        }

        // ======================================================================
        // POST: /TeacherPayments/Create
        // ======================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherPayment input)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required payment details correctly.";
                return RedirectToAction(nameof(Details), new { id = input.TeacherId });
            }

            // compute days absent if not provided
            if (input.DaysAbsent == 0 && input.TotalWorkingDays > 0)
            {
                input.DaysAbsent = input.TotalWorkingDays - input.DaysPresent - input.Holidays;
                if (input.DaysAbsent < 0) input.DaysAbsent = 0;
            }

            input.LastPaymentDate = input.PaymentDate;
            input.LastPaymentAmount = (input.DaysPresent * input.DailySalary) - input.Deductions;
            if (string.IsNullOrWhiteSpace(input.PaymentMode)) input.PaymentMode = "Direct Bank Transfer (NEFT)";
            if (string.IsNullOrWhiteSpace(input.PaymentStatus)) input.PaymentStatus = "Paid";
            if (string.IsNullOrWhiteSpace(input.TransactionReference))
            {
                input.TransactionReference = $"NEFT/SBI/{DateTime.Now:yyyyMMdd}/{new Random().Next(100000, 999999)}";
            }
            if (string.IsNullOrWhiteSpace(input.Remarks))
            {
                input.Remarks = $"Salary for {input.MonthName} {input.Year} credited to Bank Account";
            }

            _context.TeacherPayments.Add(input);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Payment of ₹{(input.LastPaymentAmount ?? 0):N2} recorded successfully for {input.MonthName} {input.Year}!";
            return RedirectToAction(nameof(Details), new { id = input.TeacherId });
        }

        // ======================================================================
        // View Models
        // ======================================================================
        public class TeacherPaymentsIndexViewModel
        {
            public List<TeacherPaymentItem> Teachers { get; set; } = new();
            public TeacherPaymentItem? SelectedTeacher { get; set; }
            public List<Teacher> AllTeachers { get; set; } = new();
        }

        public class TeacherPaymentItem
        {
            public int TeacherId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Qualification { get; set; } = string.Empty;
            public int ExperienceYears { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public DateTime JoiningDate { get; set; }
            public string? ProfessionalDescription { get; set; }

            // Requested properties
            public int TotalWorkingDays { get; set; }
            public int TotalDaysWorked { get; set; }
            public int Absences { get; set; }
            public int Holidays { get; set; }
            public decimal DailySalary { get; set; }
            public decimal GrossSalary { get; set; }
            public decimal SalaryDeductions { get; set; }
            public decimal FinalSalary { get; set; }
            public double AttendancePercentage { get; set; }

            public DateTime? LastPaymentDate { get; set; }
            public decimal? LastPaymentAmount { get; set; }
            public string PaymentMode { get; set; } = "Direct Bank Transfer (NEFT)";
            public string TransactionReference { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = "Paid";
            public int PaymentCount { get; set; }
            public TeacherPayment? LatestPayment { get; set; }
            public List<TeacherPayment> History { get; set; } = new();
        }

        public class PaymentViewModel
        {
            public Teacher? Teacher { get; set; }
            public TeacherPayment? LastPayment { get; set; }
            public TeacherPayment? LastMonthRecord { get; set; }
            public List<TeacherPayment> PaymentHistory { get; set; } = new();
            public List<Teacher> AllTeachers { get; set; } = new();
            public decimal TotalEarningsYtd { get; set; }
            public decimal TotalDeductionsYtd { get; set; }
            public int TotalDaysPresentYtd { get; set; }
            public int TotalWorkingDaysYtd { get; set; }
            public double AttendanceRateYtd { get; set; }
            public decimal AverageMonthlyNetSalary { get; set; }
        }
    }
}
