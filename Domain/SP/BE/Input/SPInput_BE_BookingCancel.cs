using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_BookingCancel : SPInput_Base
    {
        public Int64 OrderNo { set; get; }
        public string UserID { set; get; }
    }
}
