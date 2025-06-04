using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public class FillSectionHandler : IFillSectionHandler
    {
        private List<PreLoadStudentSection> _noGroupList;
        private Parameters _p;
        private CalcModel _calcModel;
        private Job _job;
        private IReUseSectionHandler _reUseSectionHandler;

        public FillSectionHandler(List<PreLoadStudentSection> noGroupList, Parameters p, CalcModel calcModel, Job job, IReUseSectionHandler reUseSectionHandler)
        {
            _noGroupList = noGroupList;
            _p = p;
            _calcModel = calcModel;
            _job = job;
            _reUseSectionHandler = reUseSectionHandler;
        }

        public void FitNonCohortsLeftIntoEmptySeatsInDb()
        {
            if (!_noGroupList.Any()) return;

            var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };
            var lastClassGroupStudentSection = _p.GetLastClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);

            if (!lastClassGroupStudentSection.Any()) return;

            var sectionGuidList = lastClassGroupStudentSection
                .Select(s => s.SectionGuid)
                .Distinct()
                .ToList();

            foreach (var sectionGuid in sectionGuidList)
            {
                var cohortMatchList = lastClassGroupStudentSection
                    .Where(w => w.SectionGuid == sectionGuid)
                    .Where(w => w.SectionGuid != Guid.Empty)
                    .ToList();

                var firstCohortMatch = cohortMatchList
                    .First();

                if (!cohortMatchList.Any()) return;

                if (firstCohortMatch.TargetStudentCount == cohortMatchList.Count) return;

                if (firstCohortMatch.TargetStudentCount > cohortMatchList.Count) _reUseSectionHandler.MakeChanges(firstCohortMatch, cohortMatchList, _noGroupList);
            }

        }

        public void FillInMemoryEmptySeats(List<PreLoadStudentSection> tempGroupList, CalcModel calculatedModel)
        {
            if (!_noGroupList.Any()) return;

            if (calculatedModel.TotalStudentsRegistered >= calculatedModel.MaxStudentsPerSection) return;

            var emptySeats = calculatedModel.MaxStudentsPerSection - calculatedModel.TotalStudentsRegistered;
            var recordsToAdd = _noGroupList.Count > emptySeats
                ? _noGroupList.Take(emptySeats).ToList()
                : _noGroupList.ToList();

            foreach (var rec in recordsToAdd)
            {
                _noGroupList.Remove(rec);
                tempGroupList.Add(rec);
            }
        }
    }
}
