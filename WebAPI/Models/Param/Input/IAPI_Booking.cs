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
    }
}