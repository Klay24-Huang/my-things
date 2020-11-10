using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Bill
{
    public class SPOutput_GetInsurancePrice 
    {
        /// <summary>
        /// 安心服務級距
        /// </summary>
        public int InsuranceLevel { set; get; }
        /// <summary>
        /// 安心服務每小時價格
        /// </summary>
        public float InsurancePerHours { set; get; }
    }
}
