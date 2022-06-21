using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_SavePassenger
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string MEMIDNO { get; set; }
        /// <summary>
        /// 副承租人姓名
        /// </summary>
        public string MEMCNAME { get; set; }
        /// <summary>
        /// 副承租人安心服務每小時金額
        /// </summary>
        public int InsurancePerHours { get; set; }
        /// <summary>
        /// 承租人安心服務費用
        /// </summary>
        public int InsuranceAMT { get; set; }
        /// <summary>
        /// 副承租人安心服務費用
        /// </summary>
        public int InsuranceOtherAMT { get; set; }
    }
}
