using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetBannerInfo
    {
        public int QUEUE { set; get; }
        public string URL { set; get; }
        public string MarqueeText { set; get; }
        public DateTime STARTDATE { set; get; }
        public DateTime ENDDATE { set; get; }
        public string PIC_NAME { set; get; } //命名要和sp吐回的一樣
        public string SEQNO { set; get; }
        public string Status { set; get; }
        public string STARTDATE2 { set; get; }
        public string ENDDATE2 { set; get; }
    }
}
