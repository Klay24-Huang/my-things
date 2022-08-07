namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 租金
    /// </summary>
    public class RentBase
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; } = "";

        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string BookingStartDate { set; get; } = "";

        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string BookingEndDate { set; get; } = "";

        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string RentalDate { set; get; } = "";

        /// <summary>
        /// 實際租用時數
        /// </summary>
        public string RentalTimeInterval { set; get; } = "0";

        /// <summary>
        /// 可折抵時數 (實際租用時數 - 訂閱制時數 - 標籤優惠時數)
        /// </summary>
        public int CanDiscountTime { get; set; }

        /// <summary>
        /// 可使用的折抵總時數 (汽車:RedeemingTimeCarInterval 機車:RedeemingTimeCarInterval+RedeemingTimeMotorInterval)
        /// </summary>
        public string RedeemingTimeInterval { set; get; } = "0";

        /// <summary>
        /// 可使用的折抵時數(汽車)
        /// </summary>
        public string RedeemingTimeCarInterval { set; get; } = "0";

        /// <summary>
        /// 可使用的折抵時數(機車)
        /// </summary>
        public string RedeemingTimeMotorInterval { set; get; } = "0";

        /// <summary>
        /// 實際可折抵的時數
        /// </summary>
        public string ActualRedeemableTimeInterval { set; get; } = "0";

        /// <summary>
        /// 折抵後的租用時數
        /// </summary>
        public string RemainRentalTimeInterval { set; get; } = "0";

        /// <summary>
        /// 月租折抵時數
        /// 20201128 ADD BY ADAM REASON.
        /// </summary>
        public string UseMonthlyTimeInterval { set; get; } = "0";

        /// <summary>
        /// 一般時段時數折抵
        /// </summary>
        public string UseNorTimeInterval { get; set; } = "0";

        /// <summary>
        /// 標籤優惠折抵時數
        /// </summary>
        public int UseGiveMinute { get; set; }

        /// <summary>
        /// 每小時基本租金
        /// </summary>
        public int RentBasicPrice { set; get; } = 0;

        /// <summary>
        /// 車輛租金
        /// </summary>
        public int CarRental { set; get; } = 0;

        /// <summary>
        /// 里程費用
        /// </summary>
        public int MileageRent { set; get; } = 0;

        /// <summary>
        /// ETAG費用
        /// </summary>
        public int ETAGRental { set; get; } = 0;

        /// <summary>
        /// 逾時費用
        /// </summary>
        public int OvertimeRental { set; get; } = 0;

        /// <summary>
        /// 停車費用
        /// </summary>
        public int ParkingFee { set; get; } = 0;

        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransferPrice { set; get; } = 0;

        /// <summary>
        /// 安心服務費用
        /// </summary>
        public int InsurancePurePrice { set; get; } = 0;

        /// <summary>
        /// 安心服務費用(逾時)
        /// </summary>
        public int InsuranceExtPrice { set; get; } = 0;

        /// <summary>
        /// 總計
        /// </summary>
        public int TotalRental { set; get; } = 0;

        /// <summary>
        /// 預授權金額
        /// </summary>
        public int PreAmount { get; set; } = 0;

        /// <summary>
        /// 差額
        /// </summary>
        public int DiffAmount { get; set; } = 0;

        /// <summary>
        /// 企業客戶統一編號
        /// </summary>
        public string EnterpriseTaxID { get; set; } = "";

        /// <summary>
        /// 企業客戶Etag請款項目 (0:個人 1:公司)
        /// </summary>
        public int EnterpriseEtag { get; set; } = 0;

        /// <summary>
        /// 企業客戶安心服務請款項目 (0:個人 1:公司)
        /// </summary>
        public int EnterpriseInsurance { get; set; } = 0;

        /// <summary>
        /// 企業客戶停車費請款項目 (0:個人 1:公司)
        /// </summary>
        public int EnterpriseParking { get; set; } = 0;

        /// <summary>
        /// 企業月結金額
        /// </summary>
        public int EnterpriseFee { get; set; } = 0;

        /// <summary>
        /// 自付款項金額
        /// </summary>
        public int PersonalFee { get; set; } = 0;
    }
}