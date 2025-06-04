using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;
using Infrastructure.Utilities;
using Domain.Interfaces.Infrastructure.Database.Commands;

namespace Services.OneStudentPerSection
{
    public class OneStudentPerSectionErrorChecking : IOneStudentPerSectionErrorChecking
    {
        private List<StudentSectionError> _studentSectionErrorList = new List<StudentSectionError>();
        private readonly IConfigInstance _configInstance;
        private readonly IMapper _mapper;

        public OneStudentPerSectionErrorChecking(IMapper mapper, IConfigInstance configInstance)
        {
            _mapper = mapper;
            _configInstance = configInstance;
        }

        public Error RunErrorChecks(PreLoadStudentSection record, IConfigurationRepository configurationRepository, Job job)
        {
            Error error = AreSectionValuesValidCheck(record, configurationRepository, job);
            if (!error.ErrorFound)
                error = IsRecordStudentValuesValidCheck(record, job);

            return error;
        }

        public Error AreSectionValuesValidCheck(PreLoadStudentSection record, IConfigurationRepository configurationRepository, Job job)
        {
            if (record == null) return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.NULL_VALUE };

            if (record.TargetStudentCount != 1)
            {
                SaveErrorRecordToMemory(record, StudentSectionStatus.TARGET_STUDENT_COUNT_IS_NOT_ONE, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.TARGET_STUDENT_COUNT_IS_NOT_ONE };
            }
            if ( !record.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER, StringComparison.OrdinalIgnoreCase) )
            {
                SaveErrorRecordToMemory(record, StudentSectionStatus.NON_SUPER_SECTION_NOT_ALLOWED_IN_ONE_STUDENT_PER_SECTION, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.NON_SUPER_SECTION_NOT_ALLOWED_IN_ONE_STUDENT_PER_SECTION };
            }
            if (record.AdCourseID < 1 || string.IsNullOrEmpty(record.CourseCode) || string.IsNullOrEmpty(record.SectionCode)
                || record.AdClassSchedID < 1 || record.StartDate == DateTime.MinValue
                || string.IsNullOrEmpty(record.Term))
            {
                SaveErrorRecordToMemory(record, StudentSectionStatus.MISSING_REQUIRED_DATAFIELD_VALUE, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_REQUIRED_DATAFIELD_VALUE };
            }
            if (record.StartDate.Date < _configInstance.GetCutOffStartDateTime())
            {
                SaveErrorRecordToMemory(record, StudentSectionStatus.START_DATE_IS_LESS_THAN_ALLOWED, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.START_DATE_IS_LESS_THAN_ALLOWED };
            }
            //if (record.GroupTypeKey == Group.EXCLUSIVE_COHORT || record.GroupTypeKey == Group.INCLUSIVE_COHORT)
            //{
            //    SaveErrorRecordToMemory(record, StudentSectionStatus.INVALID_COHORT_GROUP_TYPE_KEY, job);
            //    return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.INVALID_COHORT_GROUP_TYPE_KEY };
            //}

            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public Error IsRecordStudentValuesValidCheck(PreLoadStudentSection record, Job job)
        {
            if (record == null) return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.NULL_VALUE };

            if (!record.SyStudentID.HasValue || record.SyStudentID < 1)
            {
                SaveErrorRecordToMemory(record, StudentSectionStatus.MISSING_STUDENT_ID, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_STUDENT_ID };
            }

            if (!record.Si_PseudoRegistratonTrackingID.HasValue)
            {
                if (!record.AdEnrollID.HasValue || record.AdEnrollID < 1)
                {
                    SaveErrorRecordToMemory(record, StudentSectionStatus.MISSING_ENROLL_ID, job);
                    return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_ENROLL_ID };
                }

                if (!record.AdEnrollSchedID.HasValue || record.AdEnrollSchedID < 1)
                {
                    SaveErrorRecordToMemory(record, StudentSectionStatus.MISSING_AD_ENROLL_SCHED_ID, job);
                    return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_AD_ENROLL_SCHED_ID };
                }
            }

            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public List<StudentSectionError> GetStudentSectionErrorList() { return _studentSectionErrorList; }
        public void SaveErrorRecordToMemory(PreLoadStudentSection section, int errorNumber, Job job)
        {
            StudentSectionError sectionError = new StudentSectionError();
            _mapper.Map<PreLoadStudentSection, StudentSectionError>(section, sectionError);
            sectionError.StatusID = errorNumber;
            sectionError.JobID = job.Id;
            _studentSectionErrorList.Add(sectionError);
        }

        public void SaveErrorRecordToMemory(OneStudentPerSectionRecord section, int errorNumber, Job job)
        {
            var sectionError = new StudentSectionError();
            _mapper.Map<OneStudentPerSectionRecord, StudentSectionError>(section, sectionError);
            sectionError.StatusID = errorNumber;
            sectionError.JobID = job.Id;
            _studentSectionErrorList.Add(sectionError);
        }

        public List<StudentSectionError> SaveErrorRecordsToDbAndRemoveThemFromMemory(IAddStudentSectionErrorCommand AddStudentSectionErrorCommand, Job job)
        {
            var updatedRecords = new List<StudentSectionError>();

            if (_studentSectionErrorList.Any())
            {
                // this check was put in place to prevent duplicate error records from being added. This is just a quick work around
                // but the source of the duplication has not been fixed yet. But at least this code prevents duplicate record errors 
                // from being added to the database. The possible source may be in the HandleErrorInactivationRecords method.
                //_studentSectionErrorList = GetErrorRecordsInMemoryWithoutDuplicates();

                updatedRecords = AddStudentSectionErrorCommand.ExecuteCommand(_studentSectionErrorList);
            }

            _studentSectionErrorList = new List<StudentSectionError>();

            return updatedRecords;
        }

        public List<StudentSectionError> GetErrorRecordsInMemory()
        {
            return _studentSectionErrorList;
        }
    }
}
