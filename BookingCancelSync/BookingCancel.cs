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
    public class BookingCancel
    {
        private static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        public BookingCancel()
        {

        }

        public void DoSyncBookingCancel()
        {
            /*if (NowCount >= MaxCount)
            {
                Console.WriteLine("休息一分鐘開始");
                Thread.Sleep(60000);
                Console.WriteLine("休息一分鐘結束");
                NowCount = 0;
            }*/
            try
            {
                string spName = "usp_SYNC_BookingCancel";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                SPInput_SYNC_BookingCancel spInput = new SPInput_SYNC_BookingCancel()
                {
                    NowTime = DateTime.Now.AddHours(8)
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SYNC_BookingCancel, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SYNC_BookingCancel, SPOutput_Base>(connetStr);
                bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                /*
                Console.WriteLine("休息五秒鐘開始");
                Thread.Sleep(5000);
                if (NowCount >= MaxCount)
                {
                    NowCount = 0;
                }
                else
                {
                    NowCount++;
                }
                Console.WriteLine("休息五秒鐘結束，再次啟動");
                checkThread();
                */
            }
        }
    }
}
