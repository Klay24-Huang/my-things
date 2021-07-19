using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 重算租金
    /// </summary>
    public class IAPI_RePayDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 汽車時數
        /// </summary>
        public int Discount { set; get; }
        /// <summary>
        /// 機車時數
        /// </summary>
        public int MotorDiscount { set; get; }
        /// <summary>
        /// 重計價模式 0:只算租金 1:租金+月租(暫不開)
        /// </summary>
        public int RePayMode { set; get; } = 0;
        /// <summary>
        /// 是否儲存最後結果,1存,0不存
        /// </summary>
        public int IsSave { set; get; } = 0;
        /// <summary>
        /// 月租是否存檔,1存,0不存
        /// </summary>
        public int IsMontnSave { get; set; } = 0;
        /// <summary>
        /// 是否輸出json
        /// </summary>
        public int jsonOut { get; set; } = 0;//0不輸出,1輸出jsonDts,2輸出traceDts
        /// <summary>
        /// 月租Id,可多筆
        /// </summary>
        public string MonIds { get; set; }

    }
}