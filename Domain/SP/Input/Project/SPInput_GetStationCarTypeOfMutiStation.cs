using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Project
{
    public class SPInput_GetStationCarTypeOfMutiStation
    {
        /// <summary>
        /// 據點代碼（1~多個）
        /// </summary>
        public string StationIDs { get; set; }
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SD { get; set; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime ED { get; set; }
        /// <summary>
        /// 車型群組代碼
        /// </summary>
        public string CarType { get; set; }
        /// <summary>
        /// 客戶代碼
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 是否使用安心服務
        /// </summary>
        public int Insurance { set; get; }
        public Int64 LogID { get; set; }
    }
}
