using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class CarData
    {
        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 目前所在據點
        /// </summary>
        public string StationID { set; get; }
    }
}
