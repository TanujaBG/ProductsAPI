namespace Practice.Geico
{
    public class MergeInterval
    {
        public int[][] CombineRanges(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
                return Array.Empty<int[]>();
        
            Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));
            var merged = new List<int[]>();
            int start = 0, end = 1;

            foreach(var interval in intervals)
            {
                if(merged.Count == 0 || merged[^1][end] < interval[start])
                {
                    merged.Add(new int[] { interval[start], interval[end] });
                }
                else
                {
                    merged[^1][end] = Math.Max(merged[^1][end], interval[end]);
                }
            }
            return merged.ToArray();
        }
    }

    public class MergeIntervalTests
    {
        public void RunTests()
        {
            var testcases = new (int[][] intervals, int[][] expected)[]
            {
                // Has intervals which needs merging
                (
                    new int[][] { new int[] { 1, 3 }, new int[] { 2, 6 }, new int[] { 8, 10 }, new int[] { 15, 18 } },
                    new int[][] { new int[] { 1, 6 }, new int[] { 8, 10 }, new int[] { 15, 18 } }
                ),
                // No intervals to merge
                (
                    new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 }, new int[] { 5, 6 } },
                    new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 }, new int[] { 5, 6 } }
                ),
                // All intervals merge into one
                (
                    new int[][] { new int[] { 1, 4 }, new int[] { 2, 5 }, new int[] { 3, 6 } },
                    new int[][] { new int[] { 1, 6 } }
                ),
                //intervals with matching end of next start
                (
                    new int[][] { new int[] { 1, 2 }, new int[] { 2, 3 }, new int[] { 3, 4 } },
                    new int[][] { new int[] { 1, 4 } }
                ),
                // repeated intervals
                (
                    new int[][] { new int[] { 1, 3 }, new int[] { 1, 3 }, new int[] { 1, 3 } },
                    new int[][] { new int[] { 1, 3 } }
                ),
                // 1st interval is big enough to cover the rest
                (
                    new int[][] { new int[] { 1, 10 }, new int[] { 2, 3 }, new int[] { 4, 5 } },
                    new int[][] { new int[] { 1, 10 } }
                ),
                // Unsorted input
                (
                    new int[][]
                    {
                        new[] { 8, 10 },
                        new[] { 1, 3 },
                        new[] { 2, 6 }
                    },
                    new int[][]
                    {
                        new[] { 1, 6 },
                        new[] { 8, 10 }
                    }
                ),

                // Zero-length range
                (
                    new int[][]
                    {
                        new[] { 1, 1 },
                        new[] { 1, 2 },
                        new[] { 5, 5 }
                    },
                    new int[][]
                    {
                        new[] { 1, 2 },
                        new[] { 5, 5 }
                    }
                )
            }; 
            var merger = new MergeInterval();
            foreach (var (intervals, expected) in testcases)
            {
                var result = merger.CombineRanges(intervals);
                // compare the result with the expected output
                bool isEqual = result.Length == expected.Length;
                if (isEqual)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i][0] != expected[i][0] || result[i][1] != expected[i][1])
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }
                Console.WriteLine(isEqual ? "Test passed" : "Test failed");
            }
        }
    }
}