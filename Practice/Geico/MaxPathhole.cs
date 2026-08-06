using System.Collections;

namespace Practice.Geico
{
    public class MaxPathhole
    {
        public int MaxPatholes(string road, int budget)
        {
            if(string.IsNullOrEmpty(road) || budget <= 1)
            {
                return 0;
            }

            // Implementation for calculating max potholes will go here.
            int res = 0;

            //process road

            //diction in descending order
            SortedDictionary<int, int> pathholeCount = new(Comparer<int>.Create((x, y) => y.CompareTo(x)));
            int i = 0;
            while (i < road.Length)
            {
                if(road[i] != 'x')
                {
                    i++;
                    continue;
                }

                int holeLength = 0;
                while (i < road.Length && road[i] == 'x')
                {
                    holeLength++;
                    i++;
                }
                if(holeLength > 0)
                {
                    pathholeCount.TryGetValue(holeLength, out int count);
                    pathholeCount[holeLength] = count + 1;
                }
            }

            //calculate max potholes within budget
            foreach(var kvp in pathholeCount)
            {
                //return if budget is too low
                if(budget <= 1)
                {
                    break;
                }

                int length = kvp.Key;
                int count = kvp.Value;
                while(count > 0)
                {
                    // if budget is smaller than length
                    if(budget <= length + 1)
                    {
                        res += budget - 1;
                        budget = 0;
                        break;       
                    }
                    else // budget is more than pothole length
                    {
                        res += length;
                        budget -= length + 1;
                        count--;
                    }
                }
            }

            return res;
        }
    }

    public class MaxPathholeTest
    {
        public void RunTests()
        {
            // Each case pairs an encoded input with the string it should decode to.
            var testCases = new (string road, int budget, int expected)[]
            {
                // One group: exact budget
                ("xxx", 4, 3),

                // One group: partial repair
                ("xxxxxxxxxx", 6, 5),

                // Duplicate group lengths: catches frequency-counting errors
                ("xx..xx", 6, 4),

                // Full group followed by partial repair:
                // 3 potholes cost 4, then 2 potholes cost 3
                ("xxx..xx", 7, 5),

                // Full group repaired first, then one pothole from another group
                ("xxxxx..xxx", 8, 6),

                // Budget too small for even one repair
                ("xxxxx", 1, 0),

                // No potholes
                ("..........", 100, 0),

                // All groups can be repaired
                ("x.x.x", 6, 3)
            };

            foreach (var (road, budget, expected) in testCases)
            {
                int result = new MaxPathhole().MaxPatholes(road, budget);
                Console.WriteLine(result == expected ? "Pass" : "Fail");
            }
        }
    }
}