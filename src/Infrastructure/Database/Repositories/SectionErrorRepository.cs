using System;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Infrastructure.Database.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories
{
    public class SectionErrorRepository : ISectionErrorRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public SectionErrorRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Add(StudentSectionError section)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.StudentSectionErrors.Add(section);
        }

        public int NumberOfErrorsThatNeedToBeSaved()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.StudentSectionErrors.Local.Count;
        }

        public IQueryable<StudentSectionError> GetErrors(int jobID, int? courseID, DateTime? startDate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            if ( courseID.HasValue && startDate.HasValue)
                return context.StudentSectionErrors
                    .Include("Status")
                    .Where(e => e.JobID == jobID)
                    .Where(e => e.AdCourseID == courseID)
                    .Where(e => e.StartDate == startDate);

            return context.StudentSectionErrors
                .Include("Status")
                .Where(e => e.JobID == jobID);
        }

        public void Save()
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.SaveChanges();
        }

        public void DeleteStudentSection(StudentSectionError studentSection)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.StudentSectionErrors.Remove(studentSection);
            context.SaveChanges();
        }
    }
}
