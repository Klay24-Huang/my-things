using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class BindResponseParams
    {
        /// <summary>
        /// 
        /// </summary>
        public string ResultCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResultMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BindResultData ResultData { get; set; }
    }
}
