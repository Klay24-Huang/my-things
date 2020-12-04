using System;

namespace Domain.SP.Input.Car
{
    public class SPInput_CheckCariButton : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
    }
}