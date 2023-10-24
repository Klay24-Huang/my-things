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
            var n = costs.Length;
            var pqLeft = new PriorityQueue<int, int>();
            var pqRight = new PriorityQueue<int, int>();

            var l = 0;

            for (; l < candidates;)
            {
                pqLeft.Enqueue(l, costs[l]);
                l++;

            }

            var r = n - 1;
            for (var i = 0; i < candidates; i++)
            {
                if (l > r)
                {
                    break;
                }
                pqRight.Enqueue(r, costs[r]);
                r--;
            }

            long cost = 0;
            for (var i = 0; i < k; i++)
            {
                var rIndex = int.MaxValue;
                var rCost = int.MaxValue;
                var rHasItem = pqRight.TryPeek(out rIndex, out rCost);

                var lIndex = int.MaxValue;
                var lCost = int.MaxValue;
                var lHasItem = pqLeft.TryPeek(out lIndex, out lCost);

                Console.WriteLine();
                Console.WriteLine();
                if ((lHasItem && !rHasItem) || lCost < rCost || ((lCost == rCost) && (lIndex < rIndex)))
                {
                    cost += lCost;
                    pqLeft.Dequeue();
                    if (l <= r)
                    {
                        pqLeft.Enqueue(l, costs[l]);
                        Console.WriteLine($"enquue index {l}");
                        l++;
                    }
                }

                if ((rHasItem && !lHasItem) || rCost < lCost || ((rCost == lCost) && (rIndex < lIndex)))
                {
                    cost += rCost;
                    pqRight.Dequeue();
                    if (r >= l)
                    {
                        pqRight.Enqueue(r, costs[r]);
                        Console.WriteLine($"enquue index {r}");
                        r--;
                    }
                }
                Console.WriteLine($"r, index {rIndex}, cost {rCost}");

                Console.WriteLine($"l, index {lIndex}, cost {lCost}");
                Console.WriteLine($"after round. cost is {cost}");
            }
            return cost;
            //var n = costs.Length;
            //var pqFromStart = new PriorityQueue<int,int>();
            //var pqFromEnd = new PriorityQueue<int,int>();
            //for(var i = 0; i < k; i++)
            //{
            //    pqFromStart.Enqueue(i, costs[i]);
            //    pqFromEnd.Enqueue(i, costs[n - 1 - i]);
            //    pqFromStart.con
            //}

            //long cost = 0;
            //for (var i = 0; i < k; i++)
            //{
            //    int i1 = -1;
            //    int c1 = -1;
            //    pqFromStart.TryPeek(out i1, out c1);

            //    int i2 = -1;
            //    int c2 = -1;
            //    pqFromEnd.TryPeek(out i2, out c2);

            //    if (c1< c2)
            //    {
            //        cost += c1;
            //        pqFromStart.Dequeue();
            //        pqFromStart.Enqueue(i1 + 1, costs[i1 + 1]);
            //    }
            //    else
            //    {
            //        cost += c2;
            //        pqFromEnd.Dequeue();
            //        pqFromEnd.Enqueue(i2 + 1, costs[i2 + 1]);
            //    }
            //}

            //return cost;
        }
    }
}
