using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleMemberScore : IAPI_BE_Base
    {
        public string IDNO { get; set; }
        public string SON { get; set; }
        public string SCORE { get; set; }
        public string APP { get; set; }
        public string SEQ { get; set; }
    }
}