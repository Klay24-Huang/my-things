using System;

namespace Domain.SP.Input.Car
{
    public class SPInput_GetAnyRentCar : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// 是否顯示全部(0:否 1:是)
        /// </summary>
        public int ShowALL { get; set; }

        /// <summary>
        /// 最小緯度
        /// </summary>
        public double MinLatitude { get; set; }

        /// <summary>
        /// 最小經度
        /// </summary>
        public double MinLongitude { get; set; }

        /// <summary>
        /// 最大緯度
        /// </summary>
        public double MaxLatitude { get; set; }

        /// <summary>
        /// 最大經度
        /// </summary>
        public double MaxLongitude { get; set; }
    }
}