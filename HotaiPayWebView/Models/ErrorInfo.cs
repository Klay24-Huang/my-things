using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class ErrorInfo
    {
        public string ErrorCode { set; get; }
        public string ErrorMsg { set; get; }
        public string ExtendsCode { set; get; }
        public string ExtendsMsg { set; get; }
        public bool isSendMsg { set; get; }
    }
}