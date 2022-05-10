using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CENS_ResponseInfo
    {
        public string CID { set; get; }
        public int OBDStatus { set; get; }
        public int GPSStatus { set; get; }
        public int GPRSStatus { set; get; }
        public int AccON { set; get; }
        public int PowON { set; get; }
        public double SPEED { set; get; }
        public double Milage { set; get; }
        public double Volt { set; get; }
        public double Lat { set; get; }
        public double Lng { set; get; }
        public string doorStatus { set; get; }
        public string lockStatus { set; get; }
        public int OrderStatus { set; get; }
        public int indoorLight { set; get; }
        public int SecurityStatus { set; get; }
        public int CentralLock { set; get; }
        public DateTime GPSTime { set; get; }
        public int iButton { set; get; }
        public string iButtonKey { set; get; }
        public string fwver { set; get; }
        public string CSQ { set; get; }
        public string retrycnt { set; get; }
        public float gaslvl { set; get; }
    }
}