// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.JobDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using EventFly.Jobs;

namespace EventFly.Definitions
{
    public interface IJobDefinition
    {
        JobName Name { get; }
        Type Type { get; }
        Type IdentityType { get; }
        IJobManagerDefinition ManagerDefinition { get; }
    }

    public interface IJobManagerDefinition
    {
        Type JobSchedulreType { get; }
        Type JobRunnerType { get; }
        Type JobType { get; }
        Type IdentityType { get; }
    }
}
