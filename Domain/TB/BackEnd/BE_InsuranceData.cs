using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public class BE_InsuranceData
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CarTypeGroupCode { set; get; }
        /// <summary>
        /// 層級
        /// </summary>
        public string InsuranceLevel { set; get; }
        /// <summary>
        /// 安心保險金額
        /// </summary>
        public string InsurancePerHours { set; get; }
    }
}
