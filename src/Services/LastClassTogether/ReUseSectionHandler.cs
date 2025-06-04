using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;

namespace Services.LastClassTogether
{
    public class ReUseSectionHandler : IReUseSectionHandler
    {
        private readonly List<PreLoadStudentSection> _groupList;
        private readonly Job _job;
        private readonly Parameters _p;
        private readonly CalcModel _calcModel;
        private readonly IMapper _mapper;

        public ReUseSectionHandler(
            IMapper mapper, 
            List<PreLoadStudentSection> groupList,
            Job job,
            Parameters p,
            CalcModel calcModel) 
        {
            _groupList = groupList;
            _job = job;
            _p = p;
            _calcModel = calcModel;
            _mapper = mapper;
        }

        public void FindSectionsToReUseIfPossibleAndAssignStudents()
        {
            if (!_groupList.Any()) return;

            var distinctGroupIds = _groupList.Select(r => r.GroupNumber)
                .Distinct()
                .ToList();

            foreach (var groupNumber in distinctGroupIds)
            {
                var tempGroupList = _groupList.Where(l => l.GroupNumber == groupNumber)
                    .ToList();

                var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };
                var lastClassGroupStudentSections = _p.GetLastClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);

                if (!lastClassGroupStudentSections.Any()) continue;

                var firstPreLoadRecord = tempGroupList.First();

                var studentIdGroupList = tempGroupList.Where(w => w.SyStudentID != null)
                    .Select(g => (int)g.SyStudentID)
                    .ToList();

                RemoveFromMemoryStudentsAlreadyInDb(tempGroupList, lastClassGroupStudentSections, studentIdGroupList);

                var firstCohortMatch = lastClassGroupStudentSections
                    .Where(w => w.AdCourseID == firstPreLoadRecord.AdCourseID)
                    .Where(w => w.StartDate == firstPreLoadRecord.StartDate)
                    .Where(w => w.LastAdClassSchedIDTaken == firstPreLoadRecord.LastAdClassSchedIDTaken)
                    .FirstOrDefault(w => !studentIdGroupList.Contains(w.SyStudentID));

                if (firstCohortMatch == null) return;

                var cohortMatchList = lastClassGroupStudentSections
                    .Where(w => w.SectionGuid == firstCohortMatch.SectionGuid)
                    .ToList();

                if (!cohortMatchList.Any()) return;

                if (firstCohortMatch.TargetStudentCount == cohortMatchList.Count) return;

                if (firstCohortMatch.TargetStudentCount > cohortMatchList.Count) MakeChanges(firstCohortMatch, cohortMatchList, tempGroupList);
            }
        }

        private void RemoveFromMemoryStudentsAlreadyInDb(List<PreLoadStudentSection> tempGroupList, 
            List<LastClassGroupStudentSection> lastClassGroupStudentSections,
            List<int> studentIdGroupList)
        {
            var firstPreLoadRecord = tempGroupList.First();

            var existingList = lastClassGroupStudentSections
                .Where(w => w.AdCourseID == firstPreLoadRecord.AdCourseID)
                .Where(w => w.StartDate == firstPreLoadRecord.StartDate)
                .Where(w => w.LastAdClassSchedIDTaken == firstPreLoadRecord.LastAdClassSchedIDTaken)
                .Where(w => studentIdGroupList.Contains(w.SyStudentID))
                .ToList();

            if (!existingList.Any()) return;

            foreach (var rec in existingList)
            {
                tempGroupList.RemoveAll(w => w.SyStudentID == rec.SyStudentID);
                _groupList.RemoveAll(w => w.SyStudentID == rec.SyStudentID);
            }
        }

        public void MakeChanges(LastClassGroupStudentSection firstCohortMatch, List<LastClassGroupStudentSection> cohortMatchList, List<PreLoadStudentSection> tempGroupList)
        {
            if (firstCohortMatch == null || !cohortMatchList.Any() || !tempGroupList.Any()) return;

            if (cohortMatchList.Count > firstCohortMatch.TargetStudentCount)
                throw new Exception($"cohortMatchList.Count > firstCohortMatch.TargetStudentCount - {cohortMatchList.Count} > {firstCohortMatch.TargetStudentCount}");

            var recordsToAdd = new List<PreLoadStudentSection>();
            var emptySeats = firstCohortMatch.TargetStudentCount - cohortMatchList.Count;

            if (emptySeats == 0) return;

            if (tempGroupList.Count > emptySeats)
            {
                recordsToAdd = tempGroupList.Take(emptySeats).ToList();
                foreach (var rec in recordsToAdd)
                {
                    tempGroupList.Remove(rec);
                    _groupList.Remove(rec);
                }
            }
            else
            {
                recordsToAdd = tempGroupList.ToList();
                recordsToAdd.ForEach(r => _groupList.Remove(r));
            }

            var tempLastClassGroupStudentSection = new List<LastClassGroupStudentSection>();
            _mapper.Map<List<PreLoadStudentSection>, List<LastClassGroupStudentSection>>(recordsToAdd, tempLastClassGroupStudentSection);
            foreach (var record in tempLastClassGroupStudentSection)
            {
                record.JobID = _job.Id;
                record.SectionCode = firstCohortMatch.SectionCode;
                record.SectionGuid = firstCohortMatch.SectionGuid;
                record.GroupCategory = firstCohortMatch.GroupCategory;
                record.GroupNumber = firstCohortMatch.GroupNumber;
                //if (firstCohortMatch.LastAdClassSchedIDTaken == record.LastAdClassSchedIDTaken)
                //{
                //    record.GroupNumber = firstCohortMatch.GroupNumber;
                //}
                record.StatusID = firstCohortMatch.StatusID;
            }

            _p.AddLastClassGroupStudentSectionsCommand.ExecuteCommand(tempLastClassGroupStudentSection);
        }
    }
}
