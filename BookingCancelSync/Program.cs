using Domain.SP.Output;
using Domain.Sync.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using WebCommon;

namespace BookingCancelSync
{
    class Program
    {
        private static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        //static System.Threading.Thread BookingCancelThread;
        //static System.Threading.ThreadStart ts = new System.Threading.ThreadStart(new Program().DoSyncBookingCancel);
        //private static int NowCount = 0;
        //private static int MaxCount = 10;
        static void Main(string[] args)
        {
            //BookingCancelThread = new System.Threading.Thread(ts);
            //BookingCancelThread.Start();
            Console.WriteLine("開始查詢取消清單");

            BookingCancel sync = new BookingCancel();
            sync.DoSyncBookingCancel();

        }

        private void checkThread()
        {
            /*
            Thread waitThread = new Thread(checkThread);
            if (false == BookingCancelThread.IsAlive)
            {
                BookingCancelThread = new Thread(ts);
                BookingCancelThread.Start();
                //  Console.WriteLine("重啟thread");
                if (waitThread.IsAlive)
                {
                    waitThread.Abort();
                }
            }
            else
            {
                // Thread.Sleep(1000);

                waitThread.Start();
            }
            */
        }
        
    }
}
