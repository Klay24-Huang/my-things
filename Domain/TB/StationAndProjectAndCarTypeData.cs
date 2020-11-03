using System;

namespace Domain.TB
{
    public class StationAndProjectAndCarTypeData
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string PRONAME { set; get; }

        /// <summary>
        /// 優惠專案描述
        /// </summary>
        public string PRODESC { get; set; }

        /// <summary>
        /// 專案平日價
        /// </summary>
        public int Price { set; get; }
        /// <summary>
        /// 專案假日價
        /// </summary>
        public int PRICE_H { set; get; }
        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型代碼(群組代碼)
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車型圖片
        /// </summary>
        public string CarTypePic { set; get; }
        /// <summary>
        /// 業者icon
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 業者評分
        /// </summary>
        public Single OperatorScore { set; get; }

        /// <summary>
        ///座位數
        /// </summary>
        public int Seat { set; get; }
        public string StationID { set; get; }
        public string StationName { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 其他說明
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 計費模式
        /// </summary>
        public Int16 PayMode { set; get; }
        /// <summary>
        /// 站別類型
        /// </summary>
        public string CarOfArea { get; set; }
        /// <summary>
        /// 是否可租 Y/N
        /// </summary>
        public string IsRent { get; set; }
    }
}