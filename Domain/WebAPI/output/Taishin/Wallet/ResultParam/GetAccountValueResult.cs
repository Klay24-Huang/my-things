using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    public class GetAccountValueResult
    {
        /// <summary>
        /// 查詢區間資料總筆數
        /// </summary>
        /// <mark>301</mark>
        public int TotalCount { get; set; }
        /// <summary>
        /// 最後回傳資料筆數位置 (每次最多回傳200筆)
        /// </summary>
        /// <mark>200</mark>
        public int LastResponseCount { get; set; }
        public List<GetAccountValueResultDetail> Detail { get; set; }
    }
}
