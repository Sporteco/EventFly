using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;
using System;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public sealed class ProjectId : Identity<ProjectId>
    {
        public ProjectId(String value) : base(value) { }
    }
}