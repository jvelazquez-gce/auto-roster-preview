using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;
using AutoMapper;
using Domain.Interfaces.Infrastructure.Database.Commands;

namespace Services.OneStudentPerSection
{
    public class OneStudentPerSectionHandler : IOneStudentPerSectionHandler
    {
        private readonly IMapper _mapper;
        private readonly IOneStudentPerSectionErrorChecking _oneStudentPerSectionErrorChecking;
        private readonly IOneStudentPerSectionCVueSectionsHandler _oneStudentPerSectionCVueSectionsHandler;
        private readonly IOneStudentPerSectionCourseSectionHandler _oneStudentPerSectionCourseSectionHandler;

        public OneStudentPerSectionHandler(
            IMapper mapper, 
            IOneStudentPerSectionErrorChecking oneStudentPerSectionErrorChecking, 
            IOneStudentPerSectionCVueSectionsHandler oneStudentPerSectionCVueSectionsHandler,
            IOneStudentPerSectionCourseSectionHandler oneStudentPerSectionCourseSectionHandler)
        {
            _mapper = mapper;
            _oneStudentPerSectionErrorChecking = oneStudentPerSectionErrorChecking;
            _oneStudentPerSectionCVueSectionsHandler = oneStudentPerSectionCVueSectionsHandler;
            _oneStudentPerSectionCourseSectionHandler = oneStudentPerSectionCourseSectionHandler;
        }

        public OneStudentPerSectionResults ProcessRecordsAndGetCalcModel(List<PreLoadStudentSection> scVueSctionsWithSameCourseAndStartDate, 
            IConfigurationRepository configurationRepository, CalcModel calcModel, Parameters p)
        {
            UpdateParameterObjectValues(calcModel, p);
            LoadDataToProcessAndRemoveRecordsFromListPassedInAndRunErrorChecks(scVueSctionsWithSameCourseAndStartDate, p, configurationRepository);
            var results = ExcecuteLogic(p, configurationRepository);
            return results;
        }

        public void UpdateParameterObjectValues(CalcModel calcModel, Parameters p)
        {
            p.CalcModel = new CalcModel() { CourseID = calcModel.CourseID, StartDate = calcModel.StartDate, JobStatusId = calcModel.JobStatusId }; 
            p.QueryInputParams.StartDate = calcModel.StartDate;
            p.QueryInputParams.AdCourseID = calcModel.CourseID;
            
            // re-set values from previous start-date/course-id values
            p.ListOfRecordsFromDbMissingMatchingRecordFromCVue = new List<OneStudentPerSectionRecord>();
        }

        public void LoadDataToProcessAndRemoveRecordsFromListPassedInAndRunErrorChecks(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            Parameters p, 
            IConfigurationRepository configurationRepository)
        {
            p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue
                = sectionsWithSameCourseAndStartDate.Where(
                    w => w.ReuseEmptySections == false).ToList();
            // && w.TargetStudentCount == 1).ToList();

            p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue.ForEach(r => sectionsWithSameCourseAndStartDate.Remove(r));
            var mixRecordsSharingSameSection = FindMixRecordsSharingSameSection(sectionsWithSameCourseAndStartDate, p);
            mixRecordsSharingSameSection.ForEach(r => sectionsWithSameCourseAndStartDate.Remove(r));

            p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue = CheckForRecordsWithDataErrorsAndRemoveThem(p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue, 
                                                                                    p.AddStudentSectionErrorCommand, p.Job, configurationRepository, mixRecordsSharingSameSection);

            _mapper.Map<List<PreLoadStudentSection>, List<OneStudentPerSectionRecord>>(p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue,
                p.OneStudentPerSectionsWithSameCourseAndStartDateFromCVue);
        }

        public List<PreLoadStudentSection> FindMixRecordsSharingSameSection(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, Parameters p)
        {
            List<int> uniqueAdClassSchedIDs = p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue
                                                    .Select(s => s.AdClassSchedID)
                                                        .Distinct().ToList();

            var mixRecordsSharingSameSection = sectionsWithSameCourseAndStartDate.Where(
                    w => w.ReuseEmptySections == true
                        && uniqueAdClassSchedIDs.Contains(w.AdClassSchedID)).ToList();

            return mixRecordsSharingSameSection;
        }

        // The value sectionsWithSameCourseAndStartDate gets updated and returned. If the returned values is not used beyond this method
        // that will be an issue since that parameter only gets updated in the scope of this method and not outside.
        public List<PreLoadStudentSection> CheckForRecordsWithDataErrorsAndRemoveThem(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            IAddStudentSectionErrorCommand AddStudentSectionErrorCommand, 
            Job job, 
            IConfigurationRepository configurationRepository,
            List<PreLoadStudentSection> mixRecordsSharingSameSection)
        {
            List<PreLoadStudentSection> listOfRecordsToRemove = new List<PreLoadStudentSection>();
            foreach (var record in sectionsWithSameCourseAndStartDate)
                if (_oneStudentPerSectionErrorChecking.RunErrorChecks(record, configurationRepository, job).ErrorFound)
                    listOfRecordsToRemove.Add(record);

            foreach (var record in mixRecordsSharingSameSection)
            {
                _oneStudentPerSectionErrorChecking.SaveErrorRecordToMemory(record, StudentSectionStatus.MIX_RECORDS_NOT_ALLOWED_IN_ONE_STUDENT_PER_SECTION, job);
                listOfRecordsToRemove.Add(record);
            }

            _oneStudentPerSectionErrorChecking.SaveErrorRecordsToDbAndRemoveThemFromMemory(AddStudentSectionErrorCommand, job);

            listOfRecordsToRemove.ForEach(r => sectionsWithSameCourseAndStartDate.Remove(r));

            return sectionsWithSameCourseAndStartDate;
        }

