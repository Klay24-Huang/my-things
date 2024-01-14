using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All._2462._Total_Cost_to_Hire_K_Workers
{
    public class Solution
    {
        private class Item
        {
            public int Cost { get; set; }
            public int Index { get; set; }
        }

        private class PriorityComparere : IComparer<Item> 
        {
            public int Compare(Item? x, Item? y)
            {
                if (x.Cost != y.Cost) return x.Cost.CompareTo(y.Cost);
                else return x.Index.CompareTo(y.Index);
            }
        }

        public long TotalCost(int[] costs, int k, int candidates)
        {
            var len = costs.Length;
            var pq = new PriorityQueue<Item, Item>(new PriorityComparere());
            var l = 0;
            var r = len - 1;

            //for (var i = 0; i < candidates; i++)
            //{
            //    l = i;
            //    pq.Enqueue(l, costs[l]);
            //    var newR = len - 1 - i;
            //    if (newR >  candidates - 1)
            //    {
            //        r = newR;
            //        pq.Enqueue(r, costs[r]);
            //    }
            //}

            for (var i = 0; i < candidates; i++)
            {
                l = i;
                var item = new Item {
                    Index = l,
                    Cost = costs[l],
                };
                pq.Enqueue(item, item);
            }
            // // Console.WriteLine($"left index {l}, right index {r}, pq count {pq.Count}");
            for (var i = 0; i < candidates; i++)
            {
                var newR = len - 1 - i;
                if (newR <= candidates - 1)
                {
                    break;
                }

                r = newR;
                var item = new Item
                {
                    Index = r,
                    Cost = costs[r],
                };
                pq.Enqueue(item, item);

            }


            //PrintPQ(pq, costs)
            // // Console.WriteLine($"left index {l}, right index {r}, pq count {pq.Count}");
            long totalCost = 0;
            for (var i = 0; i < k; i++)
            {
                //// Console.WriteLine($"count of pq {pq.Count}");
                var hasItem = pq.TryDequeue(out var item, out _);
                if (hasItem)
                {
                    totalCost += item.Cost;
                    // // Console.WriteLine($"add: {item.Cost}, index {item.Index}, total cost {totalCost}");
                    //// Console.WriteLine($"{l} {r}");

                    // check if there remain item to enqueue
                    if (r - 1 > l)
                    {
                        Item newItem;
                        if (item.Index - r >= 0)
                        {
                            // from right
                            r--;
                            newItem = new Item
                            {
                                Index = r,
                                Cost = costs[r],
                            };
                        }
                        else
                        {
                            // from left
                            l++;
                            newItem = new Item
                            {
                                Index = l,
                                Cost = costs[l],
                            };
                        }
                        pq.Enqueue(newItem, newItem);
                    }
                }

                //// Console.WriteLine($"left index {l}, right index {r}");
            }
            return totalCost;
        }
    }
}
