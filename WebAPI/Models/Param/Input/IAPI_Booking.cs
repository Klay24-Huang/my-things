using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 預約
    /// </summary>
    public class IAPI_Booking
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SDate { set; get; }
        /// <summary>
        /// 預計還車時間，同站才有
        /// </summary>
        public string EDate { set; get; }
        /// <summary>
        /// 車號，路邊及機車才會有
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車型，同站才會有
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 是否加購安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 Insurance { set; get; }
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 選擇的訂閱制制月租
        /// </summary>
        public Int64 MonId { get; set; }
        /// <summary>
        /// 手機的定位(緯度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLat { get; set; } = 0;
        /// <summary>
        /// 手機的定位(經度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLon { get; set; } = 0;
    }
}