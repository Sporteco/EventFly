using Akkatecture.Core;

namespace Demo
{
    public class UserId : Identity<UserId>
    {
        public UserId(string value) : base(value){}
    }

}
