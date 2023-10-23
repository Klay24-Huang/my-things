using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._2542._Maximum_Subsequence_Score
{
    public class Solution
    {
        public long MaxScore(int[] nums1, int[] nums2, int k)
        {
            Array.Sort(nums2, nums1);
            //long ans = 0;

            long ans = 0;
            void action(ref long ans, int index, int[] sub1, int[] sub2)
            {
                for (var i = index; i < nums1.Length; i++)
                {
                    var newSub1 = sub1;
                    var newSub2 = sub2;
                    if (newSub1.Length < k)
                    {
                        newSub1 = newSub1.Append(nums1[i]).ToArray();
                        newSub2 = newSub2.Append(nums2[i]).ToArray();

                        if (newSub1.Length == k)
                        {
                            var r = newSub1.Sum() * newSub2.Min();
                            Console.WriteLine(r);
                            if (r > ans)
                                ans = r;
                        }
                    }

                    action(ref ans, i+1, newSub1, newSub2);
                }
            }

            action(ref ans, 0, Array.Empty<int>(), Array.Empty<int>());

            return ans;
        }
    }
}
