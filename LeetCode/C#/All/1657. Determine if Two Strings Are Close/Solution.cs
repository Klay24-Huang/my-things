using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All._1657._Determine_if_Two_Strings_Are_Close
{
    public class Solution
    {
        public bool CloseStrings(string word1, string word2)
        {
            if (word1.Length != word2.Length) return false;

            var arr1 = word1.ToArray();
            var arr2 = word2.ToArray();
            Array.Sort(arr1);
            Array.Sort(arr2);

            var s1 = new HashSet<char>();
            var s2 = new HashSet<char>();

            var l1 = new List<int>();
            var l2 = new List<int>();

            var prev1 = new char();
            var prev2 = new char();

            var count1 = 0;
            var count2 = 0; 

            for (var i  = 0; i < arr1.Length; i++)
            {
                var char1 = arr1[i];
                if (!s1.Contains(char1))
                {
                    s1.Add(char1);
                }

                if (char1 != prev1)
                {
                    l1.Add(count1);
                    count1 = 1;
                    prev1 = char1;
                }
                else
                {
                    count1++;
                }

                var char2 = arr2[i];
                if(!s2.Contains(char2))
                {
                    s2.Add(char2);
                }

                if (char2 != prev2)
                {
                    l2.Add(count2);
                    count2 = 1;
                    prev2 = char2;
                }
                else
                {
                    count2++;
                }
            }
            l1.Add(count1);
            l2.Add(count2);
            l1.Sort();
            l2.Sort() ;
            // Console.WriteLine($"{JsonSerializer.Serialize(l1)}");
            // Console.WriteLine($"{JsonSerializer.Serialize(l2)}");
            return s1.SetEquals(s2) && l1.OrderBy(x=>x).SequenceEqual(l2.OrderBy(x => x));
        }
    }
}
