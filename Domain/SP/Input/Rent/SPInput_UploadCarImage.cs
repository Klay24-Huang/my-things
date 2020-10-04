using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_UploadCarImage:SPInput_Base
    {
       public string  IDNO { set; get; }
       public string  OrderNo { set; get; }
        public string  Token { set; get; }
        public Int16  Mode { set; get; }
        public Int16  CarImageType { set; get; }
        public string  CarImage { set; get; }
    }
}
