using System.Collections.Generic;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Entities;
using Domain.Models.Helper;

namespace Services.Calculators
{
    public class DataMapper
    {
        public List<PreLoadStudentSection> GetStudentDataWithGroupTypes(Parameters p, int jobMode)
        {
            List<PreLoadStudentSection> listOfRecords = new List<PreLoadStudentSection>();
            if (jobMode == JobStatus.PREVIEW_JOB_RUNNING)
            {
                listOfRecords = p.GetPreLoadDataForPreviewQuery.ExecuteQuery(p.QueryInputParams);
            }
            else if (jobMode == JobStatus.RUNNING_LIVE_JOB)
            {
                listOfRecords = p.GetPreLoadDataForUpdateQuery.ExecuteQuery(p.QueryInputParams);
            }
            else
            {
                throw new InvalidJobModeException();
            }

            return listOfRecords;
        }
    }
}
