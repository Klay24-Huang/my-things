using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMemberScoreItem
    {
        public List<BE_GetMemberScoreItem> datalst { get; set; }
    }
}