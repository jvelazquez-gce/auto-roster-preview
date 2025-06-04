using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;

namespace Services.Calculators
{
    public interface IErrorChecking
    {
        Error IsRecordCleanFirstCheck(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, IConfigurationRepository configurationRepository, Job job);

        Error IsRecordCleanSecondCheck(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job);

        Error IsExclusiveRecordClean(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job);

        Error IsInclusiveRecordClean(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job);

        void SaveErrorRecord(PreLoadStudentSection section, int errorNumber, ISectionErrorRepository sectionErrorRepository, Job job);

        void SaveErrorRecord(PreviewStudentSection section, int errorNumber, ISectionErrorRepository sectionErrorRepository);
    }
}
