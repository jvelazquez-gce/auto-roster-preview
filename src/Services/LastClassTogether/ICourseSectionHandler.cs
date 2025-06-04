using Domain.Models.Helper;

namespace Services.LastClassTogether
{
    public interface ICourseSectionHandler
    {
        void CreateCourseSectionsAndSaveThemAndLastClassGroupStudentSectionsToDb(CalcModel calcModel, Parameters p);
    }
}
