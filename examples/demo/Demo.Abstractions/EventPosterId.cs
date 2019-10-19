using EventFly.Core;

namespace Demo
{
    public class EventPosterId : Identity<EventPosterId>
    {
        public EventPosterId(string value) : base(value)
        {
        }
    }
}