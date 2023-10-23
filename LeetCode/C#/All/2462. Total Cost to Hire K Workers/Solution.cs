using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._2462._Total_Cost_to_Hire_K_Workers
{
    public class Solution
    {
        public long TotalCost(int[] costs, int k, int candidates)
        {
            var n = costs.Length;
            var pgFromStart = new PriorityQueue<int,int>();
            var pgFromEnd = new PriorityQueue<int,int>();
            for(var i = 0; i < k; i++)
            {
                pgFromStart.Enqueue(i, costs[i]);
                pgFromEnd.Enqueue(i, costs[n - 1 - i]);
            }

            long cost = 0;
            for (var i = 0; i < k; i++)
            {
                int i1 = -1;
                int c1 = -1;
                pgFromStart.TryPeek(out i1, out c1);

                int i2 = -1;
                int c2 = -1;
                pgFromEnd.TryPeek(out i2, out c2);
                
                if (i1< i2)
                {
                    cost += i1;
                    pgFromStart.Dequeue();
                }
            }

            return cost;
        }
    }
}
