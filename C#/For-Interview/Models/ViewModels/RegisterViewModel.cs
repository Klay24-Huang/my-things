using System.ComponentModel.DataAnnotations;

namespace For_Interview.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string Account { get; set; } = string.Empty;
        [Required]
        [StringLength(8, MinimumLength = 20)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password")]
        [StringLength(8, MinimumLength = 20)]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(10, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        [Required]
        public string Organizatoin { get; set; } = string.Empty;
        [Required]

        public IFormFile? File { get; set; }
    }
}
