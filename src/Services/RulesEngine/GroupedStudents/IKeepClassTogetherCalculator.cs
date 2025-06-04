using Domain.Models.Helper;

namespace Services.RulesEngine.GroupedStudents
{
    public interface  IKeepClassTogetherCalculator
    {
        void ProcessRecords(CalcModel calculatedModel);
    }
}
