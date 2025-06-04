using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public class InCohortPreloadSectionHandler : IInCohortPreloadSectionHandler
    {
        private readonly Parameters _p;
        private CalcModel _calcModel;
        private List<PreLoadStudentSection> _sectionsWithSameCourseAndStartDate;
        private readonly ICohortOrganizer _cohortOrganizer;

        public InCohortPreloadSectionHandler(
            Parameters p, 
            CalcModel calcModel, 
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            ICohortOrganizer cohortOrganizer)
        {
            _p = p;
            _calcModel = calcModel;
            _sectionsWithSameCourseAndStartDate = sectionsWithSameCourseAndStartDate;
            _cohortOrganizer = cohortOrganizer;
        }

        public List<PreLoadStudentSection> FindAndMarkRecordsAsLastClassTogetherCohortRemoveNonCohortsFromDbAndStudentCleanUp()
        {
            var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };
            _cohortOrganizer.CheckForErrorsRemoveAndSaveToDbIfFound(_sectionsWithSameCourseAndStartDate, _p);
            var recordsToUpdate = _cohortOrganizer.GetListWithUpdateGroupDetails(_sectionsWithSameCourseAndStartDate);

            var lastClassGroupStudentSection = _p.GetLastClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);
            if (!recordsToUpdate.Any())
            {
                if (lastClassGroupStudentSection.Any())
                {
                    _p.RemoveLastClassGroupStudentSectionCommand.ExecuteCommand(lastClassGroupStudentSection);
                }

                return recordsToUpdate;
            }

            RemoveNonCohortsFromDb(lastClassGroupStudentSection);
            RemoveInMemoryPreloadsAlreadyInTheSystem(lastClassGroupStudentSection, recordsToUpdate);

            return recordsToUpdate;
        }

        private void RemoveNonCohortsFromDb(List<LastClassGroupStudentSection> lastClassGroupStudentSectionFromDb)
        {
            var recordsToRemove = lastClassGroupStudentSectionFromDb
                .Where(w => w.GroupTypeKey == Group.LAST_CLASS_TOGETHER_NO_COHORT)
                .ToList();

            if (!recordsToRemove.Any()) return;

            _p.RemoveLastClassGroupStudentSectionCommand.ExecuteCommand(recordsToRemove);
            recordsToRemove.ForEach(r => lastClassGroupStudentSectionFromDb.Remove(r));
        }

        private void RemoveInMemoryPreloadsAlreadyInTheSystem(
            IEnumerable<LastClassGroupStudentSection> lastClassGroupStudentSection, 
            ICollection<PreLoadStudentSection> recordsToUpdate)
        {
            foreach (var recToRemove in lastClassGroupStudentSection
                .Select(dbRec => recordsToUpdate
                    .Where(p => p.AdCourseID == dbRec.AdCourseID)
                    .Where(p => p.StartDate == dbRec.StartDate)
                    .Where(p => p.LastAdClassSchedIDTaken == dbRec.LastAdClassSchedIDTaken)
                    .FirstOrDefault(p => p.SyStudentID == dbRec.SyStudentID))
                .Where(recToRemove => recToRemove != null))
            {
                recordsToUpdate.Remove(recToRemove);
                _sectionsWithSameCourseAndStartDate.Remove(recToRemove);
            }
        }
    }
}
