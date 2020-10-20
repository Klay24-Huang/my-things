using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_SentCardSetting:IAPI_BE_Base
    {
        /// <summary>
        /// 卡號
        /// <para>0:萬用卡</para>
        /// <para>1:顧客卡</para>
        /// </summary>
        public int CardType { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
    }
}