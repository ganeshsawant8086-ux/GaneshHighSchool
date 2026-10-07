using System.ComponentModel.DataAnnotations;

namespace Ganesh1.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public string StudentGender { get; set; } = "Male";

        public int? Age { get; set; }
        public int Standard { get; set; }

        [Display(Name = "Mobile Number")]
        public string? MobileNumber { get; set; }
    }
}
