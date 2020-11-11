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
    public class BE_PowerList
    {
        public int OperationPowerID      {set;get;}
        public int OperationPowerGroupId {set;get;}
        public string Code                  {set;get;}
        public string OPName                { set; get; }
    }
}
