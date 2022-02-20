
/*

------------------------------------------------------------------------------------------

algoexpert - Max Profit With K Transactions
https://www.algoexpert.io/questions/Max%20Profit%20With%20K%20Transactions

  You're given an array of positive integers representing the prices of a single stock on
  various days (each index in the array represents a different day). You're also
  given an integer <span>k</span>, which represents the number of transactions
  you're allowed to make. One transaction consists of buying the stock on a
  given day and selling it on another, later day.

  Write a function that returns the maximum profit that you can make by buying
  and selling the stock, given k transactions.

  Note that you can only hold one share of the stock at a time; in other words,
  you can't buy more than one share of the stock on any given day, and you can't
  buy a share of the stock if you're still holding another share. Also, you
  don't need to use all k transactions that you're allowed.

Sample Input
prices = [5, 11, 3, 50, 60, 90]
k = 2

Sample Output
93 // Buy: 5, Sell: 11; Buy: 3, Sell: 90


------------------------------------------------------------------------------------------

  complexity:
  first loop takes two N
  after then we will create binary tree from root elements and their children, but it will be done in parts and will be depending on K
  I used fixed elements' pool for this solution, no GC need
 
  for eaxmple we have different 100 arrays with 10 elements, and K = 100, so it can use both whole 100 and 10
  so with the worst price disposition we will have complexity required to build binary tree N*log(N)
  and finally it will be 2*N + N*log(N) or just N*log(N), that faster than task requered N*K

------------------------------------------------------------------------------------------

*/


// #define USE_PACK_TO_LONG
using System;


class algoexpert_MaxProfitWithKTransactionsFixedTree
{

    static void Main_()
    {
        int[][] tests = new[] {
            new int[] { 5, 11, 3, 50, 60, 50, 90 },
            new int[] { 5, 7, 34, 75, 23, 72, 97, 56, 723, 52, 52, 643, 6, 777, 2, 5, 6, 7, 6 },
            new int[] { 5, 7, 34, 75, 23, 72, 97, 56, 723, 52, 52, 643, 6, 2, 5, 6, 7, 6, 777 },
            new int[] { 5, 75, 23, 97, 56, 723, 52, 643, 2, 7, 6, 777 },
            new int[] { 5, 7, 34, 75, 23, 72, 97, 56, 723 }
        };

        Console.WriteLine(algoexpert_MaxProfitWithKTransactionsFixedTree.MaxProfitWithKTransactions(tests[3], 4));
    }


    public static int MaxProfitWithKTransactions(int[] prices, int k)
    {
        return new MaxProfitWithKTransactionsInstance(prices, k).Calc();
    }

    struct PriceVector
    {
        // binary tree / linked list
        public bool has_children => first_child_node != -1;
        // for sequence left_node is pointer to last element, right_node is pointer to next element
        public int left_node, right_node, first_child_node, parent_node;
        
        // data
        public int summary_profit => direction * (max_price - min_price);

#if USE_PACK_TO_LONG
        long data;

        const int SIZE_32 = 32, DIR_SIZE_2 = 2;
        const long MIN_PRICE_MASK = ((long)-1) >> SIZE_32 << SIZE_32;
        const long MAX_PRICE_MASK = (~MIN_PRICE_MASK) >> DIR_SIZE_2 << DIR_SIZE_2;
        const long DIR_MASK = 3;
        const long DIR_HAS_VALUE_MASK = 4;
        // thrash
        public int min_price { get => (int)((data & MIN_PRICE_MASK) >> SIZE_32); set => data = (data & ~MIN_PRICE_MASK) | ((long)value << SIZE_32); }
        public int max_price { get => (int)((data & MAX_PRICE_MASK) >> DIR_SIZE_2); set => data = (data & ~MAX_PRICE_MASK) | ((long)value << DIR_SIZE_2); }
        public int direction
        {
            get => (data & DIR_HAS_VALUE_MASK) == 0 ? (int)((data = data & ~DIR_MASK | 2 | DIR_HAS_VALUE_MASK) & 0) : (int)((data & DIR_MASK) - 2); 
            set => data = (data & ~DIR_MASK) | ((long)value + 2);
        }
#else
        public int min_price, max_price, direction;
#endif
    }


    class MaxProfitWithKTransactionsInstance
    {
        static MaxProfitWithKTransactionsInstance instance;
        
        // fixed arrays
        readonly PriceVector[] POOL;
        int pool_size;
        int pool_index = 0;
        readonly int[] root_list;
        int root_list_count = 0;
        
        // input
        int[] PRICES;
        int K;
       
