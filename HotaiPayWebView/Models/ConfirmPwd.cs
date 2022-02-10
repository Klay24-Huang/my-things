using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class ConfirmPwd
    {
        [Required(ErrorMessage = "密碼必填")]
        [RegularExpression(@"^.*(?=.{6,12})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "密碼格式不正確，請輸入6至12個字元，須混合使用英文大寫、英文小寫、數字。")]
        public string Pwd { get; set; }


        [Compare("Pwd",ErrorMessage = "密碼與確認密碼不符")]
        public string PwdConfirm { get; set; }
    }
}