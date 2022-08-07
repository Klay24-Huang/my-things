using System;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetAnyRentProject
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SDate { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string EDate { set; get; }

        /// <summary>
        /// 用車行程
        /// 1 = 個人身分，2 = 企業身分
        /// </summary>
        public Int16 CarTrip { get; set; } = 1;
    }
}