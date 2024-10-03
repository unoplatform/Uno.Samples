using BenchmarkDotNet.Attributes;

namespace SamplesApp.Benchmarks.Suite.TryFinally;

public class TryFinallyBenchmark
{
    [Benchmark(Baseline = true)]
    public void SingleCall()
    {
        for (int i = 0; i < 100; i++)
        {
            MyTesting.SingleCall();
        }
    }


    [Benchmark()]
    public void WithTryFinallyOneMethod()
    {
        for (int i = 0; i < 100; i++)
        {
            MyTesting.WithTryFinallyOneMethod();
        }
    }

    [Benchmark()]
    public void WithTryFinallyTwoMethods()
    {
        for (int i = 0; i < 100; i++)
        {
            MyTesting.WithTryFinallyTwoMethods();
        }
    }
}

public class MyTesting
{
    public static int counter;

    public static void SingleCall()
    {
        MyMethod();
    }

    public static void WithTryFinallyOneMethod()
    {
        try
        {
            MyMethod();
        }
        finally
        {
            counter++;
        }
    }

    public static void WithTryFinallyTwoMethods()
    {
        try
        {
            MyMethod();
            MyMethod();
        }
        finally
        {
            counter++;
        }
    }

    private static void MyMethod()
    {
    }
}
