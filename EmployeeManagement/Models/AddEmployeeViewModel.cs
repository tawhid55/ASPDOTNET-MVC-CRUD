using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class AddEmployeeViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200, ErrorMessage = "Email cannot be longer than 200 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Range(0, long.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        [Display(Name = "Salary")]
        public long Salary { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(100, ErrorMessage = "Department cannot be longer than 100 characters.")]
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth > DateTime.Today)
                yield return new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(DateOfBirth) });
        }
    }
}
