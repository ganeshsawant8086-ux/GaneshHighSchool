using System.Collections.Generic;

namespace Ganesh1.Models
{
    public class DashboardViewModel
    {
        public List<Student> Students { get; set; } = new();
        public List<Passout> Passouts { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();
        public List<Stock> Stocks { get; set; } = new();

        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalPassouts { get; set; }
        public decimal AveragePassPercentage { get; set; }
        public int ActiveClasses { get; set; } = 10;

        // Clerk Stock & Inventory Metrics
        public int TotalStockItems { get; set; }
        public int TotalStockUnits { get; set; }
        public int LowStockAlerts { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
}
