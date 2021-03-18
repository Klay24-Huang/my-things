using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarInfoForMachineData
    {
        public string CarNo { set; get; }
        public string CID { set; get; }
        public string deviceId { set; get; }
        public string deviceToken { set; get; }
        public int IsCens { set; get; }
        public int IsMotor { set; get; }
        public int HasIButton { set; get; }
    }
}
