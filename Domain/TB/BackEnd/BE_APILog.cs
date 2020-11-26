﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public  class BE_APILog
    {
        /// <summary>
        /// 呼叫時間
        /// </summary>
        public DateTime MKTime { set; get; }
        /// <summary>
        /// API中文名稱
        /// </summary>
        public string APICName { set; get; }
        /// <summary>
        /// API名稱
        /// </summary>
        public string APIName { set; get; }
        /// <summary>
        /// 呼叫內容
        /// </summary>
        public string APIInput { set; get; }
    }
}