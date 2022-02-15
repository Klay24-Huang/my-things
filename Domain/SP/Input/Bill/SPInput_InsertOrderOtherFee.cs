using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Bill
{
    public class SPInput_InsertOrderOtherFee : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int IRENTORDNO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CNTRNO { get; set; }
        /// <summary>
        /// 車輛調度費
        /// </summary>
        public int CarDispatch { get; set; }
        /// <summary>
        /// 車輛調度費備註
        /// </summary>
        public string DispatchRemark { get; set; }
        /// <summary>
        /// 非配合停車費
        /// </summary>
        public int ParkingFee { get; set; }
        /// <summary>
        /// 非配合停車費備註
        /// </summary>
        public string ParkingFeeRemark { get; set; }
        /// <summary>
        /// 操作人員
        /// </summary>
        public string UserID { get; set; }

    }
}
