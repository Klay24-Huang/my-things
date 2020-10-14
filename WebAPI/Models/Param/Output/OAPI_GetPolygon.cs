using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetPolygon
    {
        //public List<PolygonData> PolygonObj { set; get; }
        /// <summary>
        /// 電子柵欄類型
        /// <para>0:可還車區域</para>
        /// <para>1:可還車區域中的不可還車區域</para>
        /// <para>2:高風險區域</para>
        /// <para>3:優惠區域</para>
        /// </summary>
        public int PolygonType { set; get; }
        public string[] PolygonObj { set; get; }
    }
}