using System;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;
using AutoMapper;

namespace Services.Calculators
{
    public class ErrorChecking : IErrorChecking
    {
        private readonly IMapper _mapper;

        private readonly IConfigInstance _configInstance;

        public ErrorChecking(IMapper mapper, IConfigInstance configInstance)
        {
            _mapper = mapper;
            _configInstance = configInstance;
        }

        public Error IsRecordCleanFirstCheck(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, IConfigurationRepository configurationRepository, Job job)
        {
            if (record == null) return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.NULL_VALUE };

            if (record.AdCourseID < 1 || string.IsNullOrEmpty(record.CourseCode) || string.IsNullOrEmpty(record.SectionCode) 
                || record.AdClassSchedID < 1 ||  record.StartDate == new DateTime(1,1,1) 
                || record.TargetStudentCount < 1 || string.IsNullOrEmpty(record.Term))
            {
                SaveErrorRecord(record, StudentSectionStatus.MISSING_REQUIRED_DATAFIELD_VALUE, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_REQUIRED_DATAFIELD_VALUE };
            }
            if (record.StartDate.Date < _configInstance.GetCutOffStartDateTime())
            {
                SaveErrorRecord(record, StudentSectionStatus.START_DATE_IS_LESS_THAN_ALLOWED, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.START_DATE_IS_LESS_THAN_ALLOWED };
            }
            if (record.GroupTypeKey == Group.EXCLUSIVE_COHORT)
                return IsExclusiveRecordClean(record, sectionErrorRepository, job);
            else if (record.GroupTypeKey == Group.INCLUSIVE_COHORT)
                return IsInclusiveRecordClean(record, sectionErrorRepository, job);
            else if (record.GroupTypeKey.HasValue && record.GroupTypeKey != 0 && !Group.COHORT_KEY_LIST.Contains((int)record.GroupTypeKey))
            {
                SaveErrorRecord(record, StudentSectionStatus.INVALID_COHORT_GROUP_TYPE_KEY, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.INVALID_COHORT_GROUP_TYPE_KEY };
            }

            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public Error IsRecordCleanSecondCheck(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job)
        {
            if (record == null) return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.NULL_VALUE };

            if (!record.SyStudentID.HasValue || record.SyStudentID < 1)
            {
                SaveErrorRecord(record, StudentSectionStatus.MISSING_STUDENT_ID, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_STUDENT_ID };
            }
            
            if( !record.Si_PseudoRegistratonTrackingID.HasValue )
            {
                if (!record.AdEnrollID.HasValue || record.AdEnrollID < 1)
                {
                    SaveErrorRecord(record, StudentSectionStatus.MISSING_ENROLL_ID, sectionErrorRepository, job);
                    return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_ENROLL_ID };
                }

                if (!record.AdEnrollSchedID.HasValue || record.AdEnrollSchedID < 1)
                {
                    SaveErrorRecord(record, StudentSectionStatus.MISSING_AD_ENROLL_SCHED_ID, sectionErrorRepository, job);
                    return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_AD_ENROLL_SCHED_ID };
                }
            }

            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public Error IsExclusiveRecordClean(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job)
        {
            if ( record.GroupNumber.HasValue && record.TargetStudentCount == 0 && !record.GroupTargetStudentCount.HasValue )
            {
                SaveErrorRecord(record, StudentSectionStatus.TARGET_AND_GROUP_STUDENT_COUNT_ARE_NULL, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.TARGET_AND_GROUP_STUDENT_COUNT_ARE_NULL };
            }

            if ( record.GroupNumber.HasValue && record.TargetStudentCount < 1 && record.GroupTargetStudentCount < 1 )
            {
                SaveErrorRecord(record, StudentSectionStatus.TARGET_STUDENT_COUNT_IS_LESS_THAN_1, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.TARGET_STUDENT_COUNT_IS_LESS_THAN_1 };
            }

            if ( !record.GroupNumber.HasValue )
            {
                SaveErrorRecord(record, StudentSectionStatus.MISSING_GROUP_NUMBER, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_GROUP_NUMBER };
            }

            if ( record.Si_PseudoRegistratonTrackingID.HasValue )
            {
                SaveErrorRecord(record, StudentSectionStatus.PSEUDO_REGISTRATION_STUDENT_CANNOT_BE_PART_OF_A_COHORT, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.PSEUDO_REGISTRATION_STUDENT_CANNOT_BE_PART_OF_A_COHORT };
            }

            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public Error IsInclusiveRecordClean(PreLoadStudentSection record, ISectionErrorRepository sectionErrorRepository, Job job)
        {
            if (record.GroupNumber.HasValue && record.TargetStudentCount == 0 )
            {
                SaveErrorRecord(record, StudentSectionStatus.TARGET_STUDENT_COUNT_IS_ZERO, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.TARGET_STUDENT_COUNT_IS_ZERO };
            }

            if (record.GroupNumber.HasValue && record.TargetStudentCount < 1 )
            {
                SaveErrorRecord(record, StudentSectionStatus.TARGET_STUDENT_COUNT_IS_LESS_THAN_1, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.TARGET_STUDENT_COUNT_IS_LESS_THAN_1 };
            }

            if (!record.GroupNumber.HasValue)
            {
                SaveErrorRecord(record, StudentSectionStatus.MISSING_GROUP_NUMBER, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.MISSING_GROUP_NUMBER };
            }

            if ( record.Si_PseudoRegistratonTrackingID.HasValue )
            {
                SaveErrorRecord(record, StudentSectionStatus.PSEUDO_REGISTRATION_STUDENT_CANNOT_BE_PART_OF_A_COHORT, sectionErrorRepository, job);
                return new Error() { ErrorFound = true, ErrorType = StudentSectionStatus.PSEUDO_REGISTRATION_STUDENT_CANNOT_BE_PART_OF_A_COHORT };
            }
            return new Error() { ErrorFound = false, ErrorType = StudentSectionStatus.NO_ERROR };
        }

        public void SaveErrorRecord(PreLoadStudentSection section, int errorNumber, ISectionErrorRepository sectionErrorRepository, Job job)
        {
            StudentSectionError sectionError = new StudentSectionError();
            _mapper.Map<PreLoadStudentSection, StudentSectionError>(section, sectionError);
            sectionError.StatusID = errorNumber;
            sectionError.JobID = job.Id;
            sectionErrorRepository.Add(sectionError);
        }

        public void SaveErrorRecord(PreviewStudentSection section, int errorNumber, ISectionErrorRepository sectionErrorRepository)
        {
            StudentSectionError sectionError = new StudentSectionError();
            _mapper.Map<PreviewStudentSection, StudentSectionError>(section, sectionError);
            sectionError.StatusID = errorNumber;
            sectionErrorRepository.Add(sectionError);
        }
    }
}
