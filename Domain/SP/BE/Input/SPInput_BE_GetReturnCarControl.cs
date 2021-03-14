using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_GetReturnCarControl : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        public string UserID { set; get; }
    }
}