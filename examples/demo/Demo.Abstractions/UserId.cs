using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class UserId : Identity<UserId>
    {
        public UserId(System.String value) : base(value) { }
    }
}
