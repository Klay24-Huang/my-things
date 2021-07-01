using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 遠傳車機版本資訊
    /// </summary>
    public class IAPI_CarMachineVerInfo
    {
        public string deviceType { get; set; }
        public string deviceName { get; set; }
        public string deviceFW { get; set; }
        public string deviceCID { get; set; }
        public string deviceBrandName { get; set; }
        public string deviceModelName { get; set; }
        public string deviceIMEI { get; set; }
        public string deviceICCID { get; set; }
    }
}