using System;

namespace Domain.TB
{
    /// <summary>
    /// 機車取得專案
    /// </summary>
    public class ProjectAndCarTypeDataForMotor
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

        /// <summary>
        /// 安心服務起始費用(類似機車基消)
        /// </summary>
        public int BaseMotoRate { set; get; }
        /// <summary>
        /// 計次分鐘單位(幾分鐘算一次錢)
        /// </summary>
        public int InsuranceMotoMin { set; get; }
        /// <summary>
        /// 計次金額(每分鐘單位多少錢)
        /// </summary>
        public int InsuranceMotoRate { set; get; }
        /// <summary>
        /// 安心服務欄位預設值(0:無法購買, 1預設不勾, 2預設勾選)
        /// </summary>
        public int InsuranceDef { set; get; }
        /// <summary>
        /// 機車安心服務基消分鐘數
        /// </summary>
        public int BaseInsuranceMinutes { set; get; }
    }
}