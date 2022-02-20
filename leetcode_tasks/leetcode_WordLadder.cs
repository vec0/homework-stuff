public class Solution {
            

        public int LadderLength(string beginWord, string endWord, IList<string> wordList)
        {
           List<string> current = new List<string>();
            current.Add(beginWord);
            int step = 0;
            while (wordList.Count > 0 && current.Count != 0)
            {
                step++;
                var tc = current.ToArray();
                current.Clear();
                foreach (var v in tc)
                {
                    for (int i = 0; i < wordList.Count; i++)
                    {
                        int d = 0;
                        for (int d_c = 0; d_c < wordList[i].Length; d_c++)
                            if (v[d_c] != wordList[i][d_c])
                            {
                                d++;
                                if (d > 1) break;
                            }
                        if (d == 1)
                        {
                            if (wordList[i] == endWord) return step + 1;
                            current.Add(wordList[i]);
                            wordList.RemoveAt(i--);
                        }
                    }
                }
            }

            return 0;

    }

}