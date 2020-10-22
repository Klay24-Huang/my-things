using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class FeedBackKind
    {
        /// <summary>
        /// 星星數
        /// </summary>
        public int Star { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int FeedBackKindId { get; set; }
    }
}