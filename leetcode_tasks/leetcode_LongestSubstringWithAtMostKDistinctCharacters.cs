public class Solution {
    public int LengthOfLongestSubstringKDistinct(string s, int k) {
        if (k == 0) return 0;
        if (s.Length == 1) return 1;

          Dictionary<char, int> cache = new Dictionary<char, int>();
            int left = 0;
            int count = 0;
            int max = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (!cache.ContainsKey(s[i])) cache.Add(s[i], 0);
                cache[s[i]]++;
                if (cache[s[i]] == 1)
                {
                    count++;

                    while (count > k)
                    {
                        cache[s[left]]--;
                        if (cache[s[left]] == 0) count--;
                        left++;
                    }
                }

                max = Math.Max(max, i - left);
            }
        return max +1;
    }
}