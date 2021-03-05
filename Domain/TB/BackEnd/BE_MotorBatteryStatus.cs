using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 機車電池狀態
    /// </summary>
    public class BE_MotorBatteryStatus
    {
        public string CarNo { set; get; }
        public string CID { set; get; }
        public double Volt { set; get; }
        public double device2TBA { set; get; }
        public double device3TBA { set; get; }
        public string deviceRSOC { set; get; }
        public double deviceMBA { set; get; }
        public double deviceMBAA { set; get; }
        public double deviceMBAT_Hi { set; get; }
        public double deviceMBAT_Lo { set; get; }
        public double deviceRBA { set; get; }
        public double deviceRBAA { set; get; }
        public double deviceRBAT_Hi { set; get; }
        public double deviceRBAT_Lo { set; get; }
        public double deviceLBA { set; get; }
        public double deviceLBAA { set; get; }
        public double deviceLBAT_Hi { set; get; }
        public double deviceLBAT_Lo { set; get; }
        public DateTime MKTime { set; get; }
    }
}
