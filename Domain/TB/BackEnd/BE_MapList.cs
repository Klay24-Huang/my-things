using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_MapList
    {
        /// <summary>
        /// 員工代號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 員工姓名
        /// </summary>
        public string GPSTime { set; get; }
        /// <summary>
        /// 區域
        /// </summary>
        public string Latitude { set; get; }
        /// <summary>
        /// 種類
        /// </summary>
        public string Longitude { set; get; }
    }
}
