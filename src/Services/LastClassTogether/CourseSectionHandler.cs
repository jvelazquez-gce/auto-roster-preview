using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;
using Infrastructure.Utilities;

namespace Services.LastClassTogether
{
    public class CourseSectionHandler : ICourseSectionHandler
    {
        private readonly IConfigInstance _configInstance;
        private readonly ICalculator _calculator;

        public CourseSectionHandler(IConfigInstance configInstance, ICalculator calculator)
        {
            _configInstance = configInstance;
            _calculator = calculator;
        }

        public void CreateCourseSectionsAndSaveThemAndLastClassGroupStudentSectionsToDb(CalcModel calcModel, Parameters p)
        {
            if (!calcModel.LastClassGroupStudentSections.Any()) return;

            var groupNumberList = calcModel.LastClassGroupStudentSections
                .Select(s => s.GroupNumber)
                .Distinct()
                .ToList();

            if (!groupNumberList.Any()) return;

            var courseSections = new List<CourseSection>();
            foreach (var groupNumber in groupNumberList)
            {
                var lastClassGroupStudentSections = calcModel.LastClassGroupStudentSections
                    .Where(w => w.GroupNumber == groupNumber)
                    .ToList();

                var firstLastClassGroupStudentSections = lastClassGroupStudentSections.First();

                var sectionGuid = Guid.NewGuid();
                var totalStudents = lastClassGroupStudentSections.Count;
                lastClassGroupStudentSections.ForEach(r => r.SectionGuid = sectionGuid);
                var courseSection = new CourseSection()
                {
                    Id = sectionGuid,
                    JobID = p.Job.Id,
                    CampusCode = GeneralStatus.CAMPUS_ONLINE,
                    CourseCode = firstLastClassGroupStudentSections.CourseCode,
                    AdCourseID = firstLastClassGroupStudentSections.AdCourseID,
                    StartDate = firstLastClassGroupStudentSections.StartDate,
                    ExternalSystem = GeneralStatus.ARB_JOB,
                    ExternalSystemId = sectionGuid.ToString(),
                    TermCode = firstLastClassGroupStudentSections.Term,
                    AdTermID = firstLastClassGroupStudentSections.AdTermID,
                    // This value should always be one
                    MaximumNumberOfStudents = _calculator.GetMaxNumberOfStudentsPerSection(firstLastClassGroupStudentSections.TargetStudentCount, firstLastClassGroupStudentSections.GroupNumber, firstLastClassGroupStudentSections.GroupTargetStudentCount),
                    PassFailSetting = GeneralStatus.PassFailType.NoOptionForPassFail,
                    PrimaryInstructor = _configInstance.GetDefaultInstructorEmail(),
                    SectionCode = firstLastClassGroupStudentSections.SectionCode,
                    SectionDeliveryMethod = GeneralStatus.ONLINE_DELIVERY_METHOD,
                    HasSectionBeenCreatedOrUpdatedInCampusVue = false,
                    GroupCategory = firstLastClassGroupStudentSections.GroupCategory,
                    StatusID = SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE,
                    Active = true,
                    IsManagedByARB = true,
                    ForecastSection = false,
                    LmsExtractStatus = firstLastClassGroupStudentSections.LmsExtractStatus,
                    SyLmsVendorID = firstLastClassGroupStudentSections.SysLmsVendorID,
                    EndDate = firstLastClassGroupStudentSections.EndDate,
                    FinalRegStudents = totalStudents,
                    CreatedBy = GeneralStatus.ARB_JOB,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = GeneralStatus.ARB_JOB,
                    UpdatedOn = DateTime.Now
                };
                courseSections.Add(courseSection);
            }
            p.AddCourseSectionsCommand.ExecuteCommand(courseSections);

            // TODO: clean up to maybe don't have to do this extra looping
            foreach (var rec in calcModel.LastClassGroupStudentSections)
                if(rec.GroupTypeKey == Group.LAST_CLASS_TOGETHER_NO_COHORT) 
                    rec.GroupNumber = Guid.Parse(LastClassGroupConstants.ONE_STUDENT_ONLY_COHORT_GROUP_NUMBER);

            p.AddLastClassGroupStudentSectionsCommand.ExecuteCommand(calcModel.LastClassGroupStudentSections);
        }
    }
}
