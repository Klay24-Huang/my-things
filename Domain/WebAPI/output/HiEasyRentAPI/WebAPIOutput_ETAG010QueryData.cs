using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// ETAG010Query output data
    /// </summary>
    public class WebAPIOutput_ETAG010QueryData
    {
        /// <summary>
        /// iRent訂單編號，含H
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 合約編號
        /// </summary>
        public string CNTRNO { set; get; }
        /// <summary>
        /// 應繳金額
        /// </summary>
        public string TAMT { set; get; }
    }
}
