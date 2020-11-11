using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_MenuCombind
    {
        /// <summary>
        /// 父選單pk
        /// </summary>
        public int MenuId { set; get; }
        /// <summary>
        /// 父選單名稱
        /// </summary>
        public string MenuName { set; get; }
        /// <summary>
        /// 父選單排序
        /// </summary>
        public string Sort { set; get; }
        /// <summary>
        /// 父選單代碼
        /// </summary>
        public string MenuCode { set; get; }
        /// <summary>
        /// 子選單列表
        /// </summary>
        public List<BE_SubMenu> lstSubMenu { set; get; }
    }
    public class BE_SubMenu
    {
        /// <summary>
        /// 子選單代碼
        /// </summary>
        public string SubMenuCode { set; get; }
        /// <summary>
        /// 子選單名稱
        /// </summary>
        public string SubMenuName { set; get; }
        /// <summary>
        /// 子選單對應mvc controller
        /// </summary>
        public string MenuController { set; get; }
        /// <summary>
        /// 子選單對應mvc action
        /// </summary>
        public string MenuAction { set; get; }
        /// <summary>
        /// 子選單排序
        /// </summary>
        public string SubMenuSort { set; get; }

        /// <summary>
        /// 是否開新視窗
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// <para>2:內頁</para>
        /// </summary>
        public string isNewWindow { set; get; }
        /// <summary>
        /// 權限id
        /// </summary>
        public string OperationPowerGroupId { set; get; }
    }
}
