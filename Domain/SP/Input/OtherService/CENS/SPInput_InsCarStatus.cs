using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.CENS
{
    public class SPInput_InsCarStatus
    {
        public string MachineNo { set; get; }
        public int OBDStatus { set; get; }
        public int GPSStatus { set; get; }
        public int GPRSStatus { set; get; }
        public int AccON { set; get; }
        public int PowON { set; get; }
        public string LockStatus { set; get; }
        public int LightStatus { set; get; }
        public int SecurityStatus { set; get; }
        public int CentralLock { set; get; }
        public int LowPowStatus { set; get; }
        public string DoorStatus { set; get; }
        public float SPEED { set; get; }
        public float Milage { set; get; }
        public float Volt { set; get; }
        public decimal Lat { set; get; }
        public decimal Lng { set; get; }
        public DateTime GPSTime { set; get; }
        public int iButton { set; get; }
        public string iButtonKey { set; get; }
        public int OrderStatus { set; get; }
        public string fwver { set; get; }
        public Int64 LogID { set; get; }
        public string CSQ { set; get; }
        public string retrycnt { set; get; }
        public float gaslvl { set; get; }

    }
}
