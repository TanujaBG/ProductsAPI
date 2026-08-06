namespace Practice.Geico
{
    public class MaxProfitGrid
    {
        public int MaxCollectableValue(int[][] grid)
        {
            if (grid == null || grid.Length == 0 || grid[0].Length == 0)
                return 0;

            int res = 0;
            int rows = grid.Length, cols = grid[0].Length;
            bool[][] visited = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                visited[i] = new bool[cols];
            }

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    if(grid[i][j] > 0)
                    {
                        int value = dfs(i, j, grid, visited);
                        res = Math.Max(res, value);
                    }
                }
            }

            return res;
        }

        private int dfs(int row, int col, int[][] grid, bool[][] visited)
        {
            int rows = grid.Length, cols = grid[0].Length;
            if(row < 0 || row >= rows || col < 0 || col >= cols || visited[row][col] || grid[row][col] == 0)
            {
                return 0;
            }

            visited[row][col] = true;

            int value = grid[row][col];

            int up = dfs(row - 1, col, grid, visited);
            int down = dfs(row+1, col, grid, visited);
            int left = dfs(row, col-1, grid, visited);
            int right = dfs(row, col+1, grid, visited);

            visited[row][col] = false;

            return value + Math.Max(Math.Max(up, down), Math.Max(left, right));
        }
    }

    public class MaxProfitGridTest
    {
        public void RunTests()
        {
            // Each case pairs an encoded input with the string it should decode to.
            var testCases = new (int[][] grid, int expected)[]
            {
                // POSITIVE TEST CASES
                (new int[][]
                {
                    new int[] {1, 0, 7},
                    new int[] {2, 0, 6},
                    new int[] {3, 4, 5},
                    new int[] {0, 3, 0},
                    new int[] {9, 0, 20}
                }, 28),

                //EMPTY grid
                (new int[][]
                {
                }, 0),

                // null grid
                (null, 0),

                // grid with all zeros
                (new int[][]
                {
                    new int[] {0, 0, 0},
                    new int[] {0, 0, 0},
                    new int[] {0, 0, 0}
                }, 0),

                // grid with one non-zero element
                (new int[][]
                {
                    new int[] {0, 0, 0},
                    new int[] {0, 5, 0},
                    new int[] {0, 0, 0}
                }, 5),

                // grid with multiple non-zero elements but isolated
                (new int[][]
                {
                    new int[] {1, 0, 0},
                    new int[] {0, 2, 0},
                    new int[] {0, 0, 3}
                }, 3),

                //grid with same maximum value via different paths
                (new int[][]
                {
                    new int[] {1, 2, 3},
                    new int[] {4, 5, 6},
                    new int[] {7, 8, 9}
                }, 45),

                // grid with a cross pattern of non-zero elements
                (new int[][]
                {
                    new int[] { 0, 6, 0 },
                    new int[] { 5, 8, 7 },
                    new int[] { 0, 9, 0 }
                }, 24),
            };

            foreach (var (grid, expected) in testCases)
            {
                int result = new MaxProfitGrid().MaxCollectableValue(grid);
                Console.WriteLine(result == expected ? "Pass" : "Fail");
            }
            Console.WriteLine("All tests completed.");
            
        }
    }

}