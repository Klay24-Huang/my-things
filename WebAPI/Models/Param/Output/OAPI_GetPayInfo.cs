using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 取得預設支付方式API 回傳物件
    /// </summary>
    public class OAPI_GetPayInfo
    {
        public int DefPayMode { get; set; }

        public int PayModeBindCount { get; set; }

        public List<PayModeObj> PayModeList { get; set; }
    }
}