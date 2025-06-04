using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.OneStudentPerSection
{
    public interface IOneStudentPerSectionErrorChecking
    {
        public Error RunErrorChecks(PreLoadStudentSection record, IConfigurationRepository configurationRepository, Job job);

        public Error AreSectionValuesValidCheck(PreLoadStudentSection record, IConfigurationRepository configurationRepository, Job job);

        public Error IsRecordStudentValuesValidCheck(PreLoadStudentSection record, Job job);

        public List<StudentSectionError> GetStudentSectionErrorList();
        public void SaveErrorRecordToMemory(PreLoadStudentSection section, int errorNumber, Job job);

        public void SaveErrorRecordToMemory(OneStudentPerSectionRecord section, int errorNumber, Job job);

        public List<StudentSectionError> SaveErrorRecordsToDbAndRemoveThemFromMemory(IAddStudentSectionErrorCommand AddStudentSectionErrorCommand, Job job);
        public List<StudentSectionError> GetErrorRecordsInMemory();
    }
}
