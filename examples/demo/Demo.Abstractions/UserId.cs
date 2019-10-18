using EventFly.Core;
using EventFly.ValueObjects;
using Newtonsoft.Json;

namespace Demo
{
    public class UserId : Identity<UserId>
    {
        public UserId(string value) : base(value){}
    }
}
