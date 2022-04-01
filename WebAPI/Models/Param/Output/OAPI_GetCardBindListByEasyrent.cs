using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 官網查信用卡綁定列表
    /// </summary>
    public class OAPI_GetCardBindListByEasyrent
    {
        /// <summary>
        /// 是否有綁定
        /// </summary>
        public int HasBind { set; get; }
        public List<CreditCardBindListByEasyrent> BindListObj { set; get; }

    }
}