using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_DelSubsHist
    {
        /// <summary>
        /// 刪除結果
        /// <para>0:失敗</para>
        /// <para>1:成功</para>
        /// </summary>
        public int DelResult { get; set; } = 0;
    }
}