using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;
using AutoMapper;

namespace Services.Calculators
{
    public class Calculator : ICalculator
    {
        private readonly IMapper _mapper;
        private readonly IConfigInstance _configInstance;

        public Calculator(IConfigInstance configInstance, IMapper mapper)
        {
            _mapper = mapper;
            _configInstance = configInstance;
        }

        public void CalculateSectionsNeeded(PreLoadStudentSection firstRecord, List<PreLoadStudentSection> studentSectionList, CalcModel calculatedModel)
        {
            // Number of sections needed is a rounded number.
            var totalStudentsRegistered = (double)studentSectionList.Count;
            calculatedModel.TotalStudentsRegistered = (int)totalStudentsRegistered;
            var maximumNumberOfStudentsPerSection = (double)GetMaxNumberOfStudentsPerSection(firstRecord.TargetStudentCount, firstRecord.GroupNumber, firstRecord.GroupTargetStudentCount);

            calculatedModel.MaxStudentsPerSection = (int)maximumNumberOfStudentsPerSection;
            var sectionsNeeded = (int)Math.Ceiling(totalStudentsRegistered / maximumNumberOfStudentsPerSection);
            calculatedModel.TotalSectionsNeeded = sectionsNeeded;
        }

        public void UpdateSectionStatusToSuccessAndAddSectionToFinalModel(CalcModel calculatedModel, PreviewStudentSection studentSection)
        {
            var courseStartDateMinusOneDay = studentSection.StartDate.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            // This is done, so ARB does not delete the preview data when it runs in live mode
            if (courseStartDateMinusOneDay >= _configInstance.GetCutOffStartDateTime() &&
                courseStartDateMinusOneDay <= _configInstance.GetCutOffEndDateTime())
                studentSection.OneDayBeforeRunningLive = true;

            studentSection.StatusID = SectionStatus.ACTIVE;
            calculatedModel.PreviewStudentRecords.Add(studentSection);
        }

        public void AddAllStudentsToModelInOneSection(CalcModel calculatedModel, List<PreLoadStudentSection> distinctGroupNumberList,
            PreLoadStudentSection firstRecord, string mainGroupNameSuffix, Job job)
        {
            var groupSectionList = new List<PreviewStudentSection>();
            _mapper.Map<List<PreLoadStudentSection>, List<PreviewStudentSection>>(distinctGroupNumberList, groupSectionList);

            foreach(var record in groupSectionList)
            {
                record.JobID = job.Id;
                record.SectionCode = mainGroupNameSuffix;
                record.GroupCategory = firstRecord.GroupCategory;
                UpdateSectionStatusToSuccessAndAddSectionToFinalModel(calculatedModel, record);
            }
        }

        public void BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(CalcModel calculatedModel, PreLoadStudentSection firtRecord,
            List<PreLoadStudentSection> distinctSuperSectionList, string groupNameSuffix, ref int counter, Job job)
        {
            var groupNames = new SectionCodeCalculator().CreateGroupNames(calculatedModel.TotalSectionsNeeded, groupNameSuffix, ref counter);
            var numberOfSectionsNotProcessYet = groupNames.Count;

            var studentsPerSection = calculatedModel.TotalStudentsRegistered / calculatedModel.TotalSectionsNeeded;
            foreach (var groupName in groupNames)
            {
                var studentsToAddToSection = 0;

                if (numberOfSectionsNotProcessYet > 2) studentsToAddToSection = calculatedModel.MaxStudentsPerSection;
                else if (numberOfSectionsNotProcessYet == 2) studentsToAddToSection = distinctSuperSectionList.Count / 2;
                else if (numberOfSectionsNotProcessYet == 1) studentsToAddToSection = distinctSuperSectionList.Count;

                numberOfSectionsNotProcessYet--;

                var tempSuperSectionList = distinctSuperSectionList
                    .Take(studentsToAddToSection)
                    .ToList();

                var tempSectionList = new List<PreviewStudentSection>();
                _mapper.Map<List<PreLoadStudentSection>, List<PreviewStudentSection>>(tempSuperSectionList, tempSectionList);

                foreach (var record in tempSectionList)
                {
                    record.JobID = job.Id;
                    record.SectionCode = groupName;
                    record.GroupCategory = firtRecord.GroupCategory;
                    UpdateSectionStatusToSuccessAndAddSectionToFinalModel(calculatedModel, record);
                }
                tempSuperSectionList.ForEach(r => distinctSuperSectionList.Remove(r));
            }
        }

        public List<SectionTotals> GetDistinctSectionListOrderByHighestStudentsInSection(List<PreviewStudentSection> listOfSections)
        {
            var firstRecord = listOfSections.FirstOrDefault();
            if (firstRecord == null) return new List<SectionTotals>();
            var maxSeatsPerSection = firstRecord.TargetStudentCount;
            return (from s in listOfSections
                    group s by s.SectionCode into g
                    select new SectionTotals { SectionCode = g.First().SectionCode, GroupCategory = g.First().GroupCategory, 
                        TotalStudentsInSection = g.Count(), TotalEmptySeats = maxSeatsPerSection - g.Count() })
                    .OrderByDescending(r => r.TotalStudentsInSection)
                    .ToList();
        }

        public List<CourseStartDate> GetCourseStartDateListToProcess(List<PreLoadStudentSection> initialStudentRawData)
        {
            var results = new List<CourseStartDate>();

            results = initialStudentRawData.GroupBy(g => new { g.AdCourseID, g.StartDate })
                .Select(g => new CourseStartDate
                {
                    CourseID = g.First().AdCourseID,
                    StartDate = g.First().StartDate,
                }).OrderBy(r => r.StartDate)
                .ThenBy(o => o.CourseID)
                .ToList();

            return results;
        }

        public int GetMaxNumberOfStudentsPerSection(int targetStudentCount, Guid? groupNumber, int? groupTargetStudentCount)
        {
            if (groupNumber != null && (groupTargetStudentCount.HasValue && groupTargetStudentCount != 0) && groupTargetStudentCount < targetStudentCount)
                return (int)groupTargetStudentCount;

            return targetStudentCount;
        }
    }
}
