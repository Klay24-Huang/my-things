using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_NPR136RetryNew: BE_NPR136Retry
    {
        /// <summary>
        /// 台新單號
        /// </summary>
        public string TaishinTradeNo { set; get; }
        /// <summary>
        /// 刷退單號
        /// </summary>
        public string TaishinRefundTradeNo { set; get; }
        /// <summary>
        /// 補繳單號
        /// </summary>
        public string ArrearTaishinTradeNo { set; get; }
    }
}
