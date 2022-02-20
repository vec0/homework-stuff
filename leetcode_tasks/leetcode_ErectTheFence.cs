/*

LeetCode - Erect the Fence
https://leetcode.com/problems/erect-the-fence/

You are given an array trees where trees[i] = [xi, yi] represents the location of a tree in the garden.
You are asked to fence the entire garden using the minimum length of rope as it is expensive. The garden is well fenced only if all the trees are enclosed.
Return the coordinates of trees that are exactly located on the fence perimeter.


Example 1:
        |                        
     5  |                        
        |                        
     4  |       x                
        |                        
     3  |           x            
        |                        
     2  |       x       x        
        |                        
     1  |   x                    
        |_______x_______________
        0   1   2   3   4   5
Input: points = [[1,1],[2,2],[2,0],[2,4],[3,3],[4,2]]
Output: [[1,1],[2,0],[3,3],[2,4],[4,2]]


Example 2:
        |                        
     5  |                        
        |                        
     4  |                        
        |                        
     3  |                        
        |                        
     2  |   x   x       x        
        |                        
     1  |                        
        |_______________________
        0   1   2   3   4   5
Input: points = [[1,2],[2,2],[4,2]]
Output: [[4,2],[2,2],[1,2]]

*/



using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

class leetcode_ErectTheFence
{

    static void Main_()
    {
        new leetcode_ErectTheFence().Run();
    }


    string[][] tests = new[]
    {
        new string[] { "[0,0],[0,5],[0,9],[2,2],[2,3],[2,4],[3,6],[4,0],[4,1],[4,9],[5,0],[5,1],[5,7],[6,0],[6,2],[6,5],[7,9],[8,1],[8,7],[9,1],[9,2],[9,5]",
            "[0,0],[4,0],[5,0],[6,0],[0,9],[4,9],[7,9],[0,5],[9,1],[9,2],[9,5],[8,7]" },
        new string[] { "[0,1],[0,2],[0,8],[1,0],[1,3],[1,6],[2,7],[2,8],[2,9],[3,8],[4,4],[4,6],[5,2],[6,1],[6,7],[7,1],[7,2],[7,4],[8,4],[8,5],[8,7],[9,5],[9,8]",
            "[2,9],[0,1],[0,2],[9,8],[0,8],[9,5],[1,0],[7,1]" },
        new string[] { "[3,0],[4,0],[5,0],[6,1],[7,2],[7,3],[7,4],[6,5],[5,5],[4,5],[3,5],[2,5],[1,4],[1,3],[1,2],[2,1],[4,2],[0,3]",
            "[7,2],[1,4],[0,3],[4,0],[5,0],[6,1],[1,2],[2,5],[3,0],[2,1],[7,3],[3,5],[7,4],[6,5],[5,5],[4,5]" },
        new string[] { "[1,1],[2,2],[2,0],[2,4],[3,3],[4,2]",
            "[2,4],[2,0],[1,1],[3,3],[4,2]" },
        new string[] { "[3,7],[6,8],[7,8],[11,10],[4,3],[8,5],[7,13],[4,13]",
            "[[4, 3], [7, 13], [4, 13], [3, 7], [8, 5], [11, 10]" }
    };


    nint[][] input, output3;

