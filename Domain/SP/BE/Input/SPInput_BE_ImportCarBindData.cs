using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ImportCarBindData:SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// iButton Key
        /// </summary>
        public string iButtonKey { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
