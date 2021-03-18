using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleCarBindData:SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// iButton
        /// </summary>
        public string iButtonKey { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
