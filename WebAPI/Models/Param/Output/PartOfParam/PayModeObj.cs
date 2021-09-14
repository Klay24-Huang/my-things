using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 支付方式 
    /// </summary>
    public class PayModeObj
    {
        /// <summary>
        /// 付款方式(0:信用卡 1:錢包)
        /// </summary>
        public int PayMode { get; set; }
        /// <summary>
        /// 付款方式名稱
        /// </summary>
        public string PayModeName { get; set; }
        /// <summary>
        /// 是否有綁定過開通(0:否1:是)
        /// </summary>
        public int HasBind { get; set; }
        /// <summary>
        /// 付款顯示資訊
        /// </summary>
        public string PayInfo { get; set; }
        /// <summary>
        /// 餘額
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 是否自動儲值 (0:否1:是)
        /// </summary>
        public int AutoStoreFlag { get; set; }
        /// <summary>
        /// 未綁定時顯示的文字 (0:否1:是)
        /// </summary>
        public string NotBindMsg { get; set; }

    }
}