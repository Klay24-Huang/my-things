using System;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CheckAppVersion
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