using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        ///  實際租用時數 (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RentalTimeInterval { set; get; } = "0";
        /// <summary>
        /// 可折抵時數 (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeInterval { set; get; } = "0";
        /// <summary>
        /// 可折抵時數(汽車) (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeCarInterval { set; get; } = "0";
        /// <summary>
        /// 可折抵時數(機車) (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeMotorInterval { set; get; } = "0";
        /// <summary>
        ///  代表該「實際」可折抵的時數
        /// </summary>
        public string ActualRedeemableTimeInterval { set; get; } = "0";
        /// <summary>
        ///  代表折抵後的租用時數
        /// </summary>
        public string RemainRentalTimeInterval { set; get; } = "0";
        /// <summary>
        /// 月租專案時數折抵顯示
        /// 20201128 ADD BY ADAM REASON.
        /// </summary>
        public string UseMonthlyTimeInterval { set; get; } = "0";
        /// <summary>
        /// 一般時段時數折抵
        /// </summary>
        public string UseNorTimeInterval { get; set; } = "0";
        /// <summary>
        /// 每小時基本租金
        /// </summary>
        public int RentBasicPrice { set; get; } = 0;
        /// <summary>
        /// 車輛租金
        /// </summary>
        public int CarRental { set; get; } = 0;
        /// <summary>
        /// 哩程費用
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
        /// 總計
        /// </summary>
        public int TotalRental { set; get; } = 0;

        /// <summary>
        /// 車麻吉費用
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
    }
}