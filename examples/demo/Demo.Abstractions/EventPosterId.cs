using Akkatecture.Core;
using Akkatecture.ValueObjects;
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