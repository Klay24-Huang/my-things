using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherService.Common
{
    public class CreditCardPayInfo
    {
        /// <summary>
        /// 付費類型
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 付費類型中文描述
        /// </summary>
        public string PayTypeStr { get; set; }

        /// <summary>
        /// 付費類型英文代碼
        /// </summary>
        public string PayTypeCode { get; set; }

        public string FrontPart { get; set; }




    }
}