    public void Run()
    {
        var inst = new leetcode_ErectTheFence();
        Stopwatch stopWatch = new Stopwatch();

        for (int i = 0; i < tests.Length; i++)
        {
            output = Enumerable.Repeat(new nint[2], GRID_SIZE + 1).Select(c => new nint[2]).ToArray();

            Console.WriteLine("-- Test run!");
            Console.WriteLine(tests[i][0]);
            Console.WriteLine(tests[i][1]);
            var q = tests[i][0].Replace(" ", "").Split("],[").Select(s => s.Trim('[').Trim(']').Split(',').Select(v => nint.Parse(v)).ToArray()).ToDictionary(k => k[0] * 10000 + k[1]);
            var a = tests[i][1].Replace(" ", "").Split("],[").Select(s => s.Trim('[').Trim(']').Split(',').Select(v => nint.Parse(v)).ToArray()).ToDictionary(k => k[0] * 10000 + k[1]);
            input = tests[i][0].Replace(" ", "").Split("],[").Select(s => s.Trim('[').Trim(']').Split(',').Select(v => nint.Parse(v)).ToArray()).ToArray();

            stopWatch.Restart();
            stopWatch.Start();
            output3 = OuterTrees(input);
            stopWatch.Stop();

            Console.WriteLine("[ {0} ] {1}/{2}",
                                      output3.Select(o => string.Format("[{0}, {1}]", o[0], o[1])).Aggregate((a, b) => a + ", " + b),
                                      input.Length,
                                      output3.Length
                         );

            foreach (var item in output3.ToDictionary(k => k[0] * 10000 + k[1]))
            {
                if (!a.ContainsKey(item.Key))
                {
                    Console.WriteLine("   ERROR!");
                    return;
                }
            }

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = (Math.Round(ts.TotalMilliseconds) / 1000).ToString();
            Console.WriteLine("   Done! RunTime " + elapsedTime);
        }
    }





    const int GRID_SIZE = 100;
    const int MAX_NUMBER = GRID_SIZE + 2;
    //const nint MIN = 0;
    //const nint MAX = 1;
    // nint[][] output = new nint[GRID_SIZE + 1][];
    nint[][] output = Enumerable.Repeat(new nint[2], GRID_SIZE + 1).Select(c => new nint[2]).ToArray();
    nint output_count = 0;
    // nint[][] minmax = new nint[GRID_SIZE + 1][];
    nint[][] minmax_for_x = Enumerable.Repeat(new nint[2], GRID_SIZE + 1).Select(c => new nint[2]).ToArray();
    double[] slope_mem_array_x = new double[GRID_SIZE / 2 + 1];
    double[] slope_mem_array_y = new double[GRID_SIZE / 2 + 1];
    double[] slope_mem_array_k = new double[GRID_SIZE / 2 + 1];


    void WriteMinMax(ref nint[][] minmax, out nint min_y, out nint max_y)
    {

        min_y = MAX_NUMBER;
        max_y = -1;
        nint _x, _y;

        for (int i = 0; i < minmax.Length; i++)
        {
            minmax[i][0] = MAX_NUMBER;
            minmax[i][1] = -1;
        }
        for (int i = 0; i < input.Length; i++)
        {
            _x = input[i][0];
            _y = input[i][1];

            if (_x < minmax[_y][0]) minmax[_y][0] = _x;
            if (_x > minmax[_y][1]) minmax[_y][1] = _x;
            if (_y < min_y) min_y = _y;
            if (_y > max_y) max_y = _y;
        }

        // ADD TO RESULT
        if (minmax[min_y][0] != minmax[min_y][1])
        {
            for (int i = 0; i < input.Length; i++) if (input[i][1] == min_y)
                {
                    output[output_count][0] = input[i][0];
                    output[output_count][1] = min_y;
                    output_count++;
                }
        }
        else
        {
            output[output_count][0] = minmax[min_y][0];
            output[output_count][1] = min_y;
            output_count++;
        }
        if (max_y != min_y)
        {
            if (minmax[max_y][0] != minmax[max_y][1])
            {
                for (int i = 0; i < input.Length; i++) if (input[i][1] == max_y)
                    {
                        output[output_count][0] = input[i][0];
                        output[output_count][1] = max_y;
                        output_count++;
                    }
            }
            else
            {
                output[output_count][0] = minmax[max_y][0];
                output[output_count][1] = max_y;
                output_count++;
            }
        }
    }



