using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.HiEasyRent
{
    public class SPInput_HandleNPR340Save : SPInput_Base
    {
        public string CUSTID        {set;get;}
        public string ORDNO         {set;get;}
        public string CNTRNO        {set;get;}
        public string PAYMENTTYPE   {set;get;}
        public string CARNO         {set;get;}
        public string AMOUNT        {set;get;}
        public string POLNO { set; get; }
    }
}
