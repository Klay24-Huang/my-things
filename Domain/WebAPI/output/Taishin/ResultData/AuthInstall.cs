using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.ResultData
{
    public class AuthInstall
    {
        public string InstallPeriod { get; set; }
        public string InstallDownPay { get; set; }
        public string InstallPay { get; set; }
        public string InstallDownPayFee { get; set; }
        public string InstallPayFee { get; set; }
    }
}
