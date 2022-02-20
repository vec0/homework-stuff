public class Solution {
       void Fill(Dictionary<int, int> d, int[] nums1, int[] nums2)
        {
            for (int i1 = 0; i1 < nums1.Length; i1++)
            {
                for (int i2 = 0; i2 < nums2.Length; i2++)
                {
                    if (!d.ContainsKey(nums1[i1] + nums2[i2])) d.Add(nums1[i1] + nums2[i2], 0);
                    d[nums1[i1] + nums2[i2]]++;
                }
            }
        }

        public int FourSumCount(int[] nums1, int[] nums2, int[] nums3, int[] nums4)
        {
            Dictionary<int, int> d1 = new Dictionary<int, int>();
            Dictionary<int, int> d2 = new Dictionary<int, int>();
            Fill(d1, nums1, nums2);
            Fill(d2, nums3, nums4);
            int result = 0;
            foreach (var v in d1)
            {
                if (d2.ContainsKey(-v.Key))
                {
                    result += v.Value * d2[-v.Key];
                }
            }
            return result;
        }
}