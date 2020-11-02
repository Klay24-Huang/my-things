using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_MonthlySubscription
    {
        public List<MonthlySubscriptionData> MonthlySubscriptionObj { set; get; }
    }
    public class MonthlySubscriptionData
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public float Workday { set; get; }
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public float Holiday { set; get; }
        /// <summary>
        /// 機車分鐘數
        /// </summary>
        public float MotoTotal { set; get; }
        /// <summary>
        /// 起始時間
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndDate { set; get; }
        /// <summary>
        /// 優惠費率
        /// </summary>
        public float FavHFee { set; get; }
        /// <summary>
        /// 流水號
        /// </summary>
        public int seqno { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjNM { set; get; }
    }
}