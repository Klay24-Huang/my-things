﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.FET
{
    public class SPInput_CarStatus
    {
        /// <summary>
        /// 車機類型
        /// <para>汽車：0</para>
        /// <para>機車：1</para>
        /// </summary>
        public int deviceType { get; set; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string deviceCID { get; set; }
        public int deviceACCStatus { get; set; }
        public int deviceGPSStatus { get; set; }
        public string deviceGPSTime { get; set; }
        public int deviceOBDstatus { get; set; }
        public int deviceGPRSStatus { get; set; }
        public int devicePowerONStatus { get; set; }
        public int devcieCentralLockStatus { get; set; }
        public string deviceDoorStatus { get; set; }
        public string deviceLockStatus { get; set; }
        public int deviceIndoorLightStatus { get; set; }
        public int deviceSecurityStatus { get; set; }
        public int deviceSpeed { get; set; }
        public double deviceVolt { get; set; }
        public double deviceLatitude { get; set; }
        public double deviceLongitude { get; set; }
        public int deviceMillage { get; set; }
        public int extDeviceStatus1 { get; set; }
        public int extDeviceStatus2 { get; set; }
        public string extDeviceData2 { get; set; }
        public string extDeviceData3 { get; set; }
        public string extDeviceData4 { get; set; }
        /// <summary>
        /// 讀卡
        /// </summary>
        public string extDeviceData7 { get; set; }
        public Int64 LogID { set; get; }
    }
}