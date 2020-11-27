using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Maintain
{
    public class GetBookingMainForMaintain
    {
        public int OrderNum { set; get; }
        /// <summary>
        /// 訂單狀態
        /// <para>0:已預約</para>
        /// <para>1:已取車</para>
        /// <para>2:已還車</para>
        /// </summary>
        public int OrderStatus { set; get; }
        /// <summary>
        /// 整備人員帳號
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 車外清潔(0:否;1:是)
        /// </summary>
        public int outsideClean { set; get; }
        /// <summary>
        /// 內裝清潔0:否;1:是)
        /// </summary>
        public int insideClean { set; get; }
        /// <summary>
        /// 車輛救援0:否;1:是)
        /// </summary>
        public int rescue { set; get; }
        /// <summary>
        /// 非路邊租還調度0:否;1:是)
        /// </summary>
        public int dispatch { set; get; }
        /// <summary>
        /// 路邊租還調度0:否;1:是)
        /// </summary>
        public int Anydispatch { set; get; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string citizen_id { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string assigned_car_id { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime start_time { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime stop_time { set; get; }
        /// <summary>
        /// 車機代碼
        /// </summary>
        public int MachineID { set; get; }
        public int cancel_status { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string MachineNo { set; get; }
        /// <summary>
        /// 是否是汽車
        /// <para>0:否，機車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int IsCar { set; get; }

        public string Site_ID { set; get; }
        public string Location { set; get; }
    }
}
