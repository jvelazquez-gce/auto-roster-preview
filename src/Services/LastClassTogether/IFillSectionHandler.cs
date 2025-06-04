using System.Collections.Generic;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public interface IFillSectionHandler
    {
        void FitNonCohortsLeftIntoEmptySeatsInDb();
        void FillInMemoryEmptySeats(List<PreLoadStudentSection> tempGroupList, CalcModel calculatedModel);
    }
}
