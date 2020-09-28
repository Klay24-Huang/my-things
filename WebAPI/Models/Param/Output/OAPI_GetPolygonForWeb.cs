using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetPolygonForWeb
    {
        public List<PolygonDataForWeb> PolygonObj { set; get; }
    }
}