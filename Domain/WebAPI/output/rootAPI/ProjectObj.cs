using System;

namespace Domain.WebAPI.output.rootAPI
{
    /// <summary>
    /// 取得專案與資費GetProjectObj內層物件 
    /// </summary>
    public class ProjectObj
    {
        /// <summary>
        /// 站點代碼        //20201028 ADD BY ADAM
        /// </summary>
        public string StationID { set; get; }

        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjName { set; get; }

        /// <summary>
        /// 優惠專案描述
        /// </summary>
        public string ProDesc { get; set; }

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
        ///座位數
        /// </summary>
        public int Seat { set; get; }
        /// <summary>
        /// 業者icon
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 業者評分
        /// </summary>
        public Single OperatorScore { set; get; }

        /// <summary>
        /// 是否可加購安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 加購安心服務每小時費用

        /// </summary>
        public int InsurancePerHour { set; get; }
        /// <summary>
        /// 是否是最低價
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMinimum { set; get; } = 0;
        /// <summary>
        /// 預估費用
        /// </summary>
        //public int Bill { set; get; }
        public int Price { set; get; }      //20201028 ADD BY ADAM
        /// <summary>
        /// 平日每小時金額
        /// </summary>
        public int WorkdayPerHour { get; set; }

        /// <summary>
        /// 假日每小時金額
        /// </summary>
        public int HolidayPerHour { set; get; }

        /// <summary>
        /// 站別類型
        /// </summary>
        public string CarOfArea { get; set; }
        /// <summary>
        /// 其他備註
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否可租 Y/N
        /// </summary>
        public string IsRent { get; set; }
        /// <summary>
        /// 是否是常用據點 20210315 ADD BY ADAM
        /// </summary>
        public int IsFavStation { set; get; }

        /// <summary>
        /// 是否顯示牌卡
        /// </summary>
        /// <mark>未包含在查詢條件則為0:此邏輯有確認過</mark>
        public int IsShowCard { get; set; } = 1;

        public Int64 MonthlyRentId { get; set; } = 0;
    }
}