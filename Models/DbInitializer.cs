using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ganesh1.Models
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StudentDbContext>();

            try
            {
                // Ensure table exists via raw SQL if needed
                context.Database.ExecuteSqlRaw(@"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TeacherPayments')
BEGIN
    CREATE TABLE TeacherPayments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TeacherId INT NOT NULL,
        PaymentDate DATETIME2 NOT NULL,
        [Month] INT NOT NULL,
        [Year] INT NOT NULL,
        TotalWorkingDays INT NOT NULL,
        DaysPresent INT NOT NULL,
        DaysAbsent INT NOT NULL,
        Holidays INT NOT NULL,
        DailySalary DECIMAL(18,2) NOT NULL,
        Deductions DECIMAL(18,2) NOT NULL,
        LastPaymentDate DATETIME2 NULL,
        LastPaymentAmount DECIMAL(18,2) NULL,
        PaymentMode NVARCHAR(50) DEFAULT 'Direct Bank Transfer (NEFT)',
        TransactionReference NVARCHAR(100) DEFAULT 'TXN-GHS-2026',
        PaymentStatus NVARCHAR(50) DEFAULT 'Paid',
        Remarks NVARCHAR(250) DEFAULT 'Monthly salary credited',
        CONSTRAINT FK_TeacherPayments_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE CASCADE
    );
END
");

                // Check extra columns
                context.Database.ExecuteSqlRaw(@"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TeacherPayments') AND name = 'PaymentMode')
    ALTER TABLE TeacherPayments ADD PaymentMode NVARCHAR(50) DEFAULT 'Direct Bank Transfer (NEFT)' WITH VALUES;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TeacherPayments') AND name = 'TransactionReference')
    ALTER TABLE TeacherPayments ADD TransactionReference NVARCHAR(100) DEFAULT 'TXN-GHS-2026' WITH VALUES;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TeacherPayments') AND name = 'PaymentStatus')
    ALTER TABLE TeacherPayments ADD PaymentStatus NVARCHAR(50) DEFAULT 'Paid' WITH VALUES;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TeacherPayments') AND name = 'Remarks')
    ALTER TABLE TeacherPayments ADD Remarks NVARCHAR(250) DEFAULT 'Monthly salary credited' WITH VALUES;
");

                // If no records in TeacherPayments, seed realistic records for existing teachers
                if (!context.TeacherPayments.Any())
                {
                    var teachers = context.Teachers.ToList();
                    foreach (var teacher in teachers)
                    {
                        decimal dailyRate = teacher.YearsOfExperience switch
                        {
                            >= 10 => 1600m,
                            >= 7 => 1450m,
                            >= 5 => 1350m,
                            _ => 1200m
                        };

                        decimal deductions = 1500m;

                        // Seed past 6 months
                        var months = new[]
                        {
                            new { Month = 9, Year = 2026, Work = 26, Pres = 25, Abs = 1, Hol = 0, PayDate = new DateTime(2026, 10, 2) },
                            new { Month = 8, Year = 2026, Work = 26, Pres = 24, Abs = 1, Hol = 1, PayDate = new DateTime(2026, 9, 2) },
                            new { Month = 7, Year = 2026, Work = 27, Pres = 26, Abs = 0, Hol = 1, PayDate = new DateTime(2026, 8, 2) },
                            new { Month = 6, Year = 2026, Work = 25, Pres = 24, Abs = 1, Hol = 0, PayDate = new DateTime(2026, 7, 2) },
                            new { Month = 5, Year = 2026, Work = 22, Pres = 20, Abs = 1, Hol = 1, PayDate = new DateTime(2026, 6, 2) },
                            new { Month = 4, Year = 2026, Work = 25, Pres = 25, Abs = 0, Hol = 0, PayDate = new DateTime(2026, 5, 2) }
                        };

                        foreach (var m in months)
                        {
                            decimal net = (m.Pres * dailyRate) - deductions;
                            context.TeacherPayments.Add(new TeacherPayment
                            {
                                TeacherId = teacher.Id,
                                PaymentDate = m.PayDate,
                                Month = m.Month,
                                Year = m.Year,
                                TotalWorkingDays = m.Work,
                                DaysPresent = m.Pres,
                                DaysAbsent = m.Abs,
                                Holidays = m.Hol,
                                DailySalary = dailyRate,
                                Deductions = deductions,
                                LastPaymentDate = m.PayDate,
                                LastPaymentAmount = net,
                                PaymentMode = "Direct Bank Transfer (NEFT)",
                                TransactionReference = $"NEFT/SBI/2026{m.Month:00}02/{teacher.Id}{m.Month:00}{new Random().Next(100, 999)}",
                                PaymentStatus = "Paid",
                                Remarks = $"Salary credited to Bank A/C ending {4400 + teacher.Id}"
                            });
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Silently handle or log initialization errors so app starts smoothly
                Console.WriteLine($"[DbInitializer Error]: {ex.Message}");
            }
        }
    }
}
