namespace Practice
{
    public class ReserveSeat
    {
        // Add properties and methods for reserving a seat here
        public static int MaxFamilies(int rowCount, int[][] reservedSeats)
        {   
            if(rowCount <= 0 || reservedSeats == null || reservedSeats.Length == 0)
            {
               return rowCount * 2;
            }

            int res = 0;
            
            Dictionary<int, HashSet<int>> seatList = new();

            foreach(var seats in reservedSeats)
            {
                int row = seats[0];
                int seat = seats[1];

                if(seatList.TryGetValue(row, out var seatSet))
                {
                    seatSet.Add(seat);
                }
                else
                {
                    seatList[row] = new HashSet<int> {seat};
                }
            }

            // Add empty rows to the result
            res += (rowCount - seatList.Count) * 2;


            foreach(var seats in seatList)
            {
                var reserved  = seats.Value;
                //check left block of 4 seats (2-5)
                if (CheckLeft(reserved))
                {
                    res++;

                    //skip middle and check right
                    if (CheckRight(reserved))
                    {
                        res++;
                    }
                }
                else
                {
                    //check middle block of 4 seats (4-7)
                    if (CheckMiddle(reserved))
                    {
                        res++;
                    }
                    else
                    {
                        //check right block of 4 seats (6-9)
                        if (CheckRight(reserved))
                        {
                            res++;
                        }
                    }
                }

            }

            // Implement the logic to calculate the maximum number of families that can be seated
            return res; 
        }


        public static void Run()
        {
            // one completelty empty row
            int rowCount = 1;
            int[][] reservedSeats = new int[0][];
            int result = MaxFamilies(rowCount, reservedSeats);
            Console.WriteLine($"{(result == 2 ? "Pass" : "Fail")}");

            // Reservations only at seats 1 and 10
            rowCount = 1;
            reservedSeats = new int[][] { new int[] { 1, 10 } };
            result = MaxFamilies(rowCount, reservedSeats);
            Console.WriteLine($"{(result == 2 ? "Pass" : "Fail")}");

            //Only the middle block is available
            rowCount = 1;
            reservedSeats = reservedSeats = new int[][]
            {
                new int[] { 1, 2 },
                new int[] { 1, 3 },
                new int[] { 1, 6 },
                new int[] { 1, 7 },
            };
            result = MaxFamilies(rowCount, reservedSeats);
            Console.WriteLine($"{(result == 1 ? "Pass" : "Fail")}");
            
            //No family can be seated
            rowCount = 1;
            reservedSeats = new int[][] 
            {
                new int[] { 1, 3 },
                new int[] { 1, 5 },
                new int[] { 1, 6 },
                new int[] { 1, 9 },
            };
            
            result = MaxFamilies(rowCount, reservedSeats);
            Console.WriteLine($"{(result == 0 ? "Pass" : "Fail")}");

            //very large rowCount with one reserved row
            rowCount = 1000000;
            reservedSeats = new int[][] { new int[] { 1, 10 } };
            result = MaxFamilies(rowCount, reservedSeats);
            Console.WriteLine($"{(result == 1999998 ? "Pass" : "Fail")}");


        }
        private static bool CheckLeft(HashSet<int> seats) => 
            !seats.Contains(2) && !seats.Contains(3) && !seats.Contains(4) && !seats.Contains(5);

        private static bool CheckRight(HashSet<int> seats) =>
            !seats.Contains(6) && !seats.Contains(7) && !seats.Contains(8) && !seats.Contains(9);

        private static bool CheckMiddle(HashSet<int> seats) =>
            !seats.Contains(4) && !seats.Contains(5) && !seats.Contains(6) && !seats.Contains(7);


    }
}