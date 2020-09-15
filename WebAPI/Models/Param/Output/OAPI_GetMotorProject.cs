using Domain.WebAPI.output.rootAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMotorProject
    {
        public List<MotorProjectObj> GetMotorProjectObj { set; get; }
    }
}