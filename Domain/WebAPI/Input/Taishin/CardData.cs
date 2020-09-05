using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
   public  class CardData
    {
        public string MemberId { get; set; }
        public string CellPhone { get; set; }
        public string CardNumber { get; set; }
        public string ExpDate { get; set; }
        public string Cvv2 { get; set; }
        public string CardName { get; set; }
    }
}
