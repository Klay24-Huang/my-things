using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Params.Search.Input
{
    public class Input_CarCurrentStatus
    {
        public string Result { get; set; }
        public Input_CarCurrentStatus_Data Data { get; set; }
    }

    public class Input_CarCurrentStatus_Data
    {
        public List<BE_CarCurrentStatus> lstData { get; set; }
    }
}