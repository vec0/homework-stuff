/*

IOI '06 - Merida, Yucatan, Mexico
Deciphering the Mayan Writing
Deciphering the Mayan writing has proven to be a harder task than anticipated by the early investigations. After almost two hundred years, very little of it was actually understood. It has been only in the last three decades that real advances have been made.

Mayan writing is based on small drawings known as glyphs which represent sounds. Mayan words are normally written as glyphs put together at various positions.

One of several problems in deciphering Mayan writing arises in the order of reading. When placing several glyphs in order to form a word, Mayan writers sometimes decided the position based more on their own esthetic views than on any particular rule. This leads to the fact that, even though the sound for many glyphs is known, sometimes archaeologists are not sure how to pronounce a written word.

The archaeologists are looking for a special word W. They know the glyphs for it, but they don't know all the possible ways of arranging them. Since they knew you were coming to IOI'06, they have asked for your help. They will provide you with the g glyphs from W and a sequence S of all the glyphs (in the order they appear) in the carvings they are studying. Help them by counting the number of possible appearances of the word W.

Write a program that, given the glyphs for W and the sequence S of glyphs in the carvings, counts the number of possible appearances of W in S; that is, every sequence of consecutive g glyphs in S that is a permutation of the glyphs in W.

Input
The first line of input contains two space-separated integers: g, the number of glyphs in W (1 ≤ g ≤ 3000) and |S|, the number of glyphs in the sequence S (g ≤ |S| ≤ 3 000 000).
The second line contains g consecutive characters that represent the glyphs in W. Valid characters are the uppercase and lowercase characters of the Roman alphabet. Uppercase and lowercase characters are considered to be different.
The third line contains |S| consecutive characters that represent the glyphs in the carvings. Valid characters are the uppercase and lowercase characters of the Roman alphabet. Uppercase and lowercase characters are considered to be different.
Output
One integer on a single line, the number of possible appearances of W in S.
Sample Input
4 11
cAda
AbrAcadAbRa
Sample Output
2
Note: In some test cases worth 50% of the total marks, g will be no more than 10.

*/



#define USE_SKIPPER // it seems there is no any profit, sometimes it is even slower ~10% if there are a lot of chars candidates

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

class other_DecipheringTheMayanWriting
{


    static void Main_()
    {
        var tests = Enumerable.Repeat(0, 1).Select(s => new other_DecipheringTheMayanWriting()).ToArray();
        foreach (var item in tests) Console.WriteLine(item.Calc());
    }



