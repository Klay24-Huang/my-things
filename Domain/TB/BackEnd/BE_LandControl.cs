using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_LandControl
    {
       public string  PROCD      {set;get;}
       public string  ORDNO      {set;get;}
       public Int64 IRENTORDNO {set;get;}
       public string  CUSTID     {set;get;}
       public string  CUSTNM     {set;get;}
       public string  BIRTH      {set;get;}
       public string  CARTYPE    {set;get;}
       public string  CARNO      {set;get;}
       public string  TSEQNO     {set;get;}
       public string  GIVEDATE   {set;get;}
       public string  GIVETIME   {set;get;}
       public Single  RENTDAYS   {set;get;}
       public Single  GIVEKM     {set;get;}
       public string  OUTBRNHCD  {set;get;}
       public string  RNTDATE    {set;get;}
       public string  RNTTIME    {set;get;}
       public string  INBRNHCD   {set;get;}
       public int  RPRICE     {set;get;}
       public int  RINSU      {set;get;}
       public int OVERHOURS  {set;get;}
       public int OVERAMT2   {set;get;}
       public int ORDAMT     {set;get;}
       public int RENTAMT    {set;get;}
       public int LOSSAMT2   {set;get;}
       public string  PROJID     {set;get;}
       public string  XID        {set;get;}
       public Int16  INVKIND    {set;get;}
       public string  UNIMNO     {set;get;}
       public string  INVTITLE   {set;get;}
       public string INVADDR { set; get; }
       public string  FinalStartTime { set; get; }
        public string  BookingStopTime { set; get; }
        public string  CARRIERID { set; get; }
        public string  NPOBAN { set; get; }
        public int  NOCAMT { set; get; }
    }
}
