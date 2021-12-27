using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelayAPI.Models.Output
{
    public class OAPI_RelayPost
    {
        public bool IsSuccess { get; set; } 
        public string RtnMessage { get; set; }
        public string ResponseData { get; set; }
    }
}