    public nint[][] OuterTrees(nint[][] input)
    {

        if (input.Length < 4) return input;


        nint t_i;
        double point_x, point_y, target_kX, target_kY;
        double ray_to_final_point_slope_factor;
        double? ray_to_previous_point_slope_factor;
        double c_kX, c_kY, current_ray_slope_factor = 0;
        nint points_in_line_count;
        bool is_last_on_border;
        bool is_point_on_border, now_three_points_in_line;
        bool found_point_candidate;
        nint candidate_x = -1, candidate_y = -1;
        nint slope_mem_pos;
        output_count = 0;


        nint min_y, max_y;
        WriteMinMax(ref minmax_for_x, out min_y, out max_y);


        // 0 - GET MIN X ; 1 - GET MAX X
        for (int MIN_MAX = 0; MIN_MAX < 2; MIN_MAX++)
        {

            point_x = minmax_for_x[min_y][MIN_MAX];
            point_y = min_y;
            target_kX = minmax_for_x[max_y][MIN_MAX] - point_x;
            target_kY = max_y - point_y;
            ray_to_final_point_slope_factor = target_kX / target_kY;
            ray_to_previous_point_slope_factor = null;
            points_in_line_count = 0;
            found_point_candidate = false;
            slope_mem_pos = 0;
            is_last_on_border = false;

            for (nint y = min_y + 1; y < max_y + 2; y++)
            {


                if (y == max_y + 1)
                {
                    is_point_on_border = false;
                    now_three_points_in_line = false;
                }
                else
                {
                    if (minmax_for_x[y][1] < 0) continue;

                    c_kY = y - point_y;
                    if (c_kY == 0) continue;
                    c_kX = minmax_for_x[y][MIN_MAX] - point_x;
                    current_ray_slope_factor = c_kX / c_kY;

                    is_point_on_border = MIN_MAX == 0 ? ((current_ray_slope_factor - ray_to_final_point_slope_factor) < 0.0000001) : ((current_ray_slope_factor - ray_to_final_point_slope_factor) > -0.0000001);
                    now_three_points_in_line = points_in_line_count == 1 || ray_to_previous_point_slope_factor.HasValue && is_point_on_border && Math.Abs(current_ray_slope_factor - ray_to_previous_point_slope_factor.Value) < 0.0000001;
                    if (now_three_points_in_line)
                    {
                        if (!is_last_on_border) points_in_line_count = 2;
                        if (points_in_line_count == 1) points_in_line_count = 2;
                        if (points_in_line_count == 0) points_in_line_count = 1;
                        now_three_points_in_line &= points_in_line_count == 2;
                        is_point_on_border &= points_in_line_count == 2;
                    }
                    else
                    {
                        points_in_line_count = 0;
                    }
                    is_last_on_border = is_point_on_border;

                }


                if (is_point_on_border)
                {
                    found_point_candidate = true;

                    ray_to_final_point_slope_factor = current_ray_slope_factor;
                    ray_to_previous_point_slope_factor = current_ray_slope_factor;
                    candidate_x = minmax_for_x[y][MIN_MAX];
                    candidate_y = y;

                    slope_mem_array_x[slope_mem_pos] = point_x;
                    slope_mem_array_y[slope_mem_pos] = point_y;
                    slope_mem_array_k[slope_mem_pos] = current_ray_slope_factor;
                }


                if (found_point_candidate && (!is_point_on_border || now_three_points_in_line))
                {
                    found_point_candidate = false;


                    if (slope_mem_pos > 0)
                    {
                        while (slope_mem_pos > 0)
                        {
                            t_i = slope_mem_pos - 1;
                            c_kX = candidate_x - slope_mem_array_x[t_i];
                            c_kY = candidate_y - slope_mem_array_y[t_i];
                            current_ray_slope_factor = c_kX / c_kY;

                            if (MIN_MAX == 0 ? ((current_ray_slope_factor - slope_mem_array_k[t_i]) < -0.0000001) : ((current_ray_slope_factor - slope_mem_array_k[t_i]) > 0.0000001))
                            {
                                slope_mem_array_k[slope_mem_pos - 1] = current_ray_slope_factor;
                                slope_mem_pos--;
                                output_count--;
                            }
                            else break;
                        }
                    } // Exclude unused points


                    if (candidate_y != max_y)
                    {
                        y--;
                        point_x = candidate_x;
                        point_y = candidate_y;

                        c_kX = minmax_for_x[max_y][MIN_MAX] - point_x;
                        c_kY = max_y - point_y;
                        ray_to_final_point_slope_factor = c_kX / c_kY;

                        output[output_count][0] = candidate_x;
                        output[output_count][1] = candidate_y;
                        output_count++;
                        slope_mem_pos++;
                    }
                }
            }
        }



        Array.Resize(ref output, (int)output_count);
        return output;


    }


}