        public OneStudentPerSectionResults ExcecuteLogic(
            Parameters p, 
            IConfigurationRepository configurationRepository)
        {
            var results = _oneStudentPerSectionCVueSectionsHandler.AddUpdateAndInactivateCampusVueSectionsToCancel(p);
            SetUpListsToCreateCancelAndRemove(p);
            UpdateStudentRecordsWithNewSectionName(p);

            results = _oneStudentPerSectionCourseSectionHandler.ExecuteCourseSectionLogic(p, configurationRepository, results);
            // if running in live mode, get records that need to be created/updated in campus vue
            if (p.Job.StatusId == JobStatus.RUNNING_LIVE_JOB)
            {
                p.CalcModel.CourseSectionsToBeCancelled = p.GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery.ExecuteQuery(p.QueryInputParams);
                p.CalcModel.CourseSections = p.GetActiveOneStudentPerSectionCourseSectionsToCreateQuery.ExecuteQuery(p.QueryInputParams);
                p.CalcModel.OneStudentPerSectionStudentRecords = p.GetPreviewOneStudentPerSectionStudentsToTransferQuery.ExecuteQuery(p.QueryInputParams);
            }
            else
            {
                // if an preview mode, set to empty lists to make sure they don't get re-added accidently
                p.CalcModel.CourseSectionsToBeCancelled = new List<CourseSection>();
                p.CalcModel.CourseSections = new List<CourseSection>();
                p.CalcModel.OneStudentPerSectionStudentRecords = new List<OneStudentPerSectionRecord>();
            }
            results.CalcModel = p.CalcModel;

            return results;
        }

        public void SetUpListsToCreateCancelAndRemove(Parameters p)
        {
            p.OneStudentPerSectionRecordFromDb = p.GetPreviewOneStudentPerSectionStudentsToTransferQuery.ExecuteQuery(p.QueryInputParams);
            p.OneStudentPerSectionRecordToCreate = p.OneStudentPerSectionsWithSameCourseAndStartDateFromCVue;
            List<OneStudentPerSectionRecord> listOfRecordsFromCVueToRemoveBecauseTheyAlreayExist = new List<OneStudentPerSectionRecord>();

            foreach (var dbRecord in p.OneStudentPerSectionRecordFromDb)
            {
                var cVueRecords = p.OneStudentPerSectionsWithSameCourseAndStartDateFromCVue
                    .Where(w => w.AdCourseID == dbRecord.AdCourseID)
                    .Where(w => w.StartDate == dbRecord.StartDate)
                    .Where(w => w.SyStudentID == dbRecord.SyStudentID)
                    .ToList();

                if (cVueRecords.Any())
                    listOfRecordsFromCVueToRemoveBecauseTheyAlreayExist.AddRange(cVueRecords);
                else
                    p.ListOfRecordsFromDbMissingMatchingRecordFromCVue.Add(dbRecord);
            }
            listOfRecordsFromCVueToRemoveBecauseTheyAlreayExist.ForEach(r => p.OneStudentPerSectionRecordToCreate.Remove(r));

            p.OneStudentPerSectionCourseSectionsReadyToCreateOrCreated = p.GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery.ExecuteQuery(p.QueryInputParams);
        }

        public void UpdateStudentRecordsWithNewSectionName(Parameters p)
        {
            string sectionPrefixName = GeneralStatus.NOCOHORT_NOLOCATION_NAME_PREFIX;
            int counter = GetNextCalculatedSectionNameBaseCounter(p);
            List<string> sectionNames = new SectionCodeCalculator().CreateGroupNames(p.OneStudentPerSectionRecordToCreate.Count, sectionPrefixName, ref counter);
            int index = 0;
            foreach (var sectionName in sectionNames)
            {
                p.OneStudentPerSectionRecordToCreate[index].JobID = p.Job.Id;
                p.OneStudentPerSectionRecordToCreate[index].SectionCode = sectionName;
                p.OneStudentPerSectionRecordToCreate[index].GroupCategory = ARBGroupCategory.ONE_STUDENT_PER_SECTION;
                p.OneStudentPerSectionRecordToCreate[index].StatusID = StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE;
                index++;
            }
        }

        public int GetNextCalculatedSectionNameBaseCounter(Parameters p)
        {
            // This code also takes into account the sections already created in case the live job fails and it has to run 
            // again and thus then having to deal with sections names that are already there to not re-use them.
            List<string> sectionNames = p.OneStudentPerSectionCourseSectionsReadyToCreateOrCreated
                                            .Where(w => !w.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER))
                                            .Select(s => s.SectionCode)
                                            .Distinct()
                                            .ToList();

            List<int> sectionNumbers = new List<int>();
            foreach(var sectionName in sectionNames)
            {
                int sectionNumber = Convert.ToInt32(sectionName.Substring(1));
                sectionNumbers.Add(sectionNumber);
            }
            if (sectionNumbers.Any())
            {
                int lastBaseCounter = sectionNumbers.OrderByDescending(o => o).First();
                return ++lastBaseCounter;
            }

            return GeneralStatus.ONE_STUDENT_PER_SECTION_BASE_COUNTER; 
        }
    }
}
