using Domain.TB.Maintain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Output
{
    public class OAPI_MA_CleanListQuery : GetBookingMainForMaintain
    {
        /// <summary>
        /// 是否可取消
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int CanCancel { set; get; }
        /// <summary>
        /// 是否可取車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int CanPick { set; get; }
        /// <summary>
        /// 是否已逾時
        /// </summary>
        public int isOverTime { set; get; }

    }
}