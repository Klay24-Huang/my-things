using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Mochi
{
    public partial class WebAPIOutput_ParkData
    {
        public Data data { get; set; }
    }
    public partial class Data
    {
        public List<Parkinglot> Parkinglots { get; set; }
    }

    public partial class Parkinglot
    {
        public string id { get; set; }
        public string name { get; set; }
        public string cooperation_state { get; set; }
        public CurrentPrice current_price { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public Detail detail { get; set; }
    }

    public partial class CurrentPrice
    {
        public double? price { get; set; }
        public string charge_mode { get; set; }
    }

    public partial class Detail
    {
        public OpeningTime Opening_Time { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
    }

    public partial class OpeningTime
    {
        public string status { get; set; }
        public string period { get; set; }
        public bool all_day_open { get; set; }
        public List<string> detail { get; set; }
    }
}
