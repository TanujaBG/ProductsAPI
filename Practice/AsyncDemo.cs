using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Practice
{
    /// <summary>
    /// Day 1 — async/await + parallelism live demo.
    /// Shows, on YOUR machine: thread-pool sizing, the thread swap across an 'await',
    /// sequential vs concurrent (WhenAll) I/O timing, CPU parallelism, and a race condition.
    /// </summary>
    public static class AsyncDemo
    {
        /// <summary>Runs every part of the demo in order.</summary>
        public static async Task RunAsync()
        {
            Part1_MachineInfo();
            await Part2_ThreadSwapAcrossAwait();
            await Part3_SequentialVsConcurrent();
            Part4_CpuParallelism();
            Part5_RaceCondition();
        }

        /// <summary>Prints core count and thread-pool min/max sizes for THIS machine.</summary>
        private static void Part1_MachineInfo()
        {
            Console.WriteLine("=== Part 1: Machine & thread pool ===");
            Console.WriteLine($"Logical processors (cores): {Environment.ProcessorCount}");
            ThreadPool.GetMinThreads(out int minWorker, out int minIo);
            ThreadPool.GetMaxThreads(out int maxWorker, out int maxIo);
            Console.WriteLine($"Thread pool min: worker={minWorker}, io={minIo}");
            Console.WriteLine($"Thread pool max: worker={maxWorker}, io={maxIo}");
            Console.WriteLine();
        }

        /// <summary>Shows the thread ID before and after an await — usually a different thread.</summary>
        private static async Task Part2_ThreadSwapAcrossAwait()
        {
            Console.WriteLine("=== Part 2: Thread swap across 'await' ===");
            Console.WriteLine($"Before await: thread #{Environment.CurrentManagedThreadId}");
            await Task.Delay(100); // simulates I/O; NO thread is used during this wait
            Console.WriteLine($"After  await: thread #{Environment.CurrentManagedThreadId}");
            Console.WriteLine("(The thread was returned to the pool during the 100ms wait.)");
            Console.WriteLine();
        }

        /// <summary>Compares awaiting 3 'I/O' calls one-by-one vs all at once with WhenAll.</summary>
        private static async Task Part3_SequentialVsConcurrent()
        {
            Console.WriteLine("=== Part 3: Sequential vs concurrent I/O (each call 'waits' 500ms) ===");

            var stopwatch = Stopwatch.StartNew();
            await FakeIoAsync();
            await FakeIoAsync();
            await FakeIoAsync();
            stopwatch.Stop();
            Console.WriteLine($"Sequential (await one by one): {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            Task a = FakeIoAsync();
            Task b = FakeIoAsync();
            Task c = FakeIoAsync();
            await Task.WhenAll(a, b, c);
            stopwatch.Stop();
            Console.WriteLine($"Concurrent (WhenAll):          {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine();
        }

        /// <summary>Simulates a 500ms I/O operation without consuming a thread.</summary>
        private static Task FakeIoAsync() => Task.Delay(500);

        /// <summary>Compares a plain loop vs Parallel.For for CPU-bound work.</summary>
        private static void Part4_CpuParallelism()
        {
            Console.WriteLine("=== Part 4: CPU work — sequential loop vs Parallel.For ===");
            const int iterations = 20;

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) BurnCpu();
            stopwatch.Stop();
            Console.WriteLine($"Sequential for: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            Parallel.For(0, iterations, _ => BurnCpu());
            stopwatch.Stop();
            Console.WriteLine($"Parallel.For:   {stopwatch.ElapsedMilliseconds} ms (spreads across all cores)");
            Console.WriteLine();
        }

        /// <summary>A deliberately CPU-heavy method (spins doing math) used to show parallel speed-up.</summary>
        private static void BurnCpu()
        {
            double sum = 0;
            for (int i = 0; i < 5_000_000; i++) sum += Math.Sqrt(i);
            if (sum < 0) Console.WriteLine(sum); // prevents the JIT from optimizing the loop away
        }

        /// <summary>Demonstrates a race condition on shared state and its correct fix.</summary>
        private static void Part5_RaceCondition()
        {
            Console.WriteLine("=== Part 5: Race condition in Parallel.For ===");
            const int count = 1_000_000;

            long buggyTotal = 0;
            Parallel.For(0, count, i => buggyTotal++);                       // BUG: unsynchronized shared write
            Console.WriteLine($"Buggy total: {buggyTotal:n0}  (expected {count:n0})");

            long safeTotal = 0;
            Parallel.For(0, count, i => Interlocked.Increment(ref safeTotal)); // FIX: atomic increment
            Console.WriteLine($"Safe  total: {safeTotal:n0}  (expected {count:n0})");
            Console.WriteLine();
        }
    }
}
