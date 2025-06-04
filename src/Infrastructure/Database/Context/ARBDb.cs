using System;
using Domain.Entities;
using Domain.Entities.Other;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Database.Context
{
    public class ARBDb : DbContext, IDisposable
    {
        private string _connectionString = string.Empty;
        public ARBDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Status> Status { get; set; }
        public virtual DbSet<PreLoadStudentSection> PreLoadStudentSections { get; set; }
        public DbSet<OneStudentPerSectionRecord> OneStudentPerSectionRecords { get; set; }
        public virtual DbSet<LastClassGroupStudentSection> LastClassGroupStudentSections { get; set; }
        public virtual DbSet<ClassGroupStudentSection> ClassGroupStudentSections { get; set; }
        public DbSet<PreviewStudentSection> PreviewStudentSections { get; set; }
        public DbSet<LiveStudentSection> LiveStudentSections { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<StudentSectionError> StudentSectionErrors { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ClassesTogetherToGroupRule> ClassesTogetherToGroupRules { get; set; }
        public DbSet<Rule> Rules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString, builder =>
            {
                builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                builder.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
