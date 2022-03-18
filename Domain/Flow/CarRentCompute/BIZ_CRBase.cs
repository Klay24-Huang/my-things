using System.Collections.Generic;
using WebCommon;

namespace Domain.Flow.CarRentCompute
{
    public class BIZ_CRBase
    {
        /// <summary>
        /// 
        /// </summary>
        public bool flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string errCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ErrorInfo> lstError { get; set; }

        //20210109 ADD BY ADAM REASON.增加constructor
        public BIZ_CRBase()
        {
            flag = false;
            errMsg = "";
            errCode = "000000";
            lstError = new List<ErrorInfo>();
        }
    }
}