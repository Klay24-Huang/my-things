using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarData
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        public string StationID { set; get; }
        public string TSEQNO { set; get; }
        public string nowStationID { set; get; }
        public string CarType { set; get; }
        public string CarOfArea { set; get; }
        public string Operator { set; get; }
        public string NowOrderNo { set; get; }
        public string LastOrderNo { set; get; }
        public string available { set; get; }
        public string last_Opt { set; get; }
        public string MKTime { set; get; }
        public string UPDTime { set; get; }
    }
}
