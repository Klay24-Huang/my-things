using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class AuthItem
    {
        public string Amount { get; set; }
        public string Name { get; set; }
        public string NonPoint { get; set; }
        public string NonRedeem { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }
}
