using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;

namespace Services.LastClassTogether
{
    public class CohortOrganizer : ICohortOrganizer
    {
        private List<StudentSectionError> _errorPreLoadList = new List<StudentSectionError>();
        private readonly IMapper _mapper;
        private Parameters _p;
        private List<int> _courseIdsToGroupBy;
        private List<int> _validLastCourseIdsTaken;

        public CohortOrganizer(IMapper mapper, Parameters p)
        {
            _mapper = mapper;
            _p = p;
            _courseIdsToGroupBy = _p.ClassesTogetherToGroupRuleList
                .Select(s => s.AdCourseID)
                .Distinct()
                .ToList();

            _validLastCourseIdsTaken = _p.ClassesTogetherToGroupRuleList
                .Select(s => s.AdCourseIdToGroupBy)
                .Distinct()
                .ToList();
        }

        public List<PreLoadStudentSection> GetPreLoadStudentSectionsAfterMainFilter(List<PreLoadStudentSection> preLoadStudentSections)
        {

            var recordsToUpdate = preLoadStudentSections
                .Where(l => _courseIdsToGroupBy.Contains(l.AdCourseID))
                .Where(l => l.LastAdClassSchedIDTaken != null)
                .Where(l => l.LastAdCourseIDTaken != null)
                .Where(l => l.GroupTypeKey == null || l.GroupTypeKey == 0)
                .Where(l => l.GroupNumber == null || l.GroupNumber == Guid.Empty)
                .ToList();

            return recordsToUpdate;
        }

        public void CheckForErrorsRemoveAndSaveToDbIfFound(List<PreLoadStudentSection> preLoadStudentSections, Parameters p)
        {
            var records = preLoadStudentSections
                .Where(r => _courseIdsToGroupBy.Contains(r.AdCourseID))
                .ToList();

            var errors = records
                .Where(r => r.LastAdClassSchedIDTaken == null)
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_LastAdClassSchedIDTaken, p.Job.Id);

            errors = records
                .Where(r => r.LastAdCourseIDTaken == null)
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_Null_LastAdCourseIDTaken, p.Job.Id);

            errors = records
                .Where(r => r.GroupTypeKey != null && r.GroupTypeKey != 0)
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_GroupTypeKey, p.Job.Id);

            errors = records
                .Where(r => r.GroupNumber != null && r.GroupNumber != Guid.Empty)
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_GroupNumber, p.Job.Id);

            errors = records
                .Where(r => !r.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_SuperSection, p.Job.Id);

            errors = records
                .Where(r => !r.LastAdCourseIDTaken.HasValue || !_validLastCourseIdsTaken.Contains((int)r.LastAdCourseIDTaken))
                .ToList();

            if (errors.Any()) AddErrorRecord(errors, preLoadStudentSections, LastClassGroupConstants.Error_Invalid_LastAdCourseIDTaken, p.Job.Id);

            if (_errorPreLoadList.Any()) p.AddStudentSectionErrorCommand.ExecuteCommand(_errorPreLoadList);
        }

        private void AddErrorRecord(List<PreLoadStudentSection> preloadErrorList, List<PreLoadStudentSection> preLoadStudentSections, int statusId, int jobId)
        {
            preloadErrorList.ForEach(r => preLoadStudentSections.Remove(r));

            var sectionErrors = new List<StudentSectionError>();
            _mapper.Map<List<PreLoadStudentSection>, List<StudentSectionError>>(preloadErrorList, sectionErrors);
            foreach (var error in sectionErrors)
            {
                error.StatusID = statusId;
                error.JobID = jobId;
                _errorPreLoadList.Add(error);
            }
        }

        public List<PreLoadStudentSection> GetListWithUpdateGroupDetails(List<PreLoadStudentSection> preLoadStudentSections)
        {
            var recordsToUpdate = GetPreLoadStudentSectionsAfterMainFilter(preLoadStudentSections);

            // if last course id is not one of the following they will not be process by the new logic: DNP_955_COURSE_ID, DNP_960_COURSE_ID, DNP_965_COURSE_ID
            recordsToUpdate = recordsToUpdate
                .Where(p => p.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                .Where(p => _validLastCourseIdsTaken.Contains((int)p.LastAdCourseIDTaken))
                .ToList();

            var lastAdClassSchedIdList = recordsToUpdate
                .Select(p => p.LastAdClassSchedIDTaken)
                .Distinct()
                .ToList();

            foreach (var lastAdClassSchedId in lastAdClassSchedIdList)
            {
                var groupNumber = Guid.NewGuid();
                var groupWithSameAdClassSchedId = recordsToUpdate
                    .Where(p => p.LastAdClassSchedIDTaken == lastAdClassSchedId);

                var groupSize = groupWithSameAdClassSchedId.Count();
                foreach (var student in groupWithSameAdClassSchedId)
                {
                    if (groupSize > 1)
                    {
                        student.GroupTypeKey = Group.LAST_CLASS_TOGETHER_COHORT;
                        student.GroupNumber = student.GroupNumber = groupNumber;
                    }
                    else
                    {
                        student.GroupTypeKey = Group.LAST_CLASS_TOGETHER_NO_COHORT;
                        student.GroupNumber = student.GroupNumber = Guid.Parse(LastClassGroupConstants.ONE_STUDENT_ONLY_COHORT_GROUP_NUMBER);
                    }
                }
            }

            return recordsToUpdate;
        }
    }
}
