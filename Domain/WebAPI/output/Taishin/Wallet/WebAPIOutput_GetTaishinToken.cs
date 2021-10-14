using Domain.WebAPI.output.Taishin.Wallet.ResultParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_GetTaishinCvsPayToken
    {
        /// <summary>
        /// JWT Token，執行各API時帶入HTTP Header之Authorization 欄位bearer 後，以驗證權限
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 皆為bearer
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// Token有效秒數
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// Token有效範圍
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// 核發Token的APIM node
        /// </summary>
        public string node { get; set; }

        /// <summary>
        /// JWT Token ID
        /// </summary>
        public string jti { get; set; }


        public string Message { get; set; }
        public string ReturnCode { get; set; }

    }
}
