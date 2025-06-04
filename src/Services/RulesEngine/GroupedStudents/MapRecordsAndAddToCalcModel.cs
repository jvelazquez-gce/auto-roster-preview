using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;
using AutoMapper;
using Infrastructure.Utilities;

namespace Services.RulesEngine.GroupedStudents
{
    public class MapRecordsAndAddToCalcModel : IMapRecordsAndAddToCalcModel
    {
        private Job _job;
        private List<PreLoadStudentSection> _groupList;
        private readonly IMapper _mapper;
        private readonly IConfigInstance _configInstance;

        public MapRecordsAndAddToCalcModel(
            IMapper mapper, 
            Job job, 
            List<PreLoadStudentSection> groupList,
            IConfigInstance configInstance)
        {
            _mapper = mapper;
            _job = job;
            _groupList = groupList;
            _configInstance = configInstance;
        }

        public void UpdateSectionStatusToSuccessAndAddSectionToFinalModel(CalcModel calculatedModel, ClassGroupStudentSection studentSection)
        {
            var courseStartDateMinusOneDay = studentSection.StartDate.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            // This is done, so ARB does not delete the preview data when it runs in live mode
            if (courseStartDateMinusOneDay >= _configInstance.GetCutOffStartDateTime() &&
                courseStartDateMinusOneDay <= _configInstance.GetCutOffEndDateTime())
                studentSection.OneDayBeforeRunningLive = true;

            studentSection.StatusID = StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE;
            calculatedModel.ClassGroupStudentSections.Add(studentSection);
        }

        public void AddAllStudentsToModelInOneSection(CalcModel calculatedModel, List<PreLoadStudentSection> distinctGroupNumberList,
            PreLoadStudentSection firstRecord, string mainGroupNameSuffix)
        {
            var groupSectionList = new List<ClassGroupStudentSection>();
            _mapper.Map<List<PreLoadStudentSection>, List<ClassGroupStudentSection>>(distinctGroupNumberList, groupSectionList);
            foreach (var record in groupSectionList)
            {
                record.JobID = _job.Id;
                record.SectionCode = mainGroupNameSuffix;
                record.GroupCategory = firstRecord.GroupCategory;
                record.GroupNumber = firstRecord.GroupNumber;
                //if (firstRecord.LastAdClassSchedIDTaken == record.LastAdClassSchedIDTaken)
                //{
                //    record.GroupNumber = firstRecord.GroupNumber;
                //}
                UpdateSectionStatusToSuccessAndAddSectionToFinalModel(calculatedModel, record);
           }

            distinctGroupNumberList.ForEach(r => _groupList.Remove(r));
            distinctGroupNumberList = new List<PreLoadStudentSection>();
        }

        public void BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(CalcModel calculatedModel, PreLoadStudentSection firstRecord,
            List<PreLoadStudentSection> distinctSuperSectionList, string groupNameSuffix, ref int counter)
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

                var tempSuperSectionList = distinctSuperSectionList.Take(studentsToAddToSection).ToList();
                var tempSectionList = new List<ClassGroupStudentSection>();
                _mapper.Map<List<PreLoadStudentSection>, List<ClassGroupStudentSection>>(tempSuperSectionList, tempSectionList);
                foreach (var record in tempSectionList)
                {
                    record.JobID = _job.Id;
                    record.SectionCode = groupName;
                    record.GroupCategory = firstRecord.GroupCategory;
                    record.GroupNumber = firstRecord.GroupNumber;
                    UpdateSectionStatusToSuccessAndAddSectionToFinalModel(calculatedModel, record);
                }

                foreach (var rec in tempSuperSectionList)
                {
                    distinctSuperSectionList.Remove(rec);
                    _groupList.Remove(rec);
                }
            }
        }
    }
}
