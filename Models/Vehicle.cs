using System.ComponentModel.DataAnnotations;

namespace VehicleServiceManager.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression(
            @"^[A-Za-z0-9 -]{5,20}$",
            ErrorMessage = "Use 5 to 20 letters, numbers, spaces, or hyphens.")]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(
            @"^[A-Za-z][A-Za-z .'-]*$",
            ErrorMessage = "Make must contain letters and may include spaces, dots, apostrophes, or hyphens.")]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [RegularExpression(
            @"^[A-Za-z0-9][A-Za-z0-9 .'/+-]*$",
            ErrorMessage = "Model contains unsupported characters.")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2027, ErrorMessage = "Year must be between 1900 and 2027.")]
        public int Year { get; set; }

        [Display(Name = "Customer")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }
    }
}
