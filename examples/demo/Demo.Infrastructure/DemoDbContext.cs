using Demo.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure
{

    public class ProjectModel 
    {
        public ProjectId Id { get; set; }
        public bool IsDeleted { get; set;}
        public ProjectName ProjectName { get;set; }
    }
    public class DemoDbContext : DbContext
    {
        public DbSet<ProjectModel> Projects { get; }
    }
}