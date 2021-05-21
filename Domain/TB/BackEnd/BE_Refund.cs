using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_Refund
    {
        public string orderNo { set; get; }
        public string IDNO { set; get; }
        public string paymentNo { set; get; }
        public string orderTime { set; get; }
        public string endTime { set; get; }
        public string ITEM { set; get; }
        public string PRICE { set; get; }
        public string tax { set; get; }
        public string amount { set; get; }
        public string refunddate { set; get; }
    }
}