        // constructor
        public MaxProfitWithKTransactionsInstance(int[] prices, int k)
        {
            this.PRICES = prices;
            this.K = k;
            this.POOL = new PriceVector[prices.Length + 10];
            this.root_list = new int[prices.Length + 10];
            this.pool_size = this.POOL.Length;
            instance = this;
        }

        // pool methods
        ref PriceVector GetNewNodeReference()
        {
            //do {pool_index++;
            //    if (pool_index == pool_size)
            //    {   pool_index = 0;
            //        pool_error++;
            //        if (pool_error > 1) throw new IndexOutOfRangeException();
            //    }
            //} while (POOL[pool_index].used != 0);
            pool_index++;
            POOL[pool_index].parent_node = POOL[pool_index].left_node = POOL[pool_index].right_node = POOL[pool_index].first_child_node = -1;
            return ref POOL[pool_index];
        }

        // body
        public int Calc()
        {
            if (PRICES.Length < 2 || K < 1) return 0;

            // Trim
            int left_bound = 0;
            while (left_bound + 1 < PRICES.Length && PRICES[left_bound] >= PRICES[left_bound + 1]) left_bound++;
            int right_bound = PRICES.Length - 1;
            while (right_bound - 1 >= 0 && PRICES[right_bound] <= PRICES[right_bound - 1]) right_bound--;
            if (left_bound >= right_bound) return 0;

            // Initialization
            ref PriceVector current_vector = ref GetNewNodeReference();
            current_vector.min_price = PRICES[left_bound];
            current_vector.max_price = PRICES[left_bound];
            current_vector.direction = 0;
            root_list[root_list_count++] = pool_index;

            int r_index, c_index, p_index;
            int dir;
            ref PriceVector p = ref current_vector, child = ref current_vector;

            // Body // place new element to the root children and then merge to many different binary trees if it is possible
            for (int i = left_bound + 1; i <= right_bound; i++)
            {
                current_vector = ref POOL[root_list[root_list_count - 1]];
                dir = PRICES[i] - PRICES[i - 1];
                if (dir == 0) continue;
                dir = Math.Sign(dir);
                if (current_vector.direction == 0) current_vector.direction = dir;

                // same dir, increase max value for last point and try to merge with any previous root node with same direction
                if (dir == current_vector.direction)
                {
                    current_vector.max_price = PRICES[i];

                    // try to merge
                    // improve (can be added min max break case to improve performance)
                    if (i == right_bound || Math.Sign(PRICES[i + 1] - PRICES[i]) != dir)
                    {
                        r_index = root_list_count - 1;
                        // try to find a candidate
                        while (true)
                        {
                            r_index--;
                            if (r_index < 0) break;
                            p_index = root_list[r_index];
                            p = ref POOL[p_index];
                            if (p.direction != current_vector.direction) continue;
                            if (p.direction * p.max_price < p.direction * current_vector.max_price && p.direction * p.min_price <= p.direction * current_vector.min_price)
                            { // merge and remove

                                // try to merge with any previuos root node
                                //  \<
                                //   \    /\
                                //    \/\/  \
                                //           \<

                                root_list_count--; // remove last root element
                                if (current_vector.has_children) BinaryTreeUtilities.CopyChildrenSequenceToChildrenAsSequence(p_index, current_vector.first_child_node);
                                p.max_price = current_vector.max_price;

                                c_index = 0;
                                r_index++;
                                while (r_index < root_list_count - c_index)
                                {
                                    BinaryTreeUtilities.CopyElementToChildrenAsSequence(p_index, root_list[r_index]);
                                    c_index++;
                                }
                                root_list_count -= c_index; // remove last root elements

                                break;
                            }
                        }
                    }
                }
                // direction changed
                else
                {
                    child = ref GetNewNodeReference();
                    child.direction = dir;
                    child.min_price = current_vector.max_price;
                    root_list[root_list_count++] = pool_index;
                   
                    if (dir * PRICES[i] > dir * current_vector.min_price)
                    {
                        child.max_price = current_vector.max_price;
                        // use merge operation in the next cycle
                        //        /<  
                        //  \<   /
                        //   \  / 
                        //    \/
                        //   
                        i--;
                    }
                    else
                    {
                        // skip merge operation
                        //  \<
                        //   \  /<   
                        //    \/
                        //          
                        child.max_price = PRICES[i];
                    }
                }
            }
            
            
            // clean up root list
            int root_node_index = -1;
            for (int i = 0; i < root_list_count; i++)
            {
                // remove negative vectors from the root array
                if (POOL[root_list[i]].direction < 0)
                {
                    // copy children to the end of root array and skip current element
                    if (POOL[root_list[i]].has_children) BinaryTreeUtilities.CopyChildrenSequenceToRootList(POOL[root_list[i]].first_child_node);
                }
                // copy current root element to final binary tree (without element's children)
                else
                {
                    if (root_node_index == -1)
                    {
                        root_node_index = root_list[i];
                        POOL[root_list[i]].left_node = -1;
                        POOL[root_list[i]].right_node = -1;
                    }
                    else
                        BinaryTreeUtilities.CopyElementToBinaryTree(root_node_index, root_list[i]);
                }
            }


            // now we will get every items with max summary_profit step by step, remove them, and add their children to the binary tree
            int result = 0;
            int current_max = -1;
            for (int i = 0; i < K; i++)
            {
                // find maximum in the tree
                if (current_max == -1 || POOL[current_max].right_node != -1)
                {
                    current_max = root_node_index;
                    while (true)
                    {
                        if (POOL[current_max].right_node != -1) current_max = POOL[current_max].right_node;
                        else break;
                    }
                }

                result += POOL[current_max].summary_profit;
                if (K < 100) Console.Write("[{0}, {1}] ", POOL[current_max].min_price, POOL[current_max].max_price);


                // remove used maximum node
                int target_to_children_sequence = POOL[current_max].first_child_node;
                if (POOL[current_max].left_node != -1)
                {
                    if (current_max == root_node_index)
                        current_max = root_node_index = POOL[current_max].left_node;
                    else
                        current_max = POOL[POOL[current_max].parent_node].right_node = POOL[current_max].left_node;
                }
                else
                {
                    if (current_max == root_node_index)
                        break;
                    else
                    {
                        POOL[POOL[current_max].parent_node].right_node = -1;
                        current_max = POOL[current_max].parent_node;
                    }
                }

                // add children if there are
                if (target_to_children_sequence != -1)
                {
                    BinaryTreeUtilities.CopyChildrenSequenceToBinaryTree(root_node_index, target_to_children_sequence);
                    current_max = -1;
                }
            }
            Console.WriteLine();

            return result;
        }


