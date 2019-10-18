// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.JobDefinitions
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using EventFly.Core.VersionedTypes;
using EventFly.Jobs;

namespace EventFly.Definitions
{
  public class JobDefinitions : VersionedTypeDefinitions<IJob, JobVersionAttribute, JobDefinition>, IJobDefinitions
  {
    protected override JobDefinition CreateDefinition(
      int version,
      Type type,
      string name)
    {
      return new JobDefinition(version, type, name);
    }
  }
}
