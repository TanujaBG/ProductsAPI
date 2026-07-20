using System;
using System.Collections.Generic;
using System.Text;

namespace Practice
{
    public class Record
    {
        public static void Run()
        {
            // Day 1 — Records exercise
            // Run with:  dotnet run
            // Predict each output BEFORE running, then check yourself against what prints.

            var a = new Movie("Inception", "Nolan", 2010);
            var b = new Movie("Inception", "Nolan", 2010);
            var c = a with { Year = 2012 };   // non-destructive copy: 'a' stays untouched

            Console.WriteLine($"(1) a == b  -> {a == b}");   // value equality?
            Console.WriteLine($"(2) a == c  -> {a == c}");   // is the copy equal to the original?
            Console.WriteLine($"(3) a.Year  -> {a.Year}");   // did 'with' change 'a'?
            Console.WriteLine($"(4) a       -> {a}");        // how does a record print itself?

            // Part C — try to mutate a record property directly.
            // UNCOMMENT the next line and run again to see the compiler error (records are immutable):
            //a.Year = 2015;
        }
    }


/// <summary>
/// A movie described by its title, director, and release year.
/// Demonstrates record value-equality, immutability, and 'with' expressions.
/// </summary>
public record Movie(string Title, string Director, int Year);

}
