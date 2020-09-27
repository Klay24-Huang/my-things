using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_ArrearsQuery
    {
        /// <summary>
        /// 欠費資訊
        /// </summary>
        public List<ArrearsQueryDetail> ArrearsInfos { set; get; }

        /// <summary>
        /// 交易序號
        /// </summary>
        public string TradeOrderNo { set; get; }
    }
}