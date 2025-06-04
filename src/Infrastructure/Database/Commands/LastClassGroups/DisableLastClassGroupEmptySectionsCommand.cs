using System;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands.LastClassGroups
{
    public class DisableLastClassGroupEmptySectionsCommand : IDisableLastClassGroupEmptySectionsCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DisableLastClassGroupEmptySectionsCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void ExecuteCommand()
        {
            using var context = _dbContextFactory.CreateDbContext();

            var startDate = DateTime.Today;
            var lctCourseSections = context.CourseSections
                .Where(w => w.GroupCategory == ARBGroupCategory.LAST_CLASS_TOGETHER_COHORT)
                .Where(w => w.StatusID == SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE)
                .Where(w => w.Active || w.IsManagedByARB)
                .Where(w => !w.HasInstructor)
                .Where(w => w.StartDate >= startDate)
                .OrderBy(o => o.StartDate)
                .ToList();

            var lastClassTogetherCohort = context.LastClassGroupStudentSections
                .Where(w => w.StatusID == StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE)
                .Where(w => w.StartDate >= startDate)
                .ToList();

            var itemsFound = false;
            foreach (var courseSection in from courseSection in lctCourseSections
                                          let foundRecord = lastClassTogetherCohort
                                              .Any(w => w.SectionGuid == courseSection.Id)
                                          where !foundRecord
                                          select courseSection)
            {
                itemsFound = true;
                courseSection.StatusID = SectionStatus.INACTIVE;
                courseSection.Active = false;
                courseSection.IsManagedByARB = false;
                courseSection.UpdatedBy = "DisableLastClassGroupEmptySectionsCommand";
                courseSection.UpdatedOn = DateTime.Now;
                context.Entry(courseSection).State = EntityState.Modified;
            }

            if (itemsFound) context.SaveChanges();
        }
    }
}
