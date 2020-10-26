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
        public DateTime DeadLine { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        /// 出車據點
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 是否為興聯車機(0:否;1:是)
        /// </summary>
        public int IsCens { get; set; }

        /// <summary>
        /// 是否為機車（0:否;1:是)
        /// </summary>
        public int IsMotor { get; set; }

        /// <summary>
        /// 遠傳車機token
        /// </summary>
        public string deviceToken { get; set; }
    }
}