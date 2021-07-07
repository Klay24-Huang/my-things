using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 遠傳回傳命令執行結果
    /// </summary>
    public class IAPI_ReceiveCMDBase
    {
        /// <summary>
        /// 回傳結果
        /// <para>Okay:成功</para>
        /// <para>NotOkay:失敗</para>
        /// </summary>
        public string CmdReply { get; set; }
        /// <summary>
        /// pk
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// NotOkay出錯步驟
        /// </summary>
        public string CmdStep { get; set; }
    }
}