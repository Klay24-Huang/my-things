using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Output.CENS 
{
    /// <summary>
    /// 
    /// </summary>
    public class WSOutput_Base
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 錯誤代碼
        /// <para>000000:成功</para>
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 是否成功執行
        /// <para>0:成功</para>
        /// <para>1:失敗</para>
        /// </summary>
        public int Result { get; set; }
    }
}
