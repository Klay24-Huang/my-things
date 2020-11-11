using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台-使用者群組列表
    /// </summary>
   public  class BE_UserGroup
    {
            public int USEQNO { set; get; }   //int
            public string UserGroupID    {set;get;}//  varchar
            public string UserGroupName  {set;get;}  //nvarchar
            public string OperatorID     {set;get;}  //int
            public DateTime StartDate      {set;get;}  //datetime
            public DateTime EndDate        {set;get;}  //datetime
            public string OperatorName   {set;get;}  //nvarchar
    }
}
