using System;

namespace Domain.SP.Input.Common
{
    public class SPInput_CheckAppVersion : SPInput_Base
    {
        /// <summary>
        /// DeviceID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// APP類型
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16? APP { get; set; }

        /// <summary>
        /// APP版號
        /// </summary>
        public string APPVersion { get; set; }
    }
}