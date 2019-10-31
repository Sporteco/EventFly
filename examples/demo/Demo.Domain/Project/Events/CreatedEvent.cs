﻿using Demo.ValueObjects;
using EventFly.Aggregates;

namespace Demo.Project
{
    public sealed class CreatedEvent : AggregateEvent<ProjectId>
    {
        public CreatedEvent(ProjectId projectId, ProjectName name)
        {
            ProjectId = projectId;
            Name = name;
        }

        public ProjectId ProjectId { get; }
        public ProjectName Name { get; }
    }
}
