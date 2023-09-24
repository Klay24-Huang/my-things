using System.ComponentModel.DataAnnotations;

namespace For_Interview.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="帳號不可為空")]
        [StringLength(15, MinimumLength = 8, ErrorMessage ="帳號需8-15英文字")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage ="密碼不可為空")]
        [StringLength(20, MinimumLength = 8, ErrorMessage ="密碼長度為8-20字")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage ="確認密碼不可為空")]
        [Compare("Password", ErrorMessage ="密碼不相同")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度為8-20字")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage ="姓名不可為空")]
        [StringLength(10, MinimumLength = 3, ErrorMessage ="姓名長度為3-10字")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="email不可為空")]
        [EmailAddress(ErrorMessage ="email格式不正確")]
        public string Email { get; set; } = string.Empty;

        [Required]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "組織不可為空")]
        public string Organizatoin { get; set; } = string.Empty;

        [Required(ErrorMessage = "附檔不可為空")]

        public IFormFile? File { get; set; }
    }
}
