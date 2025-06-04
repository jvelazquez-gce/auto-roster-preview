using System;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;
using Infrastructure.Database.Context;

namespace Infrastructure.Database.Commands.LastClassGroups
{
    public class DeleteAllLastClassGroupMissingFromPreLoadCommand : IDeleteAllLastClassGroupMissingFromPreLoadCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DeleteAllLastClassGroupMissingFromPreLoadCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void ExecuteCommand()
        {
            using var context = _dbContextFactory.CreateDbContext();

            var startDate = DateTime.Today;
            var lastClassTogetherCohort = context.LastClassGroupStudentSections
                .Where(w => w.StatusID == StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE)
                .Where(w => w.StartDate >= startDate)
                .ToList();

            var itemsFound = false;
            foreach (var lastClassRec in from lastClassRec in lastClassTogetherCohort 
                let foundRecord =
                    context.PreLoadStudentSections
                    .Where(w => w.SyStudentID == lastClassRec.SyStudentID)
                    .Where(w => w.AdCourseID == lastClassRec.AdCourseID)
                    .Where(w => w.StartDate == lastClassRec.StartDate)
                    .Where(w => w.AdClassSchedID == lastClassRec.OldSectionId)
                    .FirstOrDefault(w => w.LastAdClassSchedIDTaken == lastClassRec.LastAdClassSchedIDTaken) 
                where foundRecord == null
                select lastClassRec)
            {
                itemsFound = true;
                context.LastClassGroupStudentSections.Remove(lastClassRec);
            }

            if(itemsFound) context.SaveChanges();
        }
    }
}
