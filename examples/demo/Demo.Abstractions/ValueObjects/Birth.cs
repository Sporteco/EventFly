using System;
using EventFly.ValueObjects;

namespace Demo.ValueObjects
{
    public class Birth : SingleValueObject<DateTime>
    {
        public Birth(DateTime value) : base(value){}
    }
}