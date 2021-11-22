using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_PaymentBase
	{
		public int result { get; set; }
		public string ErrorCode { get; set; }
		public int NeedRelogin { get; set; }
		public int NeedUpgrade { get; set; }
		public string ErrorMessage { get; set; }


	}
}
