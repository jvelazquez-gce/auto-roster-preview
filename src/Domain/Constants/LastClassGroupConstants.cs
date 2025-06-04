namespace Domain.Constants
{
    public static class LastClassGroupConstants
    {
        public const int DNP_955_COURSE_ID = 32254;
        public const int DNP_960_COURSE_ID = 32255;
        public const int DNP_965_COURSE_ID = 32256;

        public const int DNP_955A_COURSE_ID = 34613;
        public const int DNP_960A_COURSE_ID = 34614;
        public const int DNP_965A_COURSE_ID = 34615;

        public const int CNL_624_COURSE_ID = 34044;
        public const int CNL_664A_COURSE_ID = 34046;
        public const int CNL_664B_COURSE_ID = 34047;

        public const string ONE_STUDENT_ONLY_COHORT_GROUP_NUMBER = "11111111-1111-1111-1111-111111111111";
        public const string ONE_STUDENT_ONLY_COHORT_GROUP_NO_EMPTY_SEAT_FOUND_NUMBER = "22222222-2222-2222-2222-222222222222";

        //public static List<int> COURSE_IDS_THAT_NEED_TO_BE_GROUPED = new List<int>()
        //{
        //    DNP_960_COURSE_ID,
        //    DNP_965_COURSE_ID,
        //    DNP_960A_COURSE_ID,
        //    DNP_965A_COURSE_ID,
        //    CNL_664A_COURSE_ID,
        //    CNL_664B_COURSE_ID,
        //};

        //public static List<int> VALID_LAST_COURSE_IDs_TAKEN = new List<int>()
        //{
        //    DNP_955_COURSE_ID,
        //    DNP_960_COURSE_ID,
        //    DNP_965_COURSE_ID,
        //    DNP_955A_COURSE_ID,
        //    DNP_960A_COURSE_ID,
        //    DNP_965A_COURSE_ID,
        //    CNL_624_COURSE_ID,
        //    CNL_664A_COURSE_ID,
        //    CNL_664B_COURSE_ID,
        //};

        public const int Error_Invalid_LastAdClassSchedIDTaken = 300;
        public const int Error_Invalid_Null_LastAdCourseIDTaken = 301;
        public const int Error_Invalid_GroupTypeKey = 302;
        public const int Error_Invalid_GroupNumber = 303;
        public const int Error_Invalid_SuperSection = 304;
        public const int Error_Invalid_LastAdCourseIDTaken = 305;
    }
}
