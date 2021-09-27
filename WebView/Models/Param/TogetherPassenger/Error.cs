using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebView.Models.Param.TogetherPassenger
{
    public class Error
    {
        public string Result { get; set; }
        public string ErrorCode { get; set; }
        public int NeedRelogin { get; set; }
        public int NeedUpgrade { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}