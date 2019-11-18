using System;

namespace EventFly.Queries
{
    public interface IQuery { }

    public interface IQuery<TResult> : IQuery { }

    public interface ICachedQuery
    {
        TimeSpan Expire { get; }
    }
}
