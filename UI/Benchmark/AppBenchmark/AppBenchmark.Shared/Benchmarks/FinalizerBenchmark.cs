using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplesApp.Benchmarks.Suite.FinalizerBench
{
    public class FinalizerBenchmark
    {
        [Benchmark(Baseline = true)]
        public void NewHandles()
        {
            for (int i = 0; i < 100; i++)
            {
                new WithFinalizer();
            }
        }


        [Benchmark()]
        public void ReuseHandle()
        {
            for (int i = 0; i < 100; i++)
            {
                new WithoutFinalizer();
            }

        }

        private class WithFinalizer
        {
            ~WithFinalizer()
            {

            }
        }

        private class WithoutFinalizer
        {

        }
    }
}
