using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    public class EventPosterId : Identity<EventPosterId>
    {
        public EventPosterId(string value) : base(value)
        {
        }
    }
}