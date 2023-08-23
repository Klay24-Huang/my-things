namespace Solution
{
    public class Solution
    {
        public IList<IList<int>> FindDifference(int[] nums1, int[] nums2)
        {
            var set1 = nums1.ToHashSet();
            var set2 = nums2.ToHashSet();

            IList<IList<int>> ans = new List<IList<int>>
            {
                nums1.Where(x => !set2.Contains(x)).Distinct().ToList(),
                nums2.Where(x => !set1.Contains(x)).Distinct().ToList(),
            };

            return ans;
        }
    }
}