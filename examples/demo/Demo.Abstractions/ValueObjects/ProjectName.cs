using EventFly.ValueObjects;

namespace Demo.ValueObjects
{
    public sealed class ProjectName : SingleValueObject<System.String>
    {
        public ProjectName(System.String value) : base(value)
        {
        }
    }
}
