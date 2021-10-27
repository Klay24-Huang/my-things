using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_MonthlyMain
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 平日剩餘總時數
        /// </summary>
        public Single WorkDayHours { set; get; }
        /// <summary>
        /// 假日剩餘總時數
        /// </summary>
        public Single HolidayHours { set; get; }
        /// <summary>
        /// 機車剩餘總時數
        /// </summary>
        public Single MotoTotalHours { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime StartDate { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime EndDate { set; get; }
        public int SEQNO { set; get; }
        public string ProjID { set; get; }
        public string ProjNM { set; get; }
        /// <summary>
        /// 綁約期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 是否綁約(Y/N)
        /// </summary>
        public string IsTiedUp { get; set; }
        /// <summary>
        /// 自動續約(Y/N)
        /// </summary>
        public string AutomaticRenewal { get; set; }
        /// <summary>
        /// 合約是否有效(0:無效,1:有效)
        /// </summary>
        public int useFlag { get; set; }
        /// <summary>
        /// 合約失效原因(1:升轉,2:解約,3:人工解約)
        /// </summary>
        public int FlagCodeId { get; set; }
        /// <summary>
        /// 訂閱方案群組編號
        /// </summary>
        public int SubsId { get; set; }
        /// <summary>
        /// 訂閱方案編號
        /// </summary>
        public int MonthlyRentId { get; set; }
        /// <summary>
        /// 是否為汽機車
        /// </summary>
        public int IsMoto { get; set; }
    }
}