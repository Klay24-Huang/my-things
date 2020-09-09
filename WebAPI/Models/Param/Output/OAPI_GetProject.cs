using Domain.WebAPI.output.rootAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 取得專案及資費(資費試算)輸出
    /// </summary>
    public class OAPI_GetProject
    {
       public List<GetProjectObj> GetProjectObj { set; get; }
    }
}