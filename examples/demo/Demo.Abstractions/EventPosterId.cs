using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;
using System;

namespace Demo
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class EventPosterId : Identity<EventPosterId>
    {
        public EventPosterId(String value) : base(value) { }
    }
}