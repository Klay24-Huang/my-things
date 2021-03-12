using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMonthList
    {
        /// <summary>
        /// 月租或短期
        /// <para>0月租</para>
        /// <para>1短期</para>
        /// </summary>
        //public int MonthMode { get; set; }
        //public string Title { get; set; }
        /// <summary>
        /// 汽車或機車
        /// <para>0汽車</para>
        /// <para>1機車</para>
        /// </summary>
        //public int CarMode { get; set; }
        public List<MonCardParam> MonCards { get; set; }
    }
    
}