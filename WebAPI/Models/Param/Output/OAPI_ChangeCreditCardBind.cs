using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_ChangeCreditCardBind
    {
        /// <summary>
        /// 綁卡成功
        /// </summary>
        public string SuccessURL { set; get; }
        /// <summary>
        /// 綁卡失敗
        /// </summary>
        public string FailURL { set; get; }
        /// <summary>
        /// 綁卡網址
        /// </summary>
        public string CardAuthURL { set; get; }
    }
}