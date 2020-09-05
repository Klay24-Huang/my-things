using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 縣市資料
    /// </summary>
   public  class CityData
    {
        /// <summary>
        /// 縣市代碼
        /// </summary>
        public int CityID { set; get; }
        /// <summary>
        /// 縣市名稱
        /// </summary>
        public string CityName { set; get; }
    }
}
