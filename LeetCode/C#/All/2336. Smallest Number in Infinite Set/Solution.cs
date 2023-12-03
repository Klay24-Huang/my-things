using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All._2336._Smallest_Number_in_Infinite_Set
{
    public class SmallestInfiniteSet
    {
        public int Index { get; set; }

        public SortedSet<int> SortedSet { get; set; } = new();

        public SmallestInfiniteSet()
        {

        }

        public int PopSmallest()
        {
            if (SortedSet.Count > 0)
            { 
                var first = SortedSet.First();
                SortedSet.Remove(first);
                return first;
            }
            else
            {
                return  ++Index;
            }
        }

        public void AddBack(int num)
        {
            if (num <= Index)
            {
                SortedSet.Add(num);
            }
        }
    }

}
