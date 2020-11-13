using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleUserMaintain:IAPI_BE_Base
    {
        public int SEQNO { set; get; }
        public string Mode { set; get; }
        public int Operator { set; get; }
        public int UserGroupID { set; get; }
        public string UserAccount { set; get; }
        public string UserPWD { set; get; }
        public string UserName { set; get; }
        public string StartDate { set; get; }
        public string EndDate { set; get; }

        public List<Power> Power { get; set; }

    }
}