using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_GetUserGroupByOperator
    {
        public List<BE_UserGroup> UserGroup { set; get; }
        public int NowID { set; get; }
        public int UserGroupID { set; get; }
    }
}