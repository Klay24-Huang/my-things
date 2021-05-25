using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
    /// <summary>
    /// 執行usp_GetCityParkingFee回傳值
    /// </summary>
    public class SPOutput_CalCityParkingFee:SPOutput_Base
    {
        /// <summary>
        /// 回傳的停車費
        /// </summary>
        public Int32 ParkingFee { set; get; }
    }
}
