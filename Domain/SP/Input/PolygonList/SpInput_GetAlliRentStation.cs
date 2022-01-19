using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.PolygonList
{
    public class SpInput_GetAlliRentStation
    {
        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double lat { get; set; } = 0;
        /// <summary>
        /// 經度
        /// </summary>
        public double lng { get; set; } = 0;
        /// <summary>
        /// 半徑
        /// </summary>
        public double radius { get; set; } = 0;
        /// <summary>
        /// 車型
        /// </summary>
        public string CarTypes { get; set; } = "";
        /// <summary>
        /// 座位數
        /// </summary>
        public string Seats { get; set; } = "";
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime? SD { get; set; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime? ED { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? SetNow { get; set; }
    }
}