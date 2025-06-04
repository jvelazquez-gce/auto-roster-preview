using Domain.Entities;
using Domain.Models.Helper;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IAddPreviewStudentAndUpdateCourseSectionsCommand
    {
        void Execute(CalcModel calculatedModel, Job job);
    }
}
