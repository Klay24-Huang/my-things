using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public class BE_SameMobileData
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 縣市
        /// </summary>
        public string CityName { set; get; }
        /// <summary>
        /// 行政區
        /// </summary>
        public string AreaName { set; get; }
        /// <summary>
        /// 聯絡地址
        /// </summary>
        public string MEMADDR { set; get; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string MEMTEL { set; get; }
    }
}
