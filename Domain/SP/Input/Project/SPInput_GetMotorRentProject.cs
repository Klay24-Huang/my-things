using System;

namespace Domain.SP.Input.Project
{
    public class SPInput_GetMotorRentProject : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 預約起日
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 預約迄日
        /// </summary>
        public DateTime ED { get; set; }
    }
}