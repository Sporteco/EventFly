using EventFly.ValueObjects;

namespace Demo.ValueObjects
{
    public sealed class ProjectName : SingleValueObject<string>
    {
        public ProjectName(string value) : base(value)
        {
        }
    }
}
