using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
    public class SPOutput_CheckCanOpenDoor : SPOutput_Base
    {
        /// <summary>
        /// 使用期限
        /// </summary>
        public DateTime DeadLine { set; get; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { set; get; }
    }
}
