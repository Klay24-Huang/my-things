using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.Question2
{
    public class Question2
    {
        public void Run()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5000000; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    GetNextId(i); //adjust 帶入i
                }));
            }
            Task.WaitAll(tasks.ToArray());

            //Console.ReadKey();
            // Console.WriteLine("currentId=" + GetCurrentId());
        }

        //private int currentId;
        //private static Mutex mutex = new Mutex();
        //public int GetNextId(int i)
        //{
        //    mutex.WaitOne();
        //        currentId = i; // currentId  存為 i
        //    mutex.ReleaseMutex();
        //        return currentId;
        //}


        private int currentId;
        private readonly object idLock = new object();
        public int GetNextId(int i)
        {
            lock (idLock)
            {
                currentId = i; // currentId  存為 i
                return currentId;
            }
        }

        public int GetCurrentId()
        {
            return currentId;
        }

    }
}
