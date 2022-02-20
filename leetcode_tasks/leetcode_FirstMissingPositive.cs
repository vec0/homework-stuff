public class Solution {
    public int FirstMissingPositive(int[] nums) {
           Dictionary<int, int> cache = new Dictionary<int, int>();
            int min = int.MaxValue;
            foreach (var m in nums)
            {
                if (!cache.ContainsKey(m)) cache.Add(m,  cache.ContainsKey(m + 1)? cache[m+1]:(m+1));
                if (m < min && m >= 0) min = m;
            }

            if (min == int.MaxValue) return 1;
         if (!cache.ContainsKey(1)) return 1;
            int result = cache[min];
            while (cache.ContainsKey(result)) result = cache[result] ;
            return result;
    }
}