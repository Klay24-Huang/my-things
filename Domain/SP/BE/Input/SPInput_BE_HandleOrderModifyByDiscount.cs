using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleOrderModifyByDiscount:SPInput_Base
    {
        public Int64 OrderNo       {set;get;}
        public int CarPoint      {set;get;}
        public int MotorPoint    {set;get;}
        public int RNTAMT        {set;get;}
        public int FinalPrice    {set;get;}
        public int Reson         {set;get;}
        public string Remark        {set;get;}
        public string UserID { set; get; }
    }
}
