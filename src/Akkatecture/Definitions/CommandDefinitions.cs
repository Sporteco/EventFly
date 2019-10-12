// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.CommandDefinitions
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Commands;
using Akkatecture.Core.VersionedTypes;
using System;

namespace Akkatecture.Definitions
{
  public class CommandDefinitions : VersionedTypeDefinitions<ICommand, CommandVersionAttribute, CommandDefinition>, ICommandDefinitions
  {
    protected override CommandDefinition CreateDefinition(
      int version,
      Type type,
      string name)
    {
      return new CommandDefinition(version, type, name);
    }
  }
}
