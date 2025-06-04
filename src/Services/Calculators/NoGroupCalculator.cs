using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;

namespace Services.Calculators
{
    public class NoGroupCalculator : INoGroupCalculator
    {
        private readonly IMapper _mapper;
        private readonly ICalculator _calculator;
        private readonly IGroupCalculator _groupCalculator;
        private readonly IConfigurationRepository _configurationRepository;

        public NoGroupCalculator(IMapper mapper, ICalculator calculator, IGroupCalculator groupCalculator, IConfigurationRepository configurationRepository)
        {
            _mapper = mapper;
            _calculator = calculator;
            _groupCalculator = groupCalculator;
            _configurationRepository = configurationRepository;
        }

        public void GetCalculatedSectionsModel(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, CalcModel calculatedModel, Job job)
        {
            List<PreLoadStudentSection> noGroupSectionStudentList = NonCohortPreLoadStudentRecords(sectionsWithSameCourseAndStartDate);

            PreLoadStudentSection firstRecord = noGroupSectionStudentList.FirstOrDefault();
            if (firstRecord == null) return;

            _calculator.CalculateSectionsNeeded(firstRecord, noGroupSectionStudentList, calculatedModel);
            if (calculatedModel.TotalStudentsRegistered > 0)
            {
                AddStudentsToNonExclusiveCohortSections(calculatedModel, noGroupSectionStudentList, job);
                _calculator.CalculateSectionsNeeded(firstRecord, noGroupSectionStudentList, calculatedModel);
                if (calculatedModel.TotalStudentsRegistered > 0)
                    CreateNewSections(calculatedModel, firstRecord, noGroupSectionStudentList, job);
            }
        }

        public CalcModel AddStudentsToNonExclusiveCohortSections(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList, Job job)
        {
            List<PreviewStudentSection> nonExclusiveCohortRecords = _groupCalculator.NonExclusiveCohortPreviewStudentRecords(calculatedModel);
            if (nonExclusiveCohortRecords.Count == 0) return calculatedModel;
            var uniqueBiggestToSmallestSections = _calculator.GetDistinctSectionListOrderByHighestStudentsInSection(nonExclusiveCohortRecords);
            return AddStudentsToNotExlusiveCohortSections(calculatedModel, distinctNoGroupSectionStudentList, uniqueBiggestToSmallestSections, job);
        }

        public CalcModel AddStudentsToNotExlusiveCohortSections(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList,
            List<SectionTotals> uniqueBiggestToSmallestSections, Job job)
        {
            foreach (var uniqueSection in uniqueBiggestToSmallestSections)
            {
                if (calculatedModel.TotalStudentsRegistered == 0)
                    break;

                int seatsTakenInSection = calculatedModel.PreviewStudentRecords.Where(s => s.SectionCode == uniqueSection.SectionCode).ToList().Count;
                int seatsAvailableInSection = calculatedModel.MaxStudentsPerSection - seatsTakenInSection;
                if (seatsAvailableInSection <= 0)
                    continue;

                DetermineHowManyNonGroupStudentsToMoveIntoOneSectionAndMoveThem(calculatedModel, distinctNoGroupSectionStudentList, uniqueSection, seatsAvailableInSection, job);
                PreLoadStudentSection firtRecord = distinctNoGroupSectionStudentList.FirstOrDefault();
                if (firtRecord == null) break;
                _calculator.CalculateSectionsNeeded(firtRecord, distinctNoGroupSectionStudentList, calculatedModel);
            }
            return calculatedModel;
        }

        public void DetermineHowManyNonGroupStudentsToMoveIntoOneSectionAndMoveThem(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList,
            SectionTotals section, int seatsAvailableInSection, Job job)
        {
            int numberOfNonGroupStudentsToTake = seatsAvailableInSection >= calculatedModel.TotalStudentsRegistered ? calculatedModel.TotalStudentsRegistered : seatsAvailableInSection;
            AddNonGroupStudentsToExistingSection(calculatedModel, section, numberOfNonGroupStudentsToTake, distinctNoGroupSectionStudentList, job);
        }

        public void AddNonGroupStudentsToExistingSection(CalcModel calculatedModel, SectionTotals section,
            int numberOfNonGroupStudentsToTake, List<PreLoadStudentSection> distinctNoGroupSectionStudentList, Job job)
        {
            List<PreLoadStudentSection> tempNonGroupStudents;
            List<PreviewStudentSection> tempSectionNonGroupStudents = new List<PreviewStudentSection>();
            tempNonGroupStudents = distinctNoGroupSectionStudentList.Take(numberOfNonGroupStudentsToTake).ToList();
            _mapper.Map<List<PreLoadStudentSection>, List<PreviewStudentSection>>(tempNonGroupStudents, tempSectionNonGroupStudents);
            foreach (var studentSection in tempSectionNonGroupStudents)
            {
                string oldSectionCode = studentSection.SectionCode;
                studentSection.SectionCode = section.SectionCode;
                studentSection.GroupCategory = section.GroupCategory;
                studentSection.JobID = job.Id;
                _calculator.UpdateSectionStatusToSuccessAndAddSectionToFinalModel(calculatedModel, studentSection);
            }
            tempNonGroupStudents.ForEach(r => distinctNoGroupSectionStudentList.Remove(r));
        }

        public CalcModel CreateNewSections(CalcModel calculatedModel, PreLoadStudentSection firtRecord,
            List<PreLoadStudentSection> noGroupSectionStudentList, Job job)
        {
            string groupNameSuffix = GeneralStatus.NOCOHORT_NOLOCATION_NAME_PREFIX;
            firtRecord.GroupCategory = ARBGroupCategory.GENERAL;
            int counter = GeneralStatus.BASE_COUNTER;
            if (calculatedModel.TotalSectionsNeeded > 1)
                _calculator.BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(calculatedModel, firtRecord, noGroupSectionStudentList, groupNameSuffix, ref counter, job);
            else if (calculatedModel.TotalSectionsNeeded == 1)
            {
                groupNameSuffix += counter.ToString();
                _calculator.AddAllStudentsToModelInOneSection(calculatedModel, noGroupSectionStudentList, firtRecord, groupNameSuffix, job);
            }

            return calculatedModel;
        }

        public List<PreviewStudentSection> NonCohortPreviewStudentRecords(List<PreviewStudentSection> previewRecords)
        {
            return previewRecords
                .Where(r => r.GroupCategory == ARBGroupCategory.GENERAL)
                .ToList();
        }

        public List<PreLoadStudentSection> NonCohortPreLoadStudentRecords(List<PreLoadStudentSection> preLoadRecords)
        {
            return preLoadRecords.Where(r => !r.GroupTypeKey.HasValue
                                             || r.GroupTypeKey == 0
                                             || r.GroupTypeKey == Group.LAST_CLASS_TOGETHER_NO_COHORT_NO_EMPTY_SEAT_FOUND
                                             || r.GroupNumber == Guid.Parse(LastClassGroupConstants.ONE_STUDENT_ONLY_COHORT_GROUP_NO_EMPTY_SEAT_FOUND_NUMBER)
                                             || r.GroupCategory == ARBGroupCategory.GENERAL)
                .ToList();
        }
    }
}
