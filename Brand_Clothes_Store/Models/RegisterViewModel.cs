using System.ComponentModel.DataAnnotations;

namespace Brand_Clothes_Store.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]

        public string PhoneNumber { get; set; } = "";
        [Required]
        public string? Address { get; set; }
    }
}
