using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleUserMaintain : SPInput_Base
    {
        public int  SEQNO       {set;get;}
        public string  Mode        {set;get;}
        public int  Operator    {set;get;}
        public int  UserGroupID {set;get;}
        public string  UserAccount {set;get;}
        public string  UserPWD     {set;get;}
        public string  UserName    {set;get;}
        public DateTime  StartDate   {set;get;}
        public DateTime  EndDate     {set;get;}
        public string  PowerStr    {set;get;}
        public string  UserID      {set;get;}
        public string AreaV { set; get; }//20210209唐加
        public string mtypeV { set; get; }//20210209唐加
        public string Area { set; get; }
        public string Type { set; get; }
    }
}
