using Domain.Entities;
using System.Collections.Generic;

namespace Services.LastClassTogether
{
    public interface IInCohortPreloadSectionHandler
    {
        List<PreLoadStudentSection> FindAndMarkRecordsAsLastClassTogetherCohortRemoveNonCohortsFromDbAndStudentCleanUp();
    }
}
