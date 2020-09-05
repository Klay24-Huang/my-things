using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class ZipCodeData
    {
        /// <summary>
        /// 縣市代碼
        /// </summary>
        public int CityID { set; get; }
        /// <summary>
        /// 縣市名稱
        /// </summary>
        public string CityName { set; get; }
        /// <summary>
        /// 行政區代碼
        /// </summary>
        public int AreaID { set; get; }
        /// <summary>
        /// 行政區名稱
        /// </summary>
        public string AreaName { set; get; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string ZIPCode { set; get; }
    }
}
