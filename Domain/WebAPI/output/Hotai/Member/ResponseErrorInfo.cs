using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    public class ResponseErrorInfo
    {
        public Dictionary<Object, Object> Errors { get; set; }
    }

    public class Errors
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }
    }
}
