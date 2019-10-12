// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.JobAggregatedDefinitions
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Core.VersionedTypes;
using Akkatecture.Jobs;

namespace Akkatecture.Definitions
{
  public class JobAggregatedDefinitions : 
      AggregatedDefinitions<JobDefinitions, JobVersionAttribute, JobDefinition>, IJobDefinitions, IVersionedTypeDefinitions<JobVersionAttribute, JobDefinition>
  {
  }
}
