using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleFuncMaintain: IAPI_BE_Base
    {
        /// <summary>
        /// 功能群組id
        /// </summary>
        public string FuncGroupID { get; set; }
        /// <summary>
        /// 權限列表
        /// </summary>
        public List<Power> Power { get; set; }
        /// <summary>
        /// 模式
        /// <para>新增：Add</para>
        /// <para>修改：Edit</para>
        /// </summary>
        public string Mode { get; set; }
    }
    public class PowerList
    {
        /// <summary>
        /// 權限代碼
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 是否有權限
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int hasPower { get; set; }
    }

    public class Power
    {
        /// <summary>
        /// 父選單代碼
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 子選單代碼
        /// </summary>
        public string SubMenuCode { get; set; }
        /// <summary>
        /// 權限列表
        /// </summary>
        public List<PowerList> PowerList { get; set; }
    }

    public class MenuList
    {
        public string beMenuList { set; get; }
        public List<Power> PowerList { set; get; }
    }
}