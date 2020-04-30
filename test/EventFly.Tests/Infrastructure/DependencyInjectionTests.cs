using EventFly.DependencyInjection;
using EventFly.Infrastructure.Definitions;
using EventFly.Queries;
using EventFly.TestFixture;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Infrastructure
{
    [Category(Categories.Application)]
    [Collection(Collections.Only)]
    public class DependencyInjectionTests : AggregateTestKit<DependencyInjectionTests.DummyContext>
    {
        public DependencyInjectionTests(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper, sc => sc.AddSingleton<IQueryProcessor, DummyQueryProcessor>()) { }

        public sealed class DummyContext : ContextDefinition
        {
            public DummyContext()
            {
                RegisterQuery<DummyQueryProcessor.DummyQuery, DummyQueryProcessor.DummyResult>();
            }

            public override IServiceCollection DI(IServiceCollection serviceDescriptors)
            {
                return serviceDescriptors;
            }
        }

        public sealed class DummyQueryProcessor : IQueryProcessor
        {
            public sealed class DummyQuery : IQuery<DummyResult>
            {

            }

            public sealed class DummyResult
            {
                public DummyResult(String result)
                {
                    Result = result;
                }

                public String Result { get; }
            }

            public Task<TResult> Process<TResult>(IQuery<TResult> query)
            {
                return Task.FromResult((TResult)(Object)new DummyResult("RESULT"));
            }

            public Task<Object> Process(IQuery query)
            {
                return Task.FromResult(new DummyResult("RESULT") as Object);
            }
        }

        [Fact]
        public async Task When_Registers_Deps_Via_Lambda()
        {
            var queryProcessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IQueryProcessor>();

            Assert.IsType<DummyQueryProcessor>(queryProcessor);

            var result = await queryProcessor.Process(new DummyQueryProcessor.DummyQuery());

            Assert.Equal("RESULT", result.Result);
        }
    }
}
