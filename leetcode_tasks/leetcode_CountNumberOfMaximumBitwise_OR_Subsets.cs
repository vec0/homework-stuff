public class Solution {
    public int CountMaxOrSubsets(int[] nums) {
        int max = 0;
            foreach (var vNum in nums)
            {
                max |= vNum;
            }

           // Dictionary<string, int> found_results = new Dictionary<string, int>();
           int found_results = 0;
            Action<int, int, string> dps = null;
            dps = (current_sum, current_idx, key) =>
            {/*
                if (current_sum == max)
                {
                 //   if (!found_results.ContainsKey(key)) found_results.Add(key, 0);
                    found_results++;
                   // return;
                }
                if (current_idx >= nums.Length) return;*/
                if (current_idx >= nums.Length)
                {
                    if (current_sum == max) found_results++;
                    return;
                }
                dps(current_sum | nums[current_idx], current_idx + 1, key + nums[current_idx].ToString());
                dps(current_sum, current_idx + 1, key);
            };
            dps(0, 0, "");
               
            return found_results;
    }
}