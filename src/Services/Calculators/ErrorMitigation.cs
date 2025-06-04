using System.Collections.Generic;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Utilities;

namespace Services.Calculators
{
    public class ErrorMitigation : IErrorMitigation
    {
        private readonly IConfigInstance _configInstance;

        public ErrorMitigation(IConfigInstance configInstance) 
        {
            _configInstance = configInstance;
        }

        public void LessThanMinAllowedCohortRecords(List<PreLoadStudentSection> cohortRecords, Job job, List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate)
        {
            if (cohortRecords.Count < _configInstance.GetCohortMinSize() && cohortRecords.Count > 0)
            {
                foreach(var record in cohortRecords)
                {
                    sectionsWithSameCourseAndStartDate.Remove(record);
                    record.GroupCategory = ARBGroupCategory.GENERAL;

                    sectionsWithSameCourseAndStartDate.Add(record);
                }
            }
        }
    }
}
