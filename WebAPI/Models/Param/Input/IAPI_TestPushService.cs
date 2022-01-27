using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_TestPushService
    {
        public int PushRegID { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Url { get; set; } = "";
    }
}