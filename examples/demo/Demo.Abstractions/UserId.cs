using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class UserId : Identity<UserId>
    {
        public UserId(string value) : base(value){}
    }

}
