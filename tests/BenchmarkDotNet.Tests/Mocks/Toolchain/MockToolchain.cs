using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Parameters;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Validators;

namespace BenchmarkDotNet.Tests.Mocks.Toolchain
{
    public class MockToolchain : IToolchain
    {
        public string Name => nameof(MockToolchain);
        public IGenerator Generator => new MockGenerator();
        public IBuilder Builder => new MockBuilder();
        public IExecutor Executor => new MockExecutor();
        public bool IsInProcess => false;
        public IEnumerable<ValidationError> Validate(BenchmarkCase benchmarkCase, IResolver resolver) => ImmutableArray<ValidationError>.Empty;

        public override string ToString() => GetType().Name;

        private class MockGenerator : IGenerator
        {
            public GenerateResult GenerateProject(BuildPartition buildPartition, ILogger logger, string rootArtifactsFolderPath)
                => GenerateResult.Success(ArtifactsPaths.Empty, ImmutableArray<string>.Empty);
        }

        private class MockBuilder : IBuilder
        {
            public BuildResult Build(GenerateResult generateResult, BuildPartition buildPartition, ILogger logger) => BuildResult.Success(generateResult);
        }

        private class MockExecutor : IExecutor
        {
            public ExecuteResult Execute(ExecuteParameters executeParameters)
            {
                var mockMeasurerAttribute = executeParameters.BenchmarkCase.Descriptor.Type.GetCustomAttribute<MockMeasurerAttribute>();
                if (mockMeasurerAttribute == null)
                    return ExecuteResult.CreateFailed($"You should define {nameof(MockMeasurerAttribute)} for the class with benchmarks");
                if (mockMeasurerAttribute.MockMeasurerType == null)
                    return ExecuteResult.CreateFailed($"{nameof(MockMeasurerAttribute)} must have non-null MockMeasurerType");
                if (!mockMeasurerAttribute.MockMeasurerType.GetInterfaces().Contains(typeof(IMockMeasurer)))
                    throw new ArgumentException($"Type '{mockMeasurerAttribute.MockMeasurerType.Name}' must implement {nameof(IMockMeasurer)}");

                var measurer = Activator.CreateInstance(mockMeasurerAttribute.MockMeasurerType) as IMockMeasurer;
                if (measurer == null)
                    return ExecuteResult.CreateFailed($"Failed to create an instance of {mockMeasurerAttribute.MockMeasurerType.Name}");

                var measurements = measurer.Measure(executeParameters.BenchmarkCase);
                return new ExecuteResult(measurements);
            }
        }
    }
}