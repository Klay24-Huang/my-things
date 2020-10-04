using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 查詢綁卡及錢包
    /// </summary>
    public class OAPI_CreditAndWalletQuery
    {
        /// <summary>
        /// 是否有綁定
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int HasBind { set; get; } = 0;
        public List<CreditCardBindList> BindListObj { set; get; }
        /// <summary>
        /// 是否有錢包
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int HasWallet { set; get; } = 0;
        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int TotalAmount { set; get; }
    }
}