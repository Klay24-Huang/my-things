using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleCarOnline:SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 是否上線
        /// <para>1:上線</para>
        /// <para>2:待上線</para>
        /// </summary>
        public int Online { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 下線原因
        /// </summary>
        public string OffLineReason { get; set; }
    }
}
