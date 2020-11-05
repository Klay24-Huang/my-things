using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Bill
{
    /// <summary>
    /// 取安心服務價格
    /// </summary>
    public class SPInput_GetInsurancePrice
    {
        /// <summary>
        /// 會員代碼
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public long LogID { set; get; }
    }
}
