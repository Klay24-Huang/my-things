using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._1456._Maximum_Number_of_Vowels_in_a_Substring_of_Given_Length
{
    public class Solution
    {
        public int MaxVowels(string s, int k)
        {
            var set = new HashSet<char>
            {
                'a', 'e', 'i','o','u'
            };

            var ans = 0;

            for (var i = 0; i < k; i++)
            {
                if (set.Contains(s[i]))
                {
                    ans++;
                }
            }

            var curr = ans;

            for (var i = k; i < s.Length; i++)
            {
                // prev
                if (set.Contains(s[i - k]))
                {
                    curr--;
                }

                // curr
                if (set.Contains(s[i]))
                {
                    curr++;
                }
                ans = Math.Max(curr, ans);
            }

            return ans;
        }
    }
}
