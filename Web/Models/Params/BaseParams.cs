using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Params
{
    public class BaseParams
    {
        public string SERVER_NAME { get; set; }
        public string DATABASE_NAME { get; set; }
        public string LOGIN { get; set; }
        public string PASSWORD { get; set; }
        public string DATABASE_TYPE { get; set; }
        public string SSAPI_ADDRESS { get; set; }
    }
}