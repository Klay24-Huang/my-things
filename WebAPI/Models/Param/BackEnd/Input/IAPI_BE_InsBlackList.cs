using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_InsBlackList
    {
        public string MOBILE { get; set; }
        public string USERID { get; set; }
        public string MEMO { get; set; }
        public string MODE { get; set; }
    }
}