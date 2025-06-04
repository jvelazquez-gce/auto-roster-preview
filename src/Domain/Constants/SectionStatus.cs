namespace Domain.Constants
{
    public static class SectionStatus
    {
        public const int READY_TO_BE_REUSED = 58;

        public const int READY_TO_CREATE_SECTION_IN_CVUE = 59;
        public const int SUCCESSFULLY_CREATED_SECTION_IN_CVUE = 60;
        public const int FAIL_TO_BE_CREATE_SECTION_IN_CVUE = 61;
        public const int FAIL_TO_CREATE_SECTION_IN_CVUE_WITH_MISSING_CORRELATION_ID = 62;

        public const int SECTION_DOES_NOT_NEED_TO_BE_UPDATED_BECAUSE_BOTH_ARB_AND_CVUE_MAX_NUMBERS_ARE_EQUAL = 63;
        public const int SECTION_CAN_NOT_BE_UPDATED_BECAUSE_NEW_ARB_MAX_IS_LESS_THAN_CURRENT_MAX_NUMBER = 64;
        public const int READY_TO_UPDATE_SECTION_MAX_STUDENTS_IN_CVUE = 65;
        public const int SUCCESSFULLY_UPDATED_SECTION_MAX_STUDNETS_IN_CVUE = 66;
        public const int FAIL_TO_UPDATE_SECTION_MAX_STUDENTS_IN_CVUE = 67;
        public const int FAIL_TO_UPDATE_SECTION_MAX_STUDENTS_IN_CVUE_WITH_MISSING_CORRELATION_ID = 68;

        public const int READY_TO_CANCEL_SECTION_IN_CVUE = 69;
        public const int SUCCESSFULLY_CANCELLED_SECTION_IN_CVUE = 70;
        public const int FAIL_TO_CANCEL_SECTION_IN_CVUE = 71;
        public const int FAIL_TO_CANCEL_SECTION_IN_CVUE_WITH_MISSING_CORRELATION_ID = 72;

        public const int ERROR_DUPLICATE_SECTION_CODE_NAMES = 73;

        public const int ACTIVE = 74;
        public const int INACTIVE = 75;
    }
}
