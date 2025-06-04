namespace Domain.Constants
{

    public static class ErrorCheckType
    {
        public const int FIRST_CHECK = 140;
        public const int SECOND_CHECK = 141;
    }

    public static class InstructorType
    {
        public const int FULL_TIME = 180;
        public const int ADJUNCT = 181;
    }

    public static class GeneralStatus
    {
        public const string COHORT_NAME_PREFIX = "XO";
        public const string NOCOHORT_NOLOCATION_NAME_PREFIX = "O";
        public const string LAST_CLASS_TOGETHER_COHORT_PREFIX = "P";
        public const int BASE_COUNTER = 500;
        public const int ONE_STUDENT_PER_SECTION_BASE_COUNTER = 600;
        public const int INCLUSIVE_AND_EXCLUSIVE_BASE_COUNTER = 0;
        public const string NOCOHORT_BASE_COUNTER_FIRST_DIGIT_STRING = "5";
        public const string LAST_CLASS_TOGETHER_BASE_COUNTER_FIRST_DIGIT_STRING = "5";
        public const string ONE_STUDENT_PER_SECTION_BASE_COUNTER_FIRST_DIGIT_STRING = "6";
        public const string EXCLUSIVE_GROUP_NAME_PREFIX = "EO";
        public const string INCLUSIVE_GROUP_NAME_PREFIX = "IO";
        public const string FRIEND_GROUP_NAME_PREFIX = "FO";
        public const string TEMPE_GROUP_NAME_PREFIX = "TEM";
        public const string PEORIA_GROUP_NAME_PREFIX = "PEO";
        public const string EAST_GROUP_NAME_PREFIX = "EAST";
        public const string NO_GROUP_NAME_SUFFIX = "N";
        public const string ONLINE_SUPER_SECTION_IDENTIFIER = "OGCU";
        public const string CAMPUS_GROUND = "TRAD";
        public const string CAMPUS_ONLINE = "NONTRAD";
        public const string ONLINE_DELIVERY_METHOD = "ONLINE";
        public const int EXPORT_ARB_INPUT_SUMMARY = 1;
        public const int EXPORT_ARB_INPUT_DETAIL = 2;
        public const int EXPORT_ARB_OUTPUT_SUMMARY = 3;
        public const int EXPORT_ARB_OUTPUT_DETAIL = 4;
        public const int EXPORT_ARB_COURSE_SECTIONS_BY_JOB_ID = 5;
        public const int EXPORT_ARB_COURSE_SECTIONS_BY_COURSEID_AND_START_DATE = 6;
        public const int EXPORT_ARB_INPUT_COURSE_SECTIONS = 7;
        public const int EXPORT_ARB_OUTPUT_COURSE_SECTIONS = 8;
        public const int EXPORT_ARB_ERROR_DETAIL = 9;
        public const int EXPORT_ARB_TOP_LOGS = 10;
        public const int EXPORT_ARB_COURSE_JOB_FATAL_LOGS = 11;
        public const string ARB_JOB = "ArbPreview";
        public const string REASON_1_TO_UNREGISTER_STUDENTS = "FS";
        public const string REASON_2_TO_UNREGISTER_STUDENTS = "WEBDROP";
        public const string REASON_3_TO_UNREGISTER_STUDENTS = "DDA";
        public const string REASON_4_TO_UNREGISTER_STUDENTS = "SSC";
        public const int PSY_885_CVUE_COURSE_ID = 32828;

        public enum PassFailType : int
        {
            UseCoursePassFailSetting = 0,
            NoOptionForPassFail = 1,
            PassFailOnly = 2,
            StudentOption = 4,
            NoUpdate = 5,
        }
    }
}
