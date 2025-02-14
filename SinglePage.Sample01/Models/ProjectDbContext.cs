using Microsoft.EntityFrameworkCore;
using System;
using SinglePage.Sample01.Models.DomainModels.PersonAggregates;

namespace SinglePage.Sample01.Models
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Person> Person { get; set; }
    }
}
