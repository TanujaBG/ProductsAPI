using System;
using System.Collections.Generic;

namespace Practice
{
    /// <summary>
    /// Day 1 — Pattern Matching exercise.
    /// Predict each numbered output BEFORE running, then check yourself.
    /// Covers: switch expressions, relational patterns, property patterns,
    /// type/object patterns, and logical patterns (and / or / not).
    /// </summary>
    public class PatternMatching
    {
        public static void Run()
        {
            // ---------------------------------------------------------------
            // Part 1 — switch expression + relational patterns
            // ---------------------------------------------------------------
            Console.WriteLine("Part 1 — Rate(score):");
            Console.WriteLine($"(1) Rate(100) -> {Rate(100)}");   // ?
            Console.WriteLine($"(2) Rate(85)  -> {Rate(85)}");    // ?  (careful!)
            Console.WriteLine($"(3) Rate(40)  -> {Rate(40)}");    // ?

            // ---------------------------------------------------------------
            // Part 2 — property patterns on a record (reuses Movie from Record.cs)
            // ---------------------------------------------------------------
            var interstellar = new Movie("Interstellar", "Nolan", 2014);
            var metropolis   = new Movie("Metropolis", "Lang", 1927);
            Console.WriteLine("\nPart 2 — Category(movie):");
            Console.WriteLine($"(4) Interstellar -> {Category(interstellar)}"); // ?
            Console.WriteLine($"(5) Metropolis   -> {Category(metropolis)}");   // ?

            // ---------------------------------------------------------------
            // Part 3 — type / object patterns over a mixed list
            // ---------------------------------------------------------------
            var shapes = new List<Shape> { new Circle(2), new Rectangle(3, 4) };
            Console.WriteLine("\nPart 3 — Area(shape):");
            Console.WriteLine($"(6) Circle(2)      area -> {Area(shapes[0]):0.00}"); // ?
            Console.WriteLine($"(7) Rectangle(3,4) area -> {Area(shapes[1]):0.00}"); // ?

            // ---------------------------------------------------------------
            // Part 4 — logical patterns (and / or / not) + the 'is' pattern
            // ---------------------------------------------------------------
            Console.WriteLine("\nPart 4 — logical patterns:");
            Console.WriteLine($"(8)  Stock(0)             -> {Stock(0)}");                 // ?
            Console.WriteLine($"(9)  Stock(5)             -> {Stock(5)}");                 // ?
            Console.WriteLine($"(10) IsWeekend(Saturday)  -> {IsWeekend(DayOfWeek.Saturday)}"); // ?
            Console.WriteLine($"(11) IsWeekend(Monday)    -> {IsWeekend(DayOfWeek.Monday)}");    // ?

            // ---------------------------------------------------------------
            // Part 5 — CHALLENGE (your turn):
            // Implement Describe(object?) below so that:
            //   Describe(42)      => "int: 42"
            //   Describe("hi")    => "string of length 2"
            //   Describe(null)    => "nothing"
            //   Describe(3.14)    => "other"
            // Then UNCOMMENT the four lines below to test it.
            // ---------------------------------------------------------------
            Console.WriteLine("\nPart 5 — Describe(value):");
            Console.WriteLine($"(12) {Describe(42)}");
            Console.WriteLine($"(13) {Describe("hi")}");
            Console.WriteLine($"(14) {Describe(null)}");
            Console.WriteLine($"(15) {Describe(3.14)}");
        }

        /// <summary>Maps a score to a label using relational patterns (first match wins).</summary>
        private static string Rate(int score) => score switch
        {
            100     => "perfect",
            >= 90   => "excellent",
            >= 50   => "pass",
            _       => "fail"
        };

        /// <summary>Categorizes a movie using property patterns.</summary>
        private static string Category(Movie movie) => movie switch
        {
            { Year: < 1980 }      => "classic",
            { Director: "Nolan" } => "Nolan film",
            _                     => "standard"
        };

        /// <summary>Computes area by matching on the concrete shape type (object/type pattern).</summary>
        private static double Area(Shape shape) => shape switch
        {
            Circle circle       => Math.PI * circle.Radius * circle.Radius,
            Rectangle rectangle => rectangle.Width * rectangle.Height,
            _                   => 0
        };

        /// <summary>Classifies stock level using logical 'and' with relational patterns.</summary>
        private static string Stock(int count) => count switch
        {
            <= 0         => "out of stock",
            > 0 and < 10 => "low stock",
            _            => "in stock"
        };

        /// <summary>Uses the 'is' pattern with logical 'or'.</summary>
        private static bool IsWeekend(DayOfWeek day) =>
            day is DayOfWeek.Saturday or DayOfWeek.Sunday;

        /// <summary>
        /// CHALLENGE: replace the single catch-all arm with real patterns.
        /// Hint: use type patterns (int n, string s), a null pattern, and _ for the rest.
        /// </summary>
        private static string Describe(object? value) => value switch
        {
            int n  => $"int: {n}",
            string s => $"string of length {s.Length}",
            null    => "nothing",
            _       => "other"
        };
    }

    /// <summary>Base type for a simple shape hierarchy used by the type-pattern demo.</summary>
    public abstract record Shape;

    /// <summary>A circle defined by its radius.</summary>
    public record Circle(double Radius) : Shape;

    /// <summary>A rectangle defined by its width and height.</summary>
    public record Rectangle(double Width, double Height) : Shape;
}
