using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class OPAI_TypeListParam
    {
        public int CodeId { get; set; } = 0;
        public string CodeNm { get; set; } = "";
        public int IsBind { get; set; } = 0;
        public string Disc { get; set; } = "";
    }
}