        class BinaryTreeUtilities
        {
            // sequence is temporary not sorted linked list
            static public void CopyElementToChildrenAsSequence(int root_index, int target_index)
            {
                if (instance.POOL[root_index].first_child_node == -1)
                {
                    instance.POOL[root_index].first_child_node = target_index;
                    instance.POOL[target_index].left_node = target_index;
                }
                else{
                    instance.POOL[instance.POOL[instance.POOL[root_index].first_child_node].left_node].right_node = target_index;
                    instance.POOL[instance.POOL[root_index].first_child_node].left_node = target_index;
                }
            }

            // sequence is temporary not sorted linked list
            internal static void CopyChildrenSequenceToChildrenAsSequence(int root_index, int target_index)
            {
                CopyElementToChildrenAsSequence(root_index, target_index);
                if (instance.POOL[target_index].right_node != -1) CopyChildrenSequenceToChildrenAsSequence(root_index, instance.POOL[target_index].right_node);
            }

            // sequence is temporary not sorted linked list
            internal static void CopyChildrenSequenceToRootList(int target_index)
            {
                instance.root_list[instance.root_list_count++] = target_index;
                if (instance.POOL[target_index].right_node != -1)
                    CopyChildrenSequenceToRootList(instance.POOL[target_index].right_node);
            }

            
            static public void CopyElementToBinaryTree(int root_index, int target_index)
            {
                int before, after = root_index;
                int value = instance.POOL[target_index].summary_profit;

                do
                {
                    before = after;
                    if (value < instance.POOL[after].summary_profit) // go to the left
                    {
                        after = instance.POOL[after].left_node;
                        if (after == -1) instance.POOL[before].left_node = target_index;
                    }
                    else // go to the right
                    {
                        after = instance.POOL[after].right_node;
                        if (after == -1) instance.POOL[before].right_node = target_index;
                    }
                } while (after != -1);

                if (instance.POOL[target_index].left_node != -1) instance.POOL[target_index].left_node = -1;
                if (instance.POOL[target_index].right_node != -1) instance.POOL[target_index].right_node = -1;
                instance.POOL[target_index].parent_node = before;
            }

            static int temp_i;
            internal static void CopyChildrenSequenceToBinaryTree(int root_index, int target_index)
            {
                temp_i = instance.POOL[target_index].right_node;
                CopyElementToBinaryTree(root_index, target_index);
                if (temp_i != -1)
                    CopyChildrenSequenceToBinaryTree(root_index, temp_i);
            }
        }
    }
}



