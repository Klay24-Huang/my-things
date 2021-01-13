using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetOrderModifyDataNewV2: BE_GetOrderModifyDataNew
    {
        public string TaishinTradeNo { set; get; }
        public int ArrearAMT { set; get; }
    }
}
