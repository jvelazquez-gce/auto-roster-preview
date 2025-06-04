using Domain.Entities;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface IErrorMitigation
    {
        void LessThanMinAllowedCohortRecords(
            List<PreLoadStudentSection> cohortRecords, 
            Job job, List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate);
    }
}
