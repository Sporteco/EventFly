using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class EventPosterId : Identity<EventPosterId>
    {
        public EventPosterId(string value) : base(value)
        {
        }
    }
}