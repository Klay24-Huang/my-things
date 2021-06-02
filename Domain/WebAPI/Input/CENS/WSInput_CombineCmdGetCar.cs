using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 取車巨集指令
    /// </summary>
    public class WSInput_CombineCmdGetCar : WSInput_Base
    {
        /// <summary>
        /// 卡號集合
        /// </summary>
        public SendCarNoData[] data { set; get; }
        public class SendCarNoData
        {
            /// <summary>
            /// 卡號
            /// </summary>
            public string CardNo { set; get; }
        }
    }
}
