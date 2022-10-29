using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet.Tests.Mocks.Toolchain
{
    public abstract class MockMeasurer : IMockMeasurer
    {
        public abstract List<Measurement> Measure(BenchmarkCase benchmarkCase);

        protected List<Measurement> CreateFromValues(double[] values) => values
            .Select((value, i) => new Measurement(1, IterationMode.Workload, IterationStage.Result, i, 1, value))
            .ToList();
    }
}