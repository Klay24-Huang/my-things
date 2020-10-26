using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Car
{
    public class SPInput_GetStationCarType: SPInput_Base
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { get; set; }

        public DateTime SD { get; set; }

        public DateTime ED { get; set; }
    }
}
