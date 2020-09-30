using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 手機條碼檢核輸出
    /// </summary>
    public class WebAPIOutput_EinvBiz
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// 檢核是否正確
        /// <para>Y:成功</para>
        /// <para>N:失敗</para>
        /// </summary>
        public string isExist { set; get; }

    }
}
