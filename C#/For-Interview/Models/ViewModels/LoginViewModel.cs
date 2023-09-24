using System.ComponentModel.DataAnnotations;

namespace For_Interview.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="帳號不可為空")]
        public string Account { get; set; } = string.Empty;
        [Required(ErrorMessage ="密碼不可為空")]
        public string Password { get; set; } = string.Empty;
    }
}
