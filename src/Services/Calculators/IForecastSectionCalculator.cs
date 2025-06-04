using Domain.Entities;
using Domain.Models.Helper;

namespace Services.Calculators
{
    public interface IForecastSectionCalculator
    {
        int GetForeCastSectionsNeeded(PreviewStudentSection firstRecord, CalcModel calculatedModel, Job job);

        int GetTotalGeneralOrFriendStudentsRegistered(CalcModel calculatedModel);

        int GetNextSectionNumberToCreate(CalcModel calculatedModel);

        int GetNumberOfEmptySeatsInExistingSections(CalcModel calculatedModel, int numberOfGeneralAndFriendStudentsRegistered,
                                            int numberOfForecastedStudents, Job job);

        int GetNumberOfEmptySitsInGeneralSections(CalcModel calculatedModel);

        int GetNumberOfEmptySeatsInARBInclusiveSections(CalcModel calculatedModel);

        PreviewStudentSection GetForecastRecord(CalcModel calculatedModel);
    }
}
