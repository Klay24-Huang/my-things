using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 功能權限名稱
    /// </summary>
    public class BE_PowerListCombind
    {
        public int OperationPowerGroupId { set; get; }

        public List<BE_PowerListCombindSubData> lstPowerFunc { set; get; }
    }
    public class BE_PowerListCombindSubData
    {
        /// <summary>
        /// 代碼
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string OPName { set; get; }
    }
}
