using System;

namespace Domain.WebAPI.output.rootAPI
{
    /// <summary>
    /// 機車專案
    /// </summary>
    public class MotorProjectObj
    {
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
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 基本費
        /// </summary>
        public int BasePrice { set; get; }
        /// <summary>
        /// 每分鐘n元
        /// </summary>
        public Single PerMinutesPrice { set; get; }
        /// <summary>
        /// 每日上限
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 站別類型
        /// </summary>
        public string CarOfArea { get; set; }
        /// <summary>
        /// 其他備註
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 剩餘電量
        /// </summary>
        public float Power { set; get; }
        /// <summary>
        /// 剩餘里程
        /// </summary>
        public float RemainingMileage { set; get; }
    }
}