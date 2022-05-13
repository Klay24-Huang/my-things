namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class AlertMailObj
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 預約時間
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string StopTime { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }
        
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string MEMCNAME { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string MEMTEL { get; set; }

        /// <summary>
        /// 據點名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 管轄門市
        /// </summary>
        public string ManageStationID { get; set; }
    }
}