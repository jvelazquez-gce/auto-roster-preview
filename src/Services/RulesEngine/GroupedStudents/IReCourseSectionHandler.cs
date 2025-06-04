using Domain.Models.Helper;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IReCourseSectionHandler
    {
        void CreateCourseSectionsAndSaveThemAndLastClassGroupStudentSectionsToDb(CalcModel calcModel, Parameters p);
    }
}
