using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.LastClassTogether
{
    public interface ICohortOrganizer
    {
        List<PreLoadStudentSection> GetPreLoadStudentSectionsAfterMainFilter(List<PreLoadStudentSection> preLoadStudentSections);

        void CheckForErrorsRemoveAndSaveToDbIfFound(List<PreLoadStudentSection> preLoadStudentSections, Parameters p);

        List<PreLoadStudentSection> GetListWithUpdateGroupDetails(List<PreLoadStudentSection> preLoadStudentSections);
    }
}
