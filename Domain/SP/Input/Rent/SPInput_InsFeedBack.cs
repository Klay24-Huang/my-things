using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_InsFeedBack:SPInput_Base
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
        public int Mode { set; get; }
        public int Star { set; get; }
        public string FeedBackKind { set; get; }
        public string Descript { set; get; }
        public string Token { set; get; }
    }
}
