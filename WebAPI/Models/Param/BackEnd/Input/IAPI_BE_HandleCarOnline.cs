using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleCarOnline:IAPI_BE_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 上線狀態
        /// <para>1:上線</para>
        /// <para>2:待上線(下線)</para>
        /// </summary>
        public int Online { set; get; }
        /// <summary>
        /// 下線原因
        /// </summary>
        public string OffLineReason { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { get; set; }
    }
}