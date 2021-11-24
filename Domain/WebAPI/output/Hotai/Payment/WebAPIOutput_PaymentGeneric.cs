using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_PaymentGeneric<TData> where TData : class
	{
		public bool success { get; set; }
		public string error { get; set; }
		public string errorMsg { get; set; }
		public TData Data { get; set; }
		


	}
}
