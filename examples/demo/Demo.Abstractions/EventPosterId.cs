using EventFly.Core;
using EventFly.ValueObjects;
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