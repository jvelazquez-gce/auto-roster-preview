using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;

namespace Infrastructure.Database.Commands
{
    public class AddStudentSectionErrorCommand : IAddStudentSectionErrorCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AddStudentSectionErrorCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<StudentSectionError> ExecuteCommand(List<StudentSectionError> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(c => context.StudentSectionErrors.Add(c));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
