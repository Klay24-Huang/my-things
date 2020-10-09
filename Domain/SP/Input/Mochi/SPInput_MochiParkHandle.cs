using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Mochi
{
    public class SPInput_MochiParkHandle:SPInput_Base
    {
        public string Id { set; get; }
        public string t_Operator { set; get; }
        public string Name { set; get; }
        public string cooperation_state { set; get; }
        public int price { set; get; }
        public string charge_mode { set; get; }
        public decimal lat { set; get; }
        public decimal lng { set; get; }
        public string open_status { set; get; }
        public string t_period { set; get; }
        public Int16 all_day_open { set; get; }
        public string detail { set; get; }
        public string city { set; get; }
        public string addr { set; get; }
        public string tel { set; get; }
        public string addUser { set; get; }
    }
}
