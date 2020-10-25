using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_CheckOperator:IAPI_BE_Base
    {
        /// <summary>
        /// 加盟業者統編
        /// </summary>
        public string Operator { set; get; }
    }
}