using System;
using Akkatecture.ValueObjects;

namespace Demo.ValueObjects
{
    public class Birth : SingleValueObject<DateTime>
    {
        public Birth(DateTime value) : base(value){}
    }
}