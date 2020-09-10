using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 取出專案基本資料
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID {set;get;}
        /// <summary>
        /// 專案類型
        /// <para>0:同站</para>
        /// <para>3:路邊</para>
        /// <para>4:機車</para>
        /// </summary>
        public int PROJTYPE { set;get;}
        /// <summary>
        /// 計費模式
        /// <para>0:以時計費</para>
        /// <para>1:以分計費</para>
        /// </summary>
        public int PayMode { set;get;}
        /// <summary>
        /// 是否是月租
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMonthRent { set;get;}
    }
}
