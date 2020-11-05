using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_AuditHistory
    {
        public int AuditHistoryID    {set;get;}
        public string UserName          {set;get;}
        public string MUserName         {set;get;}
        public string IDNO              {set;get;}
        public string AuditUser         {set;get;}
        public DateTime AuditDate         {set;get;}
        public Int16 HandleItem        {set;get;}
        public Int16 HandleType        {set;get;}
        public Int16 IsReject          {set;get;}
        public string RejectReason      {set;get;}
        public string RejectExplain { set; get; }
    }
}
