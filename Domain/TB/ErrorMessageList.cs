using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class ErrorMessageList
    {
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public string ErrCode { set; get; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { set; get; }
    }
}
