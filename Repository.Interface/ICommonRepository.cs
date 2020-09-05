using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    /// <summary>
    /// 共用類型Interface
    /// </summary>
    public interface ICommonRepository
    {
        List<ErrorMessageList> GetErrorList(string ErrCode);


    }
}
