using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_UploadFeedBackImage : SPInput_Base
    {
       public string  IDNO        {set;get;}
       public Int64  OrderNo     {set;get;}
       public string  Token       {set;get;}
       public Int16  SEQNO       {set;get;}
       public string  FeedBackFile{set;get;}
    }
}
