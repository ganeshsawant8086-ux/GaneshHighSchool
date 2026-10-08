using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganesh1.Models
{
    public class TeacherPayment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a teacher.")]
        [Display(Name = "Teacher / Faculty")]
        public int TeacherId { get; set; }

        public Teacher? Teacher { get; set; }

        [Display(Name = "Disbursement Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Display(Name = "Pay Month")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; } = DateTime.Today.Month;

        [Display(Name = "Year")]
        [Range(2020, 2035, ErrorMessage = "Please enter a valid year.")]
        public int Year { get; set; } = DateTime.Today.Year;

        [Display(Name = "Total Working Days")]
        [Range(1, 31, ErrorMessage = "Working days must be between 1 and 31.")]
        public int TotalWorkingDays { get; set; } = 26;

        [Display(Name = "Total Days Worked (Present)")]
        [Range(0, 31, ErrorMessage = "Worked days must be between 0 and 31.")]
        public int DaysPresent { get; set; } = 24;

        [Display(Name = "Absences (Days Absent)")]
        [Range(0, 31, ErrorMessage = "Absences must be between 0 and 31.")]
        public int DaysAbsent { get; set; } = 1;

        [Display(Name = "Holidays / Approved Leaves")]
        [Range(0, 31, ErrorMessage = "Holidays must be between 0 and 31.")]
        public int Holidays { get; set; } = 1;

        [Display(Name = "Daily Salary (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Range(100, 100000, ErrorMessage = "Please enter a valid daily salary.")]
        public decimal DailySalary { get; set; } = 1500m;

        [Display(Name = "Salary Deductions (PF / PT) (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Range(0, 100000, ErrorMessage = "Deductions cannot be negative.")]
        public decimal Deductions { get; set; } = 1500m;

        [Display(Name = "Last Payment Date")]
        public DateTime? LastPaymentDate { get; set; }

        [Display(Name = "Last Payment Amount (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal? LastPaymentAmount { get; set; }

        [Display(Name = "Payment Mode")]
        public string? PaymentMode { get; set; } = "Direct Bank Transfer (NEFT)";

        [Display(Name = "Transaction ID")]
        public string? TransactionReference { get; set; }

        [Display(Name = "Payment Status")]
        public string? PaymentStatus { get; set; } = "Paid";

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; } = "Monthly salary disbursed";

        // ==========================================
        // Computed Business Logic & Calculations
        // ==========================================
        [NotMapped]
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary => DaysPresent * DailySalary;

        [NotMapped]
        [Display(Name = "Final Salary (Net Payable)")]
        public decimal NetSalary => Math.Max(0, (DaysPresent * DailySalary) - Deductions);

        [NotMapped]
        [Display(Name = "Final Salary")]
        public decimal FinalSalary => LastPaymentAmount ?? NetSalary;

        [NotMapped]
        public double AttendancePercentage => TotalWorkingDays > 0 
            ? Math.Round(((double)DaysPresent / TotalWorkingDays) * 100.0, 1) 
            : 100.0;

        [NotMapped]
        public string MonthName
        {
            get
            {
                try
                {
                    return new DateTime(Year > 1900 ? Year : DateTime.Today.Year, Month >= 1 && Month <= 12 ? Month : 1, 1).ToString("MMMM");
                }
                catch
                {
                    return $"Month {Month}";
                }
            }
        }
    }
}
