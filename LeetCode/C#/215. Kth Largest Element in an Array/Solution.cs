namespace _215._Kth_Largest_Element_in_an_Array
{
    public class Solution
    {
        public int FindKthLargest(int[] nums, int k)
        {
            var pq = new PriorityQueue<int, int>();
            foreach (int num in nums)
            {
                pq.Enqueue(num, num);
            }
            
            var skip = pq.Count - k;
            while (skip > 0)
            {
                pq.Dequeue();
                skip--;
            }

            return pq.Dequeue();
        }
    }
}