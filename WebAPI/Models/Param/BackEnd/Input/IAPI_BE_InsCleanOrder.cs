using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_InsCleanOrder
    {
        public string manager { set; get; }
        public string CarNo { set; get; }
        public string SD { set; get; }
        public string ED { set; get; }
    }
}