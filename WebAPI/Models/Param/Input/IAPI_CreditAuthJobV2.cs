using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CreditAuthJobV2
    {
        
        /// <summary>
        /// 發送通道
        /// </summary>
        public int GateNo { get; set; }
        /// <summary>
        /// Retry(0:一般 1:重試)
        /// </summary>
        public int isRetry { get; set; }
    }
}