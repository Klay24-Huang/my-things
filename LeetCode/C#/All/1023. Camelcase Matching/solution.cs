namespace All._1023._Camelcase_Matching
{
    public class Solution
    {
        public IList<bool> CamelMatch(string[] queries, string pattern)
        {
            return queries.Select(s => isMatch(s, pattern)).ToList();
        }

        public bool isMatch(string s, string pattern)
        {
            if (s.Length < pattern.Length)
            {
                return false;
            }

            // index for pattern
            var i = 0;

            foreach (char c in s)
            {
                //// Console.WriteLine($"{c}, {pattern[i]}");
                if (i == pattern.Length && char.IsUpper(c))
                {
                    // pattern的所有char 都走完
                    // 但s仍有未匹配的片段
                    return false;
                }

                if (i < pattern.Length && c == pattern[i])
                {
                    // 推進到pattern下一個index
                    i++;
                    continue;
                }

                if (i < pattern.Length && char.IsUpper(c) && char.IsUpper(pattern[i]) && c != pattern[i])
                {
                    // s 途中出現不符合pattern的片段
                    return false;
                }
            }

            // s 匹配pattern 所有char
            //// Console.WriteLine(i);
            return i == pattern.Length;
        }
    }
}
