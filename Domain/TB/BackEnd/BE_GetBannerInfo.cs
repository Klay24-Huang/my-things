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
        public string SEQNO { set; get; } //設為int會出現null錯誤，沒找出為何，所以還是改string
    }
}
