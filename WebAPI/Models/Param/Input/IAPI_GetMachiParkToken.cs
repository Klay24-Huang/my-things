using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetMachiParkToken
    {
        public string user_id { set; get; }
        public string sig { set; get; }
    }
}