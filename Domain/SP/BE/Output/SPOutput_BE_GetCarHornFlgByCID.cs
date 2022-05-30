using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_GetCarHornFlgByCID : SPOutput_Base
    {
        /// <summary>
        /// 是否可以響喇叭
        /// </summary>
        public string CarHornFlg { get; set; }
    }
}
