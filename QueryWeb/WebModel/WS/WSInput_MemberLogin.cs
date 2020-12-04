using System;
using System.Runtime.CompilerServices;

namespace WebModel
{
    public class WSOutput_Base
    {
        public string msg
        {
            get;
            set;
        }

        public string result
        {
            get;
            set;
        }

        public long total
        {
            get;
            set;
        }

        public WSOutput_Base()
        {
        }
    }
}