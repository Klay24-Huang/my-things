using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All._30._Substring_with_Concatenation_of_All_Words
{
    public class Solution
    {
        public IList<int> FindSubstring(string s, string[] words)
        {
            var ans = new List<int>();
            var wordLength = words[0].Length;
            var wordsCount = words.Length;
            var subStringLength = wordLength * wordsCount;
            var dic = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (dic.ContainsKey(word))
                {
                    dic[word]++;
                }
                else
                {
                    dic.Add(word, 1);
                }
            }
            //// Console.WriteLine(JsonSerializer.Serialize(dic));

            for (var i = 0; i + subStringLength <= s.Length; i += wordLength)
            {
                var subString = s.Substring(i, subStringLength);
                //// Console.WriteLine($"substring is {subString}");
                if (IsAns(subString, DicCopy(dic), wordLength))
                {
                    ans.Add(i);
                }
            }
            return ans;
        }

        private bool IsAns(string subString, Dictionary<string, int> dic, int wordLength)
        {
            // Console.WriteLine($"substring is {subString}");
            // Console.WriteLine(JsonSerializer.Serialize(dic));
            for (var i = 0; i < subString.Length; i += wordLength)
            {
                var word = subString.Substring(i, wordLength);
                // Console.WriteLine($"word is {word}");
                // Console.WriteLine($"contain key {dic.ContainsKey(word)}");
                if (dic.ContainsKey(word))
                {
                    dic[word]--;
                    if (dic[word] == 0)
                    {
                        dic.Remove(word);
                    }
                }
                else
                {
                    // Console.WriteLine("return false");
                    return false;
                }
            }
            return dic.Count == 0;
        }

        private Dictionary<string, int> DicCopy(Dictionary<string, int> dic)
        {
            return dic.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );
        }
    }
}
