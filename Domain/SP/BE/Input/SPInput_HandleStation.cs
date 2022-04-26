using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_HandleStation : SPInput_Base
    {
        /// <summary>
        /// 據點類別
        /// </summary>
        public int StationType { get; set; }

        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 據點名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 管理據點代碼
        /// </summary>
        public string ManagerStationID { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string UniCode { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public Int16 CityID { get; set; }

        /// <summary>
        /// 據點地塊行政區代碼
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Addr { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string TEL { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 據點描述(內部註記)
        /// </summary>
        public string in_description { get; set; }

        /// <summary>
        /// 據點描述(app顯示)
        /// </summary>
        public string show_description { get; set; }

        /// <summary>
        /// 是否還車位置資訊必填
        /// </summary>
        public int IsRequired { get; set; }

        /// <summary>
        /// 尋車需響喇叭 Y/N
        /// </summary>
        public string CarHornFlg { set; get; }

        /// <summary>
        /// 共同出車庫位
        /// </summary>
        public string StationPick { get; set; }

        /// <summary>
        /// 財務部門代碼
        /// </summary>
        public string FCode { get; set; }

        /// <summary>
        /// 有效起日
        /// </summary>
        public DateTime SDate { get; set; }

        /// <summary>
        /// 有效迄日
        /// </summary>
        public DateTime EDate { get; set; }

        /// <summary>
        /// 車位數
        /// </summary>
        public Int16 ParkingNum { get; set; }

        /// <summary>
        /// 實際上線數
        /// </summary>
        public int OnlineNum { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 上傳照片1
        /// </summary>
        public string fileName1 { get; set; }

        /// <summary>
        /// 上傳照片2
        /// </summary>
        public string fileName2 { get; set; }

        /// <summary>
        /// 上傳照片3
        /// </summary>
        public string fileName3 { get; set; }

        /// <summary>
        /// 上傳照片4
        /// </summary>
        public string fileName4 { get; set; }

        /// <summary>
        /// 上傳照片5
        /// </summary>
        public string fileName5 { get; set; }

        /// <summary>
        /// 上傳照片1照片說明
        /// </summary>
        public string fileDescript1 { get; set; }

        /// <summary>
        /// 上傳照片2照片說明
        /// </summary>
        public string fileDescript2 { get; set; }

        /// <summary>
        /// 上傳照片3照片說明
        /// </summary>
        public string fileDescript3 { get; set; }

        /// <summary>
        /// 上傳照片4照片說明
        /// </summary>
        public string fileDescript4 { get; set; }

        /// <summary>
        /// 上傳照片5照片說明
        /// </summary>
        public string fileDescript5 { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string UserID { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int Mode { get; set; }
    }
}