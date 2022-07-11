using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 機車
    /// </summary>
    public class IAPI_MotorRent
    {
        /// <summary>
        /// 是否顯示全部
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int? ShowALL { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double? Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public double? Longitude { set; get; }
        /// <summary>
        /// 半徑
        /// </summary>
        public double? Radius { set; get; }

        /// <summary>
        /// 用車行程
        /// 1 = 個人身分，2 = 企業身分
        /// </summary>
        public Int16 CarTrip { get; set; }
    }
}