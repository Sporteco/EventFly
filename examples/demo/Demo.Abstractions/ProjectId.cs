using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public sealed class ProjectId : Identity<ProjectId>
    {
        public ProjectId(string value) : base(value) { }
    }
}
