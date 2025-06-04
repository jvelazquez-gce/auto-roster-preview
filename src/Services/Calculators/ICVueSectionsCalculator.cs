using Domain.Entities;
using Domain.Models.Helper;
using Infrastructure.Database.Context;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface ICVueSectionsCalculator
    {
        void UpdateAndAddCampusVueSections(List<PreLoadStudentSection> initialStudentRawData,
            CalcModel calcModel,
            Job job)
        {
        }

        void RunUpdateAndAddCampusVueSections(ARBDb context, List<PreLoadStudentSection> initialStudentRawData,
            CalcModel calcModel, Job job)
        {
        }
    }
}
