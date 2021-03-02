using System;

namespace Domain.SP.Input.Car
{
    public class SPInput_GetCarMillage : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
    }
}