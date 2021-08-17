using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;
using Domain.MemberData;
using Domain.SP.Output.Subscription;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_MonthlyPayInv
    {
        /// <summary>
        /// 開立結果
        /// </summary>
        public int PayInvResult { get; set; } = 0;
    }
}