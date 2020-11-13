using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_GetUserGroupByOperator:IAPI_BE_Base
    {
        public int OperatorID { set; get; }
        public int NowID { set; get; }
        public int UserGroupID { set; get; }
    }
}