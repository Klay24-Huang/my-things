using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetUserData
    {
        public int SEQNO         {set;get;}
        public string Account       {set;get;}
        public string UserName      {set;get;}
        public int Operator      {set;get;}
        public string OperatorName  {set;get;}
        public string UserGroup     {set;get;}
        public int UserGroupID   {set;get;}
        public string UserGroupName {set;get;}
        public string PowerList     {set;get;}
        public DateTime StartDate     {set;get;}
        public DateTime EndDate       {set;get; }
        public string area { set; get; }
        public string type { set; get; }
    }
}
