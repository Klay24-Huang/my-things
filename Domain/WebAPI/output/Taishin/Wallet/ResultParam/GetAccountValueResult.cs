using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    public class GetAccountValueResult
    {
        public int TotalCount { get; set; }
        public int LastResponseCount { get; set; }
        public List<GetAccountValueResultDetail> Detail { get; set; }
    }
}
