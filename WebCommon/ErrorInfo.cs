using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
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
