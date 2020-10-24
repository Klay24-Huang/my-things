namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMotorStatus
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        /// 車輛發動狀態，發動為1，熄火為0
        /// </summary>
        public int ACCStatus { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public float Millage { get; set; }

        /// <summary>
        /// 預估里程
        /// </summary>
        public string deviceRDistance { get; set; }

        /// <summary>
        /// 總電量兩顆平均
        /// </summary>
        public float device2TBA { get; set; }

        /// <summary>
        /// 總電量三顆平均
        /// </summary>
        public float device3TBA { get; set; }

        /// <summary>
        /// 核心電池
        /// </summary>
        public float deviceMBA { get; set; }

        /// <summary>
        /// 右邊電池
        /// </summary>
        public float deviceRBA { get; set; }

        /// <summary>
        /// 左共享電池
        /// </summary>
        public float deviceLBA { get; set; }

        /// <summary>
        /// 租約狀態
        /// </summary>
        public int extDeviceStatus1 { get; set; }

        /// <summary>
        /// 電池蓋
        /// </summary>
        public int deviceBat_Cover { get; set; }
    }
}