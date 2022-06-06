using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetEstimateMotor
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SDate { get; set; }

        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string EDate { get; set; }

        /// <summary>
        /// 是否加購安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Insurance { get; set; }

        /// <summary>
        /// 訂閱制ID
        /// </summary>
        public int MonId { get; set; } = 0;
    }
}