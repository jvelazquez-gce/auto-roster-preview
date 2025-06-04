using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using System;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface ICourseSectionCalculator
    {
        void CreateCourseSectionsInPrepForCVueCreation(CalcModel calculatedModel, int courseId, DateTime startDate,
            Job job, ISectionErrorRepository sectionErrorRepository, IConfigurationRepository configurationRepository);

        void CreateCourseSections(CalcModel calculatedModel, Job job, IConfigurationRepository configurationRepository);

        void CreateForecastedSections(CalcModel calculatedModel, Job job, IConfigurationRepository configurationRepository);


        void ReUsePreviouslyCreatedSections(CalcModel calcModel, int courseId, DateTime startDate, Job job, IConfigurationRepository configurationRepository);

        List<CourseSection> ReSortCoursesToBeReUsed(List<CourseSection> courseSectionList, int defaultTecherID);

        void ReUseSectionsAndIdentifyNotReUsedSectionsList(CalcModel calcModel, CourseSection sectionToBeReUsed, Job job, List<CourseSection> updatedCourseSectionLits);

        void UpdateCourseSection(CalcModel calcModel, CourseSection firstCourseSection, CourseSection sectionToBeReUsed, Job job);

        void UpdateStudentSectionsWithCourseSectionGuid(CalcModel calculatedModel, Job job, ISectionErrorRepository sectionErrorRepository);
    }
}
