using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.OneStudentPerSection
{
    public interface IOneStudentPerSectionCourseSectionHandler
    {
        OneStudentPerSectionResults ExecuteCourseSectionLogic(Parameters p, IConfigurationRepository configurationRepository, OneStudentPerSectionResults results);

        void InactivateSectionsWhereStudentsDropdOffAndGetRecordsWithErrors(Parameters p, List<OneStudentPerSectionPair> OneStudentPerSectionPairListToUpdate,
            List<OneStudentPerSectionPair> OneStudentPerSectionPairListWithErrors);

        CourseSection SetSectionToInactive(CourseSection courseSection);

        List<StudentSectionError> HandleErrorInactivationRecords(List<OneStudentPerSectionPair> OneStudentPerSectionPairListWithErrors, Parameters p);

        OneStudentPerSectionPair CreateCourseSectionsToAddToCampusVueAndSaveSectionsAndStudentRecordsToDb(Parameters p, IConfigurationRepository configurationRepository);
    }
}
