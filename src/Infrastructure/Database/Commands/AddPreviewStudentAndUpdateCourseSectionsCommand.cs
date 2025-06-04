using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Models.Helper;
using System.Linq;
using System;
using Infrastructure.Utilities;
using AutoMapper;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands
{
    public class AddPreviewStudentAndUpdateCourseSectionsCommand : IAddPreviewStudentAndUpdateCourseSectionsCommand
    {
        private ARBDb _context;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory _dbContextFactory;

        public AddPreviewStudentAndUpdateCourseSectionsCommand(IDbContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
        }

        public void Execute(CalcModel calculatedModel, Job job)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(
                () =>
                {
                    RunExecute(calculatedModel, job, context);
                });
        }

        private void RunExecute(CalcModel calculatedModel, Job job, ARBDb context)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION - this line is here because the code is not re-adding sections that already exists for the 1:1 rule
                    List<CourseSection> courseSectionsToDelete = _context.CourseSections
                        .Where(s => s.AdCourseID == calculatedModel.CourseID)
                        .Where(s => s.StartDate == calculatedModel.StartDate)
                        .Where(s => s.HasSectionBeenCreatedOrUpdatedInCampusVue == false)
                        .Where(s => s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                        .Where(s => s.GroupCategory != ARBGroupCategory.LAST_CLASS_TOGETHER_COHORT)
                        .Where(s => s.StatusID != SectionStatus.INACTIVE)
                        .ToList();

                    if (calculatedModel.CourseSectionsThatWereNotReUsed.Count > 0)
                    {
                        calculatedModel.CourseSectionsThatWereNotReUsed.ForEach(c => courseSectionsToDelete.RemoveAll(w => w.Id == c.Id));
                    }

                    if (courseSectionsToDelete.Count > 0)
                    {
                        List<Guid> sectionGuidsToBeReAdded =
                            calculatedModel.CourseSections
                            .Select(c => c.Id)
                            .ToList();

                        foreach (var course in courseSectionsToDelete)
                        {
                            if (sectionGuidsToBeReAdded.Contains(course.Id))
                            {
                                // Potential to loose data
                                _context.CourseSections.Remove(course);
                            }
                            else
                            {
                                MarkCourseRecordAndContextToBeInactivated(course, context);
                            }
                        }

                        _context.SaveChanges();
                    }

                    calculatedModel.CourseSections.ForEach(s => _context.CourseSections.Add(s));
                    calculatedModel.CourseSectionsToBeCancelled.ForEach(s => _context.CourseSections.Add(s));
                    calculatedModel.PreviewStudentRecords.ForEach(s => _context.PreviewStudentSections.Add(s));

                    var excludedStudentSections = new List<PreviewStudentSection>();
                    _mapper.Map<List<PreLoadStudentSection>, List<PreviewStudentSection>>(calculatedModel.SectionsToExclude, excludedStudentSections);

                    if (excludedStudentSections.Count > 0)
                    {
                        foreach (var record in excludedStudentSections)
                        {
                            record.JobID = job.Id;
                            _context.PreviewStudentSections.Add(record);
                        }
                    }

                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
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
    }
}
