using Domain.WebAPI.output.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet.ResultParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_GetBarCode : WebAPIOutput_Base
    {
        public IHUBResHeader header { get; set; }
        public BarcodeRes body { get; set; }

        
    }
}
