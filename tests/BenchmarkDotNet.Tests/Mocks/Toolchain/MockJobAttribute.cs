using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Tests.Mocks.Toolchain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MockJobAttribute : JobConfigBaseAttribute
    {
        [PublicAPI]
        public MockJobAttribute(string id = null) : base(CreateMockJob(id)) { }

        private static Job CreateMockJob(string id)
        {
            var job = new Job(id)
            {
                Infrastructure =
                {
                    Toolchain = new MockToolchain()
                }
            };
            return job.Freeze();
        }
    }
}