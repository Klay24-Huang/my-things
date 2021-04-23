using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarSettingRecord
    {
        public DateTime Time { get; set; }
        public string Station { get; set; }
        public int Total { get; set; }
        public int Renting { get; set; }
        public int OnBoard { get; set; }
        public int OffBoard { get; set; }
        public int LowBattery_3TBA { get; set; }
        public int LowBattery_2TBA { get; set; }
        public int Nonresponse_OneHour { get; set; }
        public int Nonresponse { get; set; }
    }
}
