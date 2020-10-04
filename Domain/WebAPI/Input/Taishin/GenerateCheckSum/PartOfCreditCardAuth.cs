using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.GenerateCheckSum
{
    public class PartOfCreditCardAuth
    {
        /// <summary>
        /// 版號
        /// </summary>
        public string ApiVer { set; get; }
        /// <summary>
        /// PAY+Id
        /// </summary>
        public string ApposId { set; get; }
        /// <summary>
        /// 16碼防偽隨機碼
        /// </summary>
        public string Random { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public AuthRequestParams RequestParams { set; get; }

        /// <summary>
        /// 電文產生時間
        /// </summary>
        public string TimeStamp { set; get; }
    }
}
