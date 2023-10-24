using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace All._2462._Total_Cost_to_Hire_K_Workers
{
    public class Solution
    {
        public long TotalCost(int[] costs, int k, int candidates)
        {
            var len = costs.Length;
            var pq = new PriorityQueue<int, int>();
            var l = 0;
            var r = len - 1;

            for (var i = 0; i < candidates; i++)
            {
                l = i;
                pq.Enqueue(l, costs[l]);
                var newR = len - 1 - i;
                if (newR >  candidates - 1)
                {
                    r = newR;
                    pq.Enqueue(r, costs[r]);
                }
            }
            //Console.WriteLine($"left index {l}, right index {r}");
            long totalCost = 0;
            for (var i = 0; i < k; i++)
            {
                var hasItem = pq.TryDequeue(out int index, out int cost);

                if (hasItem)
                {
                    totalCost += cost;
                    //Console.WriteLine($"add: {cost}, index {index}, total cost {totalCost}");
                    //Console.WriteLine($"{l} {r}");

                    // check if there remain item to enqueue
                    if (r - 1 > l)
                    {
                        if (index - r > 0)
                        {
                            // from right
                            r--;
                            pq.Enqueue(r, costs[r]);
                        }
                        else
                        {
                            // from left
                            l++;
                            pq.Enqueue(l, costs[l]);
                        }
                    }
                }
                //Console.WriteLine($"left index {l}, right index {r}");
            }
            return totalCost;
        }
    }
}
