using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_CheckUserGroup:IAPI_BE_Base
    {
        public string UserGroupID { set; get; }
        public int OperatorID { set; get; }
    }
}