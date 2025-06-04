namespace Domain.Constants
{
    public static class StudentSectionStatus
    {
        public const int INVALID_COHORT_GROUP_TYPE_KEY = 21;
        public const int MISSING_GROUP_NUMBER = 22;
        public const int TARGET_STUDENT_COUNT_IS_LESS_THAN_1 = 23;
        public const int TARGET_STUDENT_COUNT_IS_LESS_THAN_2 = 24;
        public const int TARGET_AND_GROUP_STUDENT_COUNT_ARE_NULL = 25;
        public const int TARGET_STUDENT_COUNT_IS_ZERO = 26;
        public const int MISSING_REQUIRED_DATAFIELD_VALUE = 27;
        public const int START_DATE_IS_LESS_THAN_ALLOWED = 28;
        public const int MORE_THAN_ALLOWED_PARTIAL_LOCATION_SECTIONS = 29;
        public const int UNEXPECTED_ERROR = 30;
        public const int MISSING_COUNSELOR_LOCATION_CODE = 31;
        public const int INVALID_COUNSELOR_LOCATION_CODE = 32;
        public const int MISSING_OR_INVALID_TRANSFER_DATA_FIELD = 33;
        public const int DOES_NOT_NEED_TO_BE_TRANSFER_BECAUSE_SOURCE_AND_DESTINATION_ARE_EQUAL = 34;
        public const int READY_TO_BE_TRANSFER_IN_CVUE = 35;
        public const int SUCCESSFULLY_TRANSFERED_IN_CVUE = 36;
        public const int FAIL_TO_BE_TRANSFERED_IN_CVUE = 37;
        public const int FAIL_TO_BE_TRANSFERED_IN_CVUE_WITH_MISSING_CORRELATION_ID = 38;
        public const int MISSING_COURSE_SECTION = 39;
        public const int MISSING_STUDENT_ID = 40;
        public const int MISSING_ENROLL_ID = 41;
        public const int MISSING_AD_ENROLL_SCHED_ID = 42;
        public const int MISSING_SI_PSEUDO_REGISTRATION_TRACKING_ID = 43;
        public const int PSEUDO_REGISTRATION_STUDENT_CANNOT_BE_PART_OF_A_COHORT = 44;
        public const int NULL_VALUE = 45;
        public const int NO_ERROR = 46;
        public const int EXCLUDED_FROM_ARB_RULE = 47;
        public const int CANCELLED_DUE_TO_STUDENT_DROPPING_OFF = 48;
        public const int TARGET_STUDENT_COUNT_IS_NOT_ONE = 49;
        public const int CANCELLED_DUE_TO_CVUE_SECTION_DROPPING_OFF = 50;
        public const int NON_SUPER_SECTION_NOT_ALLOWED_IN_ONE_STUDENT_PER_SECTION = 51;
        public const int MIX_RECORDS_NOT_ALLOWED_IN_ONE_STUDENT_PER_SECTION = 52;
        public const int CHECK_FEATURE_FLAGS = 53;
    }
}
