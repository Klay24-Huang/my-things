using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output
{
    /// <summary>
    /// Stored Procdure Base Output
    /// </summary>
    public class SPOutput_Base
    {
        /// <summary>
        /// 是否有錯誤
        /// <para>0:否(成功)</para>
        /// <para>1:是(失敗)</para>
        /// </summary>
        public int Error { set; get; }
        /// <summary>
        /// 錯誤碼
        /// </summary>
        public string ErrorCode { set; get; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMsg { set; get; }
        /// <summary>
        /// SQL Exception Code
        /// </summary>
        public string SQLExceptionCode { set; get; }
        /// <summary>
        /// SQL Exception Message
        /// </summary>
        public string SQLExceptionMsg { set; get; }
    }
}
