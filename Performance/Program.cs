﻿using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StringPerformance>();
            Console.WriteLine(summary);
            Execute(_ => new StringPerformance().ToSnakeCaseUsingStringBuildAndSpan(), "UsingStringBuilderAndSpan");
            Execute(_ => new StringPerformance().ToSnakeCaseUsingSpanOnBuffer(), "UsingSpanOnBuffer");
            Execute(_ => new StringPerformance().ToSnakeCaseUsingRegex(), "UsingRegex");
            Execute(_ => new StringPerformance().ToSnakeCaseUsingLinq(), "UsingLinq");

            Console.ReadKey();
        }

        private static void Execute(Action<string> action, string name)
        {
            var time = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                action.Invoke(string.Empty);
            }
            time.Stop();
            Console.WriteLine($"{name}\t10\t\tTempo: {time.Elapsed}");

            time = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100_000; i++)
            {
                action.Invoke(string.Empty);
            }
            time.Stop();
            Console.WriteLine($"{name}\t100_000\t\tTempo: {time.Elapsed}");

            time = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1_000_000; i++)
            {
                action.Invoke(string.Empty);
            }
            time.Stop();
            Console.WriteLine($"{name}\t1_000_000\tTempo: {time.Elapsed}");


            Console.WriteLine($"------------------------------------");
        }
    }
}
