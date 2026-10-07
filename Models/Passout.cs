namespace Ganesh1.Models
{
    public class Passout
    {
        public int Id { get; set; }

        public int Year { get; set; }

        // Student full name
        public string FullName { get; set; } = string.Empty;

        // Percentage of marks
        public decimal Percentage { get; set; }

        // Optional example contact to display (masked if needed)
        public string? ExampleContact { get; set; }

        // Specialization subject (Science, Commerce, Arts etc.)
        public string? Specialization { get; set; }
    }
}
