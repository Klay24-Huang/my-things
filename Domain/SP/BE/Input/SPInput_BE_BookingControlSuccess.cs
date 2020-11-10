using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_BookingControlSuccess:SPInput_Base
    {
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 短租回傳的訂單編號
        /// </summary>
        public string ORDNO { set; get; }
        /// <summary>
        /// 是否成功
        /// <para>0:失敗</para>
        /// <para>1:成功</para>
        /// </summary>
        public Int16 IsSuccess { set; get; }
    }
}
