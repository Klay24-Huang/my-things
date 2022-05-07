using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_MotorRent
    {
       public List<OAPI_MotorRent_Param> MotorRentObj { set; get; }
    }

    public class OAPI_MotorRent_Param: MotorRentObj
    {
        /// <summary>
        /// 訂閱制月租Id
        /// </summary>
        public Int64 MonthlyRentId { get; set; } = 0;

        /// <summary>
        /// 訂閱制月租名稱
        /// </summary>
        public string MonProjNM { get; set; } = "";
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public double CarWDHours { get; set; } = 0;
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public double CarHDHours { get; set; } = 0;
        /// <summary>
        /// 機車不分平假日時數
        /// </summary>
        public int MotoTotalMins { get; set; } = 0;
        /// <summary>
        /// 汽車平日優惠價
        /// </summary>
        public double WDRateForCar { get; set; } = 0;
        /// <summary>
        /// 汽車假日優惠價
        /// </summary>
        public double HDRateForCar { get; set; } = 0;
        /// <summary>
        /// 機車平日優惠價
        /// </summary>
        public double WDRateForMoto { get; set; } = 0;
        /// <summary>
        /// 機車假日優惠價
        /// </summary>
        public double HDRateForMoto { get; set; } = 0;

        /// <summary>
        /// 優惠標籤
        /// </summary>
        public DiscountLabel DiscountLabel { get; set; } = new DiscountLabel();
    }

}