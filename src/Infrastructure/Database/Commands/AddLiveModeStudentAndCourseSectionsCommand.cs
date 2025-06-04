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
    public class AddLiveModeStudentAndCourseSectionsCommand : IAddLiveModeStudentAndCourseSectionsCommand
    {
        private readonly IMapper _mapper;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IUpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand _updateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand;

        public AddLiveModeStudentAndCourseSectionsCommand(
            IDbContextFactory dbContextFactory, 
            IUpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand updateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand,
            IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _updateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand = updateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand;
        }

        public void Execute(CalcModel calculatedModel, Job job)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(
                () =>
                {
                    RunExecute(calculatedModel, job, context);
                });
        }

        private void RunExecute(CalcModel calculatedModel, Job job, ARBDb context)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    // s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION - this line is here because the code is not re-adding sections that already exists for the 1:1 rule
                    List<CourseSection> courseSectionsToRemoveAndReAdd = context.CourseSections
                        .Where(s => s.AdCourseID == calculatedModel.CourseID)
                        .Where(s => s.StartDate == calculatedModel.StartDate)
                        .Where(s => s.HasSectionBeenCreatedOrUpdatedInCampusVue == false)
                        //.Where(s => s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION
                        .Where(s => s.StatusID != SectionStatus.INACTIVE)
                        .ToList();

                    calculatedModel.CourseSectionsThatWereNotReUsed.ForEach(c => courseSectionsToRemoveAndReAdd.RemoveAll(w => w.Id == c.Id));

                    if (courseSectionsToRemoveAndReAdd.Count > 0)
                    {
                        List<Guid> sectionGuidsToBeReAdded = calculatedModel
                            .CourseSections.Select(c => c.Id)
                            .ToList();

                        sectionGuidsToBeReAdded.AddRange(
                                // Ready to cancel one student per sections are already saved to the db, so they need to be deleted so they can be re-added.
                                calculatedModel.CourseSectionsToBeCancelled
                                .Where(w => w.GroupCategory == ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                                .Select(c => c.Id)
                                .ToList()
                        );

                        foreach (var course in courseSectionsToRemoveAndReAdd)
                        {
                            if (sectionGuidsToBeReAdded.Contains(course.Id))
                            {
                                context.CourseSections.Remove(course);
                            }
                            else
                            {
                                MarkCourseRecordAndContextToBeInactivated(course, context);
                            }
                        }

                        context.SaveChanges();
                    }

                    calculatedModel.CourseSections.ForEach(s => context.CourseSections.Add(s));
                    calculatedModel.CourseSectionsToBeCancelled.ForEach(s => context.CourseSections.Add(s));
                    context.SaveChanges();

                    calculatedModel.LiveStudentRecords.ForEach(s => context.LiveStudentSections.Add(s));

                    if (calculatedModel.OneStudentPerSectionStudentRecords.Any())
                    {
                        var oneStudentPerSectionResults = new OneStudentPerSectionResults();
                        oneStudentPerSectionResults.LiveStudentSectionList = calculatedModel.LiveStudentRecords;
                        oneStudentPerSectionResults.OneStudentPerSectionList = calculatedModel.OneStudentPerSectionStudentRecords;
                        _updateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand.ExecuteCommand(oneStudentPerSectionResults);
                    }

                    var excludedStudentSections = new List<LiveStudentSection>();
                    _mapper.Map<List<PreLoadStudentSection>, List<LiveStudentSection>>(calculatedModel.SectionsToExclude, excludedStudentSections);

                    if (excludedStudentSections.Count > 0)
                    {
                        foreach (var record in excludedStudentSections)
                        {
                            record.JobID = job.Id;
                            context.LiveStudentSections.Add(record);
                        }
                    }
                    context.SaveChanges();
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
