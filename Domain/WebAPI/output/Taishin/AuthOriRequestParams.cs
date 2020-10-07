﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class AuthOriRequestParams
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public AuthRequestParams RequestParams { get; set; }
        public string Random { get; set; }
        public string TimeStamp { get; set; }
        public string CheckSum { get; set; }
    }
}
