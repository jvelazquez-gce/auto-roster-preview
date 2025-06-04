using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;
using System;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Infrastructure.Utilities;
using Domain.Models.Helper;

namespace Infrastructure.Database.Commands
{
    public class DeleteOrInactivateCVueSectionsTxCommand : IDeleteOrInactivateCVueSectionsTxCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DeleteOrInactivateCVueSectionsTxCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // DeleteOrInactivateCVueSectionsByCourseIdAndStartDateIfTheyHaveNotBeenUpdatedByARBInCVueYet
        public List<CourseSection> Execute(QueryInputParams queryInputParams)
        {
            using var context = _dbContextFactory.CreateDbContext();
            List<CourseSection> courseSectionsToDeleteOrInactivate = new List<CourseSection>();
            if (queryInputParams.ExcludeSuperSection)
            {
                courseSectionsToDeleteOrInactivate = context.CourseSections
                    .Where(s => s.AdCourseID == queryInputParams.AdCourseID)
                    .Where(s => s.StartDate == queryInputParams.StartDate)
                    .Where(s => s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER)
                    .Where(s => s.AdClassSchedId.HasValue)
                    .Where(s => s.AdClassSchedId != 0)
                    .Where(s => s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                    .Where(s => !s.HasSectionBeenCreatedOrUpdatedInCampusVue)
                    .ToList();
            }
            else
            {
                courseSectionsToDeleteOrInactivate = context.CourseSections
                    .Where(s => s.AdCourseID == queryInputParams.AdCourseID)
                    .Where(s => s.StartDate == queryInputParams.StartDate)
                    .Where(s => s.AdClassSchedId.HasValue)
                    .Where(s => s.AdClassSchedId != 0)
                    .Where(s => s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                    .Where(s => !s.HasSectionBeenCreatedOrUpdatedInCampusVue)
                    .ToList();
            }

            var potentialSectionsToDelete = new List<CourseSection>();
            foreach (var record in courseSectionsToDeleteOrInactivate)
            {
                if (queryInputParams.IdsToDelete.Contains((int)record.AdClassSchedId))
                {
                    potentialSectionsToDelete.Add(record);
                }
                else
                {
                    MarkCourseRecordAndContextToBeInactivated(record, context);
                }
            }

            List<CourseSection> deletedCourses = RemoveSectionsFromContextAndGetListToBeRemoved(potentialSectionsToDelete, context);
            context.SaveChanges();
            return deletedCourses;
        }

        private void MarkCourseRecordAndContextToBeInactivated(CourseSection record, ARBDb context)
        {
            record.Active = false;
            record.StatusID = SectionStatus.INACTIVE;
            // db field is 100
            record.UpdatedBy = $"{GeneralStatus.ARB_JOB} - MarkCourseRecordAndContextToBeInactivated".TruncateLongString(100);
            record.UpdatedOn = DateTime.Now;
            context.Entry(record).State = EntityState.Modified;
        }

        private List<CourseSection> RemoveSectionsFromContextAndGetListToBeRemoved(List<CourseSection> potentialSectionsToDelete, ARBDb context)
        {
            var sectionsToDelete = new List<CourseSection>();
            if (potentialSectionsToDelete.Count > 0)
            {
                List<int> adClassSchedIDlist = potentialSectionsToDelete.Select(s => (int)s.AdClassSchedId).Distinct().ToList();
                foreach (var adClassID in adClassSchedIDlist)
                {
                    List<CourseSection> duplicateList = potentialSectionsToDelete
                        .Where(c => c.AdClassSchedId == adClassID)
                        .ToList();

                    CourseSection course = duplicateList.OrderByDescending(s => s.JobID).First();
                    sectionsToDelete.Add(course);
                    context.CourseSections.Remove(course);
                }
            }
            return sectionsToDelete;
        }
    }
}
