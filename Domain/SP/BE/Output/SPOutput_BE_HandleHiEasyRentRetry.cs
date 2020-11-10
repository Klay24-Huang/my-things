using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_HandleHiEasyRentRetry:SPOutput_Base
    {
        /// <summary>
        /// 要傳送短租的模式
        /// <para>1:060</para>
        /// <para>2:125</para>
        /// <para>3:130</para>
        /// </summary>
        public int ReturnMode { set; get; }
    }
}
