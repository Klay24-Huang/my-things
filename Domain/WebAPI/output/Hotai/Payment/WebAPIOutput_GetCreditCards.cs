using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_GetCreditCards
    {
        public List<HotaiCardInfo> HotaiCards { get; set; }
        public int CardCount { get; set; }

    }


}
