using System;

namespace Domain.TB.BackEnd
{
    public class BE_GetStationInfo
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 管轄據點代碼
        /// </summary>
        public string ManageStationID { set; get; }
        /// <summary>
        /// 據點名稱
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 電話
        /// </summary>
        public string Tel { set; get; }
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
        /// 描述（內部）
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 描述（app）
        /// </summary>
        public string ContentForAPP { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNICode { set; get; }
        /// <summary>
        /// 縣市代碼
        /// </summary>
        public Int16 CityID { set; get; }
        /// <summary>
        /// 行政區代碼
        /// </summary>
        public int AreaID { set; get; }
        /// <summary>
        /// 還車資訊是否必填
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsRequiredForReturn { set; get; }
        /// <summary>
        /// 共同出車庫位代碼
        /// </summary>
        public string CommonLendStation { set; get; }
        /// <summary>
        /// 財務代碼
        /// </summary>
        public string FCODE { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SDate { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime EDate { set; get; }
        /// <summary>
        /// 據點類型
        /// <para>0:同站</para>
        /// <para>1:路邊</para>
        /// </summary>
        public Int16 IsNormalStation { set; get; }
        /// <summary>
        /// 車位數
        /// </summary>
        public Int16 AllowParkingNum { set; get; }
        /// <summary>
        /// 目前上線數
        /// </summary>
        public int NowOnlineNum { set; get; }
        /// <summary>
        /// 是否啟用
        /// <para>0:停用</para>
        /// <para>1:啟用</para>
        /// </summary>
        public Int16 use_flag { set; get; }
        /// <summary>
        /// 路邊租還車輛顯示名稱
        /// </summary>
        public string Area { set; get; }
    }
}