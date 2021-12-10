using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class SignUpProfile
    {
        [Required(ErrorMessage = "姓名必填")]
        public string Name { get; set; }
        [Required(ErrorMessage = "性別必填")]
        public string Sex { get; set; }
        [Required(ErrorMessage = "身分證必填")]
        public string CustID { get; set; }
        [Required(ErrorMessage = "生日必填")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "生日格式不正確")]
        public string Birth { get; set; }
        [Required(ErrorMessage = "Email必填")]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$", ErrorMessage = "Email格式不正確")]
        public string Email { get; set; }
    }

}