using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_ReadCard
    {
        /// <summary>
        /// 是否綁定
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int HasBind { set; get; }
    }
}