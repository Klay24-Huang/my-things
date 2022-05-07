namespace Domain.Flow.CarRentCompute
{
    public class DayPayMins
    {
        /// <summary>
        /// 日期分類
        /// </summary>
        public string DateType { get; set; }

        /// <summary>
        /// 格式yyyyMMdd
        /// </summary>
        public string xDate { get; set; }

        /// <summary>
        /// 開始時間:格式yyyyMMddHHmm
        /// </summary>
        public string xSTime { get; set; }

        /// <summary>
        /// 結束時間:格式yyyyMMddHHmm
        /// </summary>
        public string xETime { get; set; }

        /// <summary>
        /// 當日付費分鐘
        /// </summary>
        public double xMins { get; set; }

        /// <summary>
        /// 費率,汽車(每小時),機車每分鐘
        /// </summary>
        public double xRate { get; set; }

        /// <summary>
        /// 是否為註記日, 平日,假日,月租平日,月租假日
        /// </summary>
        public bool isMarkDay { get; set; }

        /// <summary>
        /// 計費區段是否有下一日,1(有),0(沒有)
        /// </summary>
        public int haveNext { get; set; }

        /// <summary>
        /// 是否為時間區段起點,1是,0否
        /// </summary>
        public int isStart { get; set; }

        /// <summary>
        /// 是否為時間區段結束,1是,0否
        /// </summary>
        public int isEnd { get; set; }

        /// <summary>
        /// 使用基本時間分鐘
        /// </summary>
        public double useBaseMins { get; set; }

        /// <summary>
        /// 時間區段id
        /// </summary>
        public string dayGroupId { get; set; }

        /// <summary>
        /// 是否為group起,1是,0否
        /// </summary>
        public double isGrpStar { get; set; }

        /// <summary>
        /// 是否為group迄,1是,0否
        /// </summary>
        public double isGrpEnd { get; set; }

        /// <summary>
        /// 是否為首24H,1是,0否
        /// </summary>
        public bool isF24H { get; set; }

        /// <summary>
        /// 是否為完整24H,1是,0否
        /// </summary>
        public bool isFull24H { get; set; }

        /// <summary>
        /// 使用優惠分鐘數
        /// </summary>
        public double UseGiveMinute { get; set; }
    }
}