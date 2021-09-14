using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_BannerSort : SPInput_Base
    {
        public int SEQNO { set; get; }
        public string Queue { set; get; }
        public string UserID { set; get; }
    }
}
