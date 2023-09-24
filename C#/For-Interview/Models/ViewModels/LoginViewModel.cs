using System.ComponentModel.DataAnnotations;

namespace For_Interview.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Account { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