    public nint Calc()
    {

        // Initialization
        const int WORDS_COUNT = 34215;// 36;
        const int ALPHABETS_COUNT = 100;
        const int SEARCH_FOR_WORD_LENGTH = 70;

        var random = new Random(0);
        char[] alphabet = Enumerable.Repeat(-1, (int)ALPHABETS_COUNT).Select((c, i) => (char)(i + 30)).ToArray();
        string search_word = new string(Enumerable.Repeat(0, (int)SEARCH_FOR_WORD_LENGTH).Select(s => alphabet[random.Next() % ALPHABETS_COUNT]).ToArray());
        nint[] search_chars = search_word.ToCharArray().Distinct().Select(c => (nint)c).ToArray();
        nint[] alphabet_and_search_word_chars_count_dic = new nint[alphabet.Max(c => (nint)c) + 1];
        nint[] alphabet_and_text_chars_count_dic = new nint[alphabet.Max(c => (nint)c) + 1];
        bool compare_inverse_flag = false;

#if USE_SKIPPER
        // will check every "i += SEARCH_FOR_WORD_LENGTH / 2" element and if alphabet_and_search_word_dic is false is will skip all elements between i and i + SEARCH_FOR_WORD_LENGTH / 2
        int alphabet_and_search_word_step = SEARCH_FOR_WORD_LENGTH / 2; //nint alphabet_and_search_word_step = 20;
#endif


        // Creating Alphabet
        Console.WriteLine("Generating text with {0} simbols...", (WORDS_COUNT * SEARCH_FOR_WORD_LENGTH).ToString("N0"));
        char[] text_string = new char[WORDS_COUNT * SEARCH_FOR_WORD_LENGTH];
        var L = text_string.Length;
        for (int i = 0; i < L; i++)
            if (random.NextDouble() < 0.01 && i < L - SEARCH_FOR_WORD_LENGTH)
            {
                for (int a = 0; a < SEARCH_FOR_WORD_LENGTH; a++) text_string[i++] = search_word[(int)a];
                i--;
            }
            //else text_string[i] = alphabet[random.Next() % ALPHABETS_COUNT]; // slow
            else text_string[i] = alphabet[Math.Abs(unchecked((i + 6363466) * 15641415)) % ALPHABETS_COUNT];
        var text = new string(text_string);


        // Body
        Console.WriteLine("Start searching...");
        nint result = 0;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        unchecked
        {
            nint search_hash1 = 0, hash1 = 0;
            nint c_cur, c_last;
            nint ab_bound = SEARCH_FOR_WORD_LENGTH - 1;
#if USE_SKIPPER
            nint skip_counter = 0;
            nint mute_skipper = 0;
            nint elements_in_hash_memory = 0;
#endif

            for (int i = 0; i < search_word.Length; i++)
            {
                alphabet_and_search_word_chars_count_dic[search_word[i]]++;
                c_cur = (nint)search_word[i] << search_word[i];
                //d1 += search_word[i] << (64 - search_word[i] % 64);
                search_hash1 ^= c_cur;
                //search_hash2 += t;
            }
#if USE_SKIPPER
            bool[] alphabet_and_search_word_array = (new bool[alphabet.Max(c => (nint)c) + 1]).Select((c, i) => alphabet_and_search_word_chars_count_dic[i] != 0).ToArray();
#endif

            for (int i = 0; i < L; i++)
            {
                c_cur = text[i];
#if USE_SKIPPER
                // skip if alphabet_and_search_word_dic is false
                if (mute_skipper > 0) mute_skipper--;
                else
                {
                    if (!alphabet_and_search_word_array[c_cur])
                    {
                        elements_in_hash_memory = 0;
                        hash1 = 0;
                        skip_counter = 1;
                        i += alphabet_and_search_word_step - 1;
                        continue;
                    }
                    else if (skip_counter != 0)
                    {
                        skip_counter = 0;
                        i -= alphabet_and_search_word_step + 1;
                        mute_skipper = alphabet_and_search_word_step;
                        continue;
                    }
                }
#endif


#if USE_SKIPPER
                if (elements_in_hash_memory > ab_bound)
#else
                if (i > ab_bound)
#endif
                {
                    c_last = text[i - SEARCH_FOR_WORD_LENGTH];
                    c_last = c_last << (int)c_last;
                    hash1 ^= c_last;
                    //da -= text[t] << (64 - text[t]);
                    //hash2 -= t;
                }

#if USE_SKIPPER
                elements_in_hash_memory++;
#endif

                c_cur = c_cur << (int)c_cur;
                hash1 ^= c_cur;
                //b_c2[t]++;
                //da += text[i] << (64 - text[i] % 64);
                //hash2 += t;

                //try
                {
                    if (hash1 == search_hash1 && i >= ab_bound)
                    {
                        int z_l = i + 1;
                        int r = 0;
                        if (!compare_inverse_flag)
                        {
                            for (int z = i - SEARCH_FOR_WORD_LENGTH + 1; z < z_l; z++) alphabet_and_text_chars_count_dic[text[z]]++;
                            for (; r < search_chars.Length; r++)
                            {
                                c_cur = search_chars[r];
                                if (alphabet_and_search_word_chars_count_dic[c_cur] != alphabet_and_text_chars_count_dic[c_cur]) break;
                            }
                        }
                        else
                        {
                            for (int z = i - SEARCH_FOR_WORD_LENGTH + 1; z < z_l; z++) alphabet_and_text_chars_count_dic[text[z]]--;
                            for (; r < search_chars.Length; r++) if (0 != alphabet_and_text_chars_count_dic[search_chars[r]]) break;
                        }

                        if (r == search_chars.Length)
                        {
                            result++;
                            compare_inverse_flag = !compare_inverse_flag;
                        }
                        else
                        {
                            compare_inverse_flag = false;
                            for (r = 0; r < search_chars.Length; r++) alphabet_and_text_chars_count_dic[search_chars[r]] = 0;
                        }
                    }
                }
                //catch
                //{
                //    nint r = 0;
                //    final_flag = false;
                //    for (r = 0; r < search_chars.Length; r++) alphabet_and_text_chars_count_dic[c_cur] = 0;
                //}

            }
        }
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = (ts.TotalMilliseconds / 1000).ToString();

        Console.WriteLine("Search completed! " + elapsedTime);
        return result;
    }
}
