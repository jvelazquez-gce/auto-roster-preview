using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.RulesEngine.GroupedStudents
{
    public class InGroupPreloadSectionHandler : IInGroupPreloadSectionHandler
    {
        private readonly Parameters _p;
        private CalcModel _calcModel;
        private List<PreLoadStudentSection> _sectionsWithSameCourseAndStartDate;
        private readonly IGroupOrganizer _groupOrganizer;

        public InGroupPreloadSectionHandler(
            Parameters p, 
            CalcModel calcModel, 
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            IGroupOrganizer groupOrganizer)
        {
            _p = p;
            _calcModel = calcModel;
            _sectionsWithSameCourseAndStartDate = sectionsWithSameCourseAndStartDate;
            _groupOrganizer = groupOrganizer;
        }

        public List<PreLoadStudentSection> FindAndMarkRecordsAsInRuleGroupRemoveNotInRuleGroupFromDbAndStudentCleanUp()
        {
            var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };

            var recordsToUpdate = _groupOrganizer.GetListWithUpdateGroupDetails(_sectionsWithSameCourseAndStartDate, _p, _calcModel.Rules);

            var ruleClassGroupStudentSection = _p.GetClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);
            if (!recordsToUpdate.Any())
            {
                if (ruleClassGroupStudentSection.Any())
                {
                    _p.RemoveClassGroupStudentSectionCommand.ExecuteCommand(ruleClassGroupStudentSection);
                }

                return recordsToUpdate;
            }

            RemoveNonCohortsFromDb(ruleClassGroupStudentSection);
            RemoveInMemoryPreloadsAlreadyInTheSystem(ruleClassGroupStudentSection, recordsToUpdate);

            return recordsToUpdate;
        }

        private void RemoveNonCohortsFromDb(List<ClassGroupStudentSection> classGroupStudentSectionFromDb)
        {
            var recordsToRemove = classGroupStudentSectionFromDb
                .Where(w => w.GroupTypeKey == Group.NOT_IN_RULE_GROUP)
                .ToList();

            if (!recordsToRemove.Any()) return;

            _p.RemoveClassGroupStudentSectionCommand.ExecuteCommand(recordsToRemove);
            recordsToRemove.ForEach(r => classGroupStudentSectionFromDb.Remove(r));
        }

        private void RemoveInMemoryPreloadsAlreadyInTheSystem(
            IEnumerable<ClassGroupStudentSection> classGroupStudentSection, 
            ICollection<PreLoadStudentSection> recordsToUpdate)
        {
            foreach (var recToRemove in classGroupStudentSection
                .Select(dbRec => recordsToUpdate
                    .Where(p => p.AdCourseID == dbRec.AdCourseID)
                    .Where(p => p.StartDate == dbRec.StartDate)
                    //.Where(p => p.LastAdClassSchedIDTaken == dbRec.LastAdClassSchedIDTaken)
                    .FirstOrDefault(p => p.SyStudentID == dbRec.SyStudentID))
                .Where(recToRemove => recToRemove != null))
            {
                recordsToUpdate.Remove(recToRemove);
                _sectionsWithSameCourseAndStartDate.Remove(recToRemove);
            }
        }
    }
}
