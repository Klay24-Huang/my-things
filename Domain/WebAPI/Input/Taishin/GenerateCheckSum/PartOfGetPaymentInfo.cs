using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.GenerateCheckSum
{
   public class PartOfGetPaymentInfo
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public string Random { get; set; }
        public IGetPaymentInfoRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }
        public string TransNo { get; set; }
   
  
    
    }
}
