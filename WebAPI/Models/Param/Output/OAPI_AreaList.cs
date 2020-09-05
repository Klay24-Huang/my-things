using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 行政區及郵遞區號
    /// </summary>
    public class OAPI_AreaList
    {
        /// <summary>
        /// 行政區資料
        /// </summary>
        public List<ZipCodeData> ZipObj { set; get; }
    }
}