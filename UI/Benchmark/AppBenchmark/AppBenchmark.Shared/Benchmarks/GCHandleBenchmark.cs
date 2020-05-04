using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace SamplesApp.Benchmarks.Suite.GCHandleBench
{
	public class GCHandleBenchmark
	{
		[Benchmark(Baseline = true)]
		public void NewHandles()
		{
			int counter = 0;

			for (int i = 0; i < 100; i++)
			{
				var o = new Object();
				var h = GCHandle.Alloc(o, GCHandleType.Normal);
				var o2 = h.Target;
				if (o2 != null)
				{
					counter++;
				}
				h.Free();
			}
		}


		[Benchmark()]
		public void ReuseHandle()
		{
			int counter = 0;

			var o = new Object();
			var h = GCHandle.Alloc(o, GCHandleType.Normal);

			for (int i = 0; i < 100; i++)
			{
				var o2 = h.Target;
				if (o2 != null)
				{
					counter++;
				}
			}

			h.Free();
		}
	}
}
