using System;
using System.ComponentModel.DataAnnotations;

namespace Ganesh1.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Subject Expertise")]
        public string SubjectExpertise { get; set; } = string.Empty;

        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        [Required]
        public string Qualification { get; set; } = string.Empty;

        [Display(Name = "Previous Organization")]
        public string? PreviousOrganization { get; set; }

        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        [Display(Name = "Professional Description")]
        public string? ProfessionalDescription { get; set; }
    }
}
