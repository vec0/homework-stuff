public class Solution {
    public int[] PlatesBetweenCandles(string s, int[][] queries) {
          int[] sum_left = new int[s.Length];
        int[] sum_right = new int[s.Length];
        int cs = 0;
        int full_left_s = 0;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (s[i] == '*') cs++;
            if (s[i] == '|')
            {
                full_left_s += cs;
                cs = 0;
                var st = i + 1;
                while (st < s.Length && s[st] != '|' )
                {
                    sum_right[st] = full_left_s;
                    st++;
                }
            }
            sum_left[i] = full_left_s;
            sum_right[i] = full_left_s;
        }

        return queries.Select(q => Math.Max(0, sum_left[q[0]] - sum_right[q[1]])).ToArray();
    }
}