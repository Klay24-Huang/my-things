using Domain.WebAPI.output.Taishin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BindResult
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public BindOriRequestParams OriRequestParams { get; set; }
        public BindResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
}