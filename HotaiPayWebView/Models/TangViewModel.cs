using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HotaiPayWebView.Models
{
    public class TangViewModel
    {
        [Required(ErrorMessage = "身分證必填")]
        public string CTBCIDNO { get; set; }

        [Required(ErrorMessage = "生日必填")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "生日格式不正確")]
        public string Birthday { get; set; }
    }
}