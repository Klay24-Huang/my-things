using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment.Param
{
    public class Body_CardFbinding : Body_AddCard
    {
  
        //身分證字號
        public string IdNo { get; set; }
        //生日(YYYYMMDD)
        public string Birthday { get; set; }
    }
}
