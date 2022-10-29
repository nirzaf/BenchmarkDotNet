using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Mocks
{
    public static class MockHelper
    {
        public static Summary Run<T>(ITestOutputHelper output)
        {
            var logger = new AccumulationLogger();
            var config = DefaultConfig.Instance
                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                .AddLogger(logger);
            var summary = BenchmarkRunner.Run<T>(config);

            var exporter = MarkdownExporter.Mock;
            exporter.ExportToLog(summary, logger);
            output.WriteLine(logger.GetLog());

            return summary;
        }
    }
}