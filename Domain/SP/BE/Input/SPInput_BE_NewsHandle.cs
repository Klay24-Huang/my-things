using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_NewsHandle:SPInput_Base
    {
        /// <summary>
        /// Mode>0時有值
        /// </summary>
        public int NewsID { set; get; }
        public string Title { set; get; }
        public Int16 NewsType { set; get; }
        public Int16 NewsClass { set; get; }
        public string Content { set; get; }
        public string URL { set; get; }
        public DateTime SD { set; get; }
        public DateTime ED { set; get; }
        /// <summary>
        /// 0:新增
        /// 1:修改
        /// 2:刪除
        /// </summary>
        public Int16 Mode { set; get; }
        public string BeTop{get;set;}
        public string UserID { set; get; }
    }
}
