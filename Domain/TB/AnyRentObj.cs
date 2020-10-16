using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 路邊租還輸出
    /// </summary>
    public class AnyRentObj
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車型代碼 20201006 ADD BY ADAM
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string CarOfArea { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public float Rental { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public float Mileage { set; get; }
        /// <summary>
        /// 是否有安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 安心服務每小時費用
        /// </summary>
        public int InsurancePrice { set; get; }
        /// <summary>
        /// 是否顯示活動文字
        /// </summary>
        public int ShowSpecial { set; get; }
        /// <summary>
        /// 活動文字
        /// </summary>
        public string SpecialInfo { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 供應商
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 幾顆星評價
        /// </summary>
        public float OperatorScore { set; get; }
        /// <summary>
        /// 車輛圖示
        /// </summary>
        public string CarTypePic { get; set; }
        /// <summary>
        /// 座位數
        /// </summary>
        public int Seat { get; set; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { get; set; }
    }
}
