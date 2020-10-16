﻿using System;
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
        public string RentalTimeInterval { set; get; } = "";
        /// <summary>
        /// 可折抵時數 (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeInterval { set; get; } = "";
        /// <summary>
        /// 可折抵時數(汽車) (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeCarInterval { set; get; } = "";
        /// <summary>
        /// 可折抵時數(機車) (time interval since 1970, 因為UI顯示x天y時z分, 保留未來彈性)
        /// </summary>
        public string RedeemingTimeMotorInterval { set; get; } = "";
        /// <summary>
        ///  代表該「實際」可折抵的時數
        /// </summary>
        public string ActualRedeemableTimeInterval { set; get; } = "";
        /// <summary>
        ///  代表折抵後的租用時數
        /// </summary>
        public string RemainRentalTimeInterval { set; get; } = "";
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
        /// 轉乘優惠，輸出時要*-1
        /// </summary>
        public int TransferPrice { set; get; } = 0;
        /// <summary>
        /// 安心保，純租金
        /// </summary>
        public int InsurancePurePrice { set; get; } = 0;
        /// <summary>
        /// 安心保，延長費用
        /// </summary>
        public int InsuranceExtPrice { set; get; } = 0;
    }
}