using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class Login
    {
        [Required(ErrorMessage = "手機必填")]
        [RegularExpression(@"^09[0-9]{8}$", ErrorMessage = "帳號格式不正確")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "密碼必填")]
        [RegularExpression(@"^.*(?=.{6,12})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "密碼格式不正確，請輸入6至12個字元，須混合使用英文大寫、英文小寫、數字。")]
        public string Pwd { get; set; }
    }
}