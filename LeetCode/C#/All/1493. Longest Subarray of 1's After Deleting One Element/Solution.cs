using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace All._1493._Longest_Subarray_of_1_s_After_Deleting_One_Element
{
    public class Solution
    {
        public int LongestSubarray(int[] nums)
        {
            var ans = 0;
            var holder = new List<int>();
            var add  = (int num) => { 
                holder.Add(num);
                if (holder.Count > 3)
                {
                    holder.RemoveAt(0);
                }
            };

            var getAns = () =>
            {
                Console.WriteLine(JsonSerializer.Serialize(holder));
                var r = holder.Sum();
                ans = r > ans ? r : ans;
            };

            var currLen = 0;
            foreach (var item in nums)
            {
                if (item == 0)
                {
                    if(currLen > 0)
                    {
                        add(currLen);
                        getAns();
                        currLen = 0;
                    }
                    add(0);
                    getAns();
                }
                else
                {
                    // is one
                    currLen++;
                }
            }

            if(currLen > 0)
            {
                add(currLen);
                getAns();
            }
            
            return ans;
        }
    }
}
