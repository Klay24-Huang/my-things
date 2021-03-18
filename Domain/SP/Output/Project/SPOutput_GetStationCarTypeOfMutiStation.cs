using System;

namespace Domain.SP.Output.Project
{
    public class SPOutput_GetStationCarTypeOfMutiStation
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
        /// 計算運算後租金
        /// </summary>
        public int PriceBill { get; set; }
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
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { set; get; }
        public string StationName { set; get; }
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
        /// 據點描述（app顯示）
        /// </summary>
        public string ContentForAPP { get; set; }
        /// <summary>
        /// 縣市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 行政區
        /// </summary>
        public string AreaName { get; set; }
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

        /// <summary>
        /// 是否可以申辦安心服務
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 安心服務計價
        /// </summary>
        public int InsurancePerHours { set; get; }

        /// <summary>
        /// 站點照片
        /// </summary>
        public string StationPicJson { get; set; }
        /// <summary>
        /// 每公里多少錢
        /// </summary>
        public float MilageBase { set; get; }
        /// <summary>
        /// 是否是常用據點 20210315 ADD BY ADAM
        /// </summary>
        public int IsFavStation { set; get; }

    }
}