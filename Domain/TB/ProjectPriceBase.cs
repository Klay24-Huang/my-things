using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class ProjectPriceBase
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID { set; get; }

        /// <summary>
        /// 專案平日價
        /// </summary>
        public int Price { set; get; }
        /// <summary>
        /// 專案假日價
        /// </summary>
        public int PRICE_H { set; get; }
    }
}
