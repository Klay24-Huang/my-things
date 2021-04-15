using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMonthGroup
    {
        public string MonProDisc { get; set; }
        /// <summary>
        /// 是否為使用中
        /// </summary>
        //public int IsOrder { get; set; } = 0;
        public List<GetMonthGroup_MonCardParam> MonCards { get; set; }
    }

    public class GetMonthGroup_MonCardParam: MonCardParam
    {
        public int IsOrder { get; set; }
    }

}