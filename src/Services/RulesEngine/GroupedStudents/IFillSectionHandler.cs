using System.Collections.Generic;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IFillSectionHandler
    {
        void FitNonCohortsLeftIntoEmptySeatsInDb();
        void FillInMemoryEmptySeats(List<PreLoadStudentSection> tempGroupList, CalcModel calculatedModel);
    }
}
