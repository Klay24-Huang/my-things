using System;

namespace Domain.SP.Input.Car
{
    public class SPInput_SetMotorStatus : SPInput_Base
    {
        /// <summary>
        /// 車機號碼
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 指令
        /// </summary>
        public string CmdType { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public long LogID { set; get; }
    }
}
