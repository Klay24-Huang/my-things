using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Input
{
    public class IAPI_MA_InsClean
    {
        public string manager { set; get; }
        public string CarNo { set; get; }
        public string SD { set; get; }
        public string ED { set; get; }
        public string SpecCode { set; get; }
    }
}