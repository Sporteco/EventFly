using Demo.Application.CreateProject;
using System.Threading.Tasks;

namespace Demo.Infrastructure.CreateProject
{
    public sealed class TestExternalService : IExternalService
    {
        public Task DoAnything()
        {
            return Task.CompletedTask;
        }
    }
}
