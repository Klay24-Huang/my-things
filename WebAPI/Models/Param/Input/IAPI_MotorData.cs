using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 由遠傳回傳機車Log
    /// </summary>
    public class IAPI_MotorData
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string deviceCID { get; set; }
        /// <summary>
        /// 車機類型
        /// <para>汽車：Vehicle</para>
        /// <para>機車：Motorcycle</para>
        /// </summary>
        public string deviceType { get; set; }
        public int deviceACCStatus { get; set; }
        public int deviceGPSStatus { get; set; }
        public int deviceGPRSStatus { get; set; }
        public string deviceGPSTime { get; set; }
        public int deviceSpeed { get; set; }
        public double deviceVolt { get; set; }
        public double deviceLatitude { get; set; }
        public double deviceLongitude { get; set; }
        public double deviceMillage { get; set; }
        public double deviceCourse { get; set; }
        public int deviceRPM { get; set; }
        public int deviceiSpeed { get; set; }
        public double device2TBA { get; set; }
        public double device3TBA { get; set; }
        public string deviceRSOC { get; set; }
        public string deviceRDistance { get; set; }
        public double deviceMBA { get; set; }
        public int deviceMBAA { get; set; }
        public int deviceMBAT_Hi { get; set; }
        public int deviceMBAT_Lo { get; set; }
        public double deviceRBA { get; set; }
        public int deviceRBAA { get; set; }
        public int deviceRBAT_Hi { get; set; }
        public int deviceRBAT_Lo { get; set; }
        public double deviceLBA { get; set; }
        public int deviceLBAA { get; set; }
        public int deviceLBAT_Hi { get; set; }
        public int deviceLBAT_Lo { get; set; }
        public int deviceTMP { get; set; }
        public int deviceCur { get; set; }
        public int deviceTPS { get; set; }
        public int deviceiVOL { get; set; }
        public int deviceErr { get; set; }
        public int deviceALT { get; set; }
        public double deviceGx { get; set; }
        public double deviceGy { get; set; }
        public double deviceGz { get; set; }
        public int deviceBLE_Login { get; set; }
        public int deviceBLE_BroadCast { get; set; }
        public int devicePwr_Mode { get; set; }
        public int deviceReversing { get; set; }
        public int devicePut_Down { get; set; }
        public int devicePwr_Relay { get; set; }
        public int deviceStart_OK { get; set; }
        public int deviceHard_ACC { get; set; }
        public int deviceEMG_Break { get; set; }
        public int deviceSharp_Turn { get; set; }
        public int deviceBat_Cover { get; set; }
        public int extDeviceStatus1 { get; set; }
        public string extDeviceDatat2 { get; set; }
    }
}