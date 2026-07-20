using System;
using System.Collections.Generic;
using System.Text;

namespace Practice
{

    internal class LINQ
    {
        public record Product(string Name, string Category, decimal Price, bool InStock);

        public static void Run()
        {
            // Day 1 — LINQ exercise
            // Run with:  dotnet run
            // Predict each output BEFORE running, then check yourself against what prints.
            var numbers = new List<int> { 1, 2, 3, 4, 5 };
            // Part A — filter and project
            var evenSquares = numbers
                .Where(n => n % 2 == 0)   // filter: only even numbers
                .Select(n => n * n);      // project: square each number
            Console.WriteLine("Even squares:");
            foreach (var square in evenSquares)
            {
                Console.WriteLine(square);
            }
            // Part B — aggregate
            var sumOfNumbers = numbers.Sum();
            Console.WriteLine($"\nSum of numbers: {sumOfNumbers}");
            // Part C — ordering
            var orderedNumbers = numbers.OrderByDescending(n => n);
            Console.WriteLine("\nNumbers in descending order:");
            foreach (var num in orderedNumbers)
            {
                Console.WriteLine(num);
            }
        }

        private IList<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product("Laptop", "Electronics", 999.99m, true),
                new Product("Smartphone", "Electronics", 699.99m, false),
                new Product("Desk Chair", "Furniture", 149.99m, true),
                new Product("Coffee Table", "Furniture", 89.99m, true),
                new Product("Headphones", "Electronics", 199.99m, true)
            };
        }

        private void RunProductQueries()
        {
            var products = GetSampleProducts();
            
            // Example LINQ query: Get all in-stock electronics
            var inStockElectronics = products
                .Where(p => p.Category == "Electronics" && p.InStock)
                .Select(p => p.Name);

            //projection
            products.Select(p => p.Name);                         // -> sequence of strings
            products.Select(p => new { p.Name, p.Price });        // -> reshaped objects

            //soeting 
            products.OrderBy(p => p.Price);                      // ascending
            products.OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Name);                        // tie-breaker

            //aggregation
            products.Count(p => p.InStock);     // how many in stock
            products.Sum(p => p.Price);     // total
            products.Average(p => p.Price);     // average
            products.Max(p => p.Price);     // maximum

            //Grouping
            products.GroupBy(p=> p.Category)
                .Select(g => new {Category = g.Key, Count = g.Count(), Total = g.Sum(x=>x.Price)});
        }
    }
}
