using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._394._Decode_String
{    
    public class Solution
    {
        private class Recorder
        {
            public StringBuilder Num { get; set; } = new StringBuilder();
            public int Index { get; set; }
        }
        public string DecodeString(string s)
        {
            // [
            var stack = new Stack<Recorder>();
            // ] 
            var q = new Queue<int>();
            var ans = new StringBuilder();
            var r = new Recorder();

            string getSubStr(Recorder l, int r)
            {
                var subStr = string.Empty;
                if(stack.Count == 0 && q.Count == 0)
                {
                    subStr = s.Substring(l.Index + 1, r - l.Index -1);
                }
                else
                {
                    getSubStr(stack.Pop(), q.Dequeue());
                }

                var repeat = int.Parse(l.Num.ToString());
                var sb = new StringBuilder();

                for (var i =0; i< repeat; i++)
                {
                    sb.Append(subStr);
                }
                return sb.ToString();
            }

            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (char.IsDigit(c))
                {
                    r.Num.Append(c);
                    continue;
                }

                if (stack.Count == 0 && q.Count == 0 && char.IsLower(c))
                {
                    ans.Append(c);
                    continue;
                }

                if(c == '[')
                {
                    r.Index = i;
                    stack.Push(r);
                    r = new Recorder();
                    continue;
                }

                if( c == ']')
                {
                    q.Enqueue(c);
                    if (q.Count == stack.Count)
                    {
                        ans.Append(getSubStr(stack.Pop(), q.Dequeue()));
                    }
                    continue;
                }
            }
            return ans.ToString();
        }
    }
}
