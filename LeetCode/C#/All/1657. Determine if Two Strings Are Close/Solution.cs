using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._1657._Determine_if_Two_Strings_Are_Close
{
    public class Solution
    {
        public bool CloseStrings(string word1, string word2)
        {
            var d = new Dictionary<char, int>();
            foreach (var word in word1)
            {
                if (d.ContainsKey(word))
                {
                    d[word]++;
                }
                else
                { d[word]++; }
            }
        }
    }
}
