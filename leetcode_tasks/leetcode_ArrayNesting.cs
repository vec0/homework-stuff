/*

LeetCode - Array Nesting
https://leetcode.com/problems/array-nesting/

You are given an integer array nums of length n where nums is a permutation of the numbers in the range [0, n - 1].
You should build a set s[k] = {nums[k], nums[nums[k]], nums[nums[nums[k]]], ... } subjected to the following rule:

The first element in s[k] starts with the selection of the element nums[k] of index = k.
The next element in s[k] should be nums[nums[k]], and then nums[nums[nums[k]]], and so on.
We stop adding right before a duplicate element occurs in s[k].
Return the longest length of a set s[k].

 
Example 1:

Input: nums = [5,4,0,3,1,6,2]
Output: 4
Explanation: 
nums[0] = 5, nums[1] = 4, nums[2] = 0, nums[3] = 3, nums[4] = 1, nums[5] = 6, nums[6] = 2.
One of the longest sets s[k]:
s[0] = {nums[0], nums[5], nums[6], nums[2]} = {5, 6, 2, 0}
Example 2:

Input: nums = [0,1,2]
Output: 1

*/


using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

class leetcode_ArrayNesting
{

    static void Main_()
    {
        Console.WriteLine(new leetcode_ArrayNesting().ArrayNesting(new nint[] { 1, 2, }));
    }


    const nint LIM_1_C = 099999999;
    const nint LIM_1 = 100000000;
    const nint LIM_2_C = 199999999;
    const nint LIM_2 = 200000000;
    public nint ArrayNesting(nint[] nums)
    {
        nint L = nums.Length;
        nint result = 0, distance = 0;
        nint d, t;

        for (int i = 0; i < L; i++)
        {

            // skip indexes that market with temp LIM_1 flag
            if (nums[i] > LIM_1_C) continue;

            // trying to find not calculated values
            d = i;
            while (nums[d] < LIM_1)
            {
                // + temp LIM_1 flag, will override below
                t = nums[d];
                nums[d] += LIM_1;
                d = t;
                distance++;
            }

            // add previous calculation
            if (nums[d] > LIM_2_C) distance += nums[d] % LIM_1;

            // put calulated values to values that marked LIM_1 flag
            d = i;
            while (nums[d] < LIM_2)
            {
                t = nums[d] % LIM_1;
                nums[d] = distance-- + LIM_2;
                // if (distance < 0) throw new Exception("ASD"); // debug
                d = t;
            }

            // log(nums, nums);

            if (nums[i] % LIM_1 > result) result = nums[i] % LIM_1;
            distance = 0;
        }


        void log(nint[] n, nint[] d)
        {
            for (int i = 0; i < n.Length; i++) Console.Write((n[i] < 0 ? "" : " ") + n[i] + " "); Console.WriteLine();
            for (int i = 0; i < n.Length; i++) Console.Write((d[i] < 0 ? "" : " ") + d[i] + " "); Console.WriteLine();
        }

        return result;
    }
}
