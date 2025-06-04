using System;
using System.Linq;
using Domain.Entities;

namespace Domain.Interfaces.Infrastructure.Database.Repositories
{
    public interface ISectionErrorRepository
    {
        void Add(StudentSectionError section);
        int NumberOfErrorsThatNeedToBeSaved();
        IQueryable<StudentSectionError> GetErrors(int jobID, int? courseID, DateTime? startDate);
        void Save();
        void DeleteStudentSection(StudentSectionError studentSection);
    }
}
