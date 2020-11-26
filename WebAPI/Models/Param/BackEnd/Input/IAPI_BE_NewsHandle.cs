using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_NewsHandle:IAPI_BE_Base
    {
        public int NewsID { set; get; }
        public string Title { set; get; }
        public string Content { set; get; }
        public Int16 NewsType { set; get; }
        public Int16 NewsClass { set; get; }
        public string URL { set; get; }
        public DateTime SD { set; get; }
        public DateTime ED { set; get; }
        public Int16 Mode { set; get; }
        public string BeTop {get; set; }
    }
}