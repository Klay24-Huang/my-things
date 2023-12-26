using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.GetSameValues
{
    public class SameValues
    {
        /// <summary>
        /// 實作副程式 GetSameValues， 取得數字同時出現在 itemsA ，也出現在 itemsB的陣列
        /// </summary>
        public void Run()
        {
            Random seed = new Random();
            int[] itemsA = new int[20000];
            int[] itemsB = new int[20000];
            for (int i = 0; i < 20000; i++)
            {
                itemsA[i] = seed.Next(10000);
                itemsB[i] = seed.Next(10000);
            }

            DateTime now = DateTime.Now;
            // Console.WriteLine("同時出現在2個陣列的數字有 {0} 筆, 費時 {1:0.00}秒",
                GetSameValues(itemsA, itemsB).Length, (DateTime.Now - now).TotalSeconds);
        }

        public int[] GetSameValues(int[] itemsA, int[] itemsB)
        {
            return itemsA.Where(x => itemsB.Contains(x)).ToArray();
        }

    }
}
