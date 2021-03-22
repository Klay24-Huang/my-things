using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class GetProject_SeatGroup
    {
        public Int16 Seat { get; set; }
        public List<GetProject_CarInfo> CarInfos { get; set; } = new List<GetProject_CarInfo>();
    }

    public class GetProject_CarInfo
    {
        public Int16 Seat { get; set; }
        public string CarType { get; set; }
        public string CarTypePic { get; set; }
        public string CarTypeName { get; set; }
    }

}