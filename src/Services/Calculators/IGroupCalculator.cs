using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface IGroupCalculator
    {
        CalcModel GetCalculatedSectionsModel(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            int groupType,
            int groupNameCounterSuffix,
            Job job,
            IConfigurationRepository configurationRepository);

        void CombineSmallestToBiggestGroupSectionsIfPossibleToCreateTheLeastAmountOfSections(CalcModel calculatedModel, List<PreviewStudentSection> groupListToCombine, Job job);

        void TransferStudentSectionsByAmount(string sectionCodeToTakeStudentsFrom, string sectionCodeToAddStudentsInto, int amount, CalcModel calculatedModel, int newGroupCategory, Job job);

        List<PreviewStudentSection> NonExclusiveCohortPreviewStudentRecords(CalcModel calculatedModel);
    }
}
