using System.Collections.Generic;

namespace Domain.Constants
{
    public static class Group
    {
       
        public const int EXCLUSIVE_COHORT = 804430000;
        public const int INCLUSIVE_COHORT = 804430001;
        public const int FRIEND_COHORT = 804430002;
        public const int LAST_CLASS_TOGETHER_COHORT = 100;
        public const int LAST_CLASS_TOGETHER_NO_COHORT = 101;
        public const int LAST_CLASS_TOGETHER_NO_COHORT_NO_EMPTY_SEAT_FOUND = 102;

        public const int IN_RULE_GROUP = 103;
        public const int NOT_IN_RULE_GROUP = 104;
        public const int NOT_IN_RULE_GROUP_NO_EMPTY_SEAT_FOUND = 105;

        public static List<int> COHORT_KEY_LIST => new List<int>() { EXCLUSIVE_COHORT, INCLUSIVE_COHORT, FRIEND_COHORT };

        public static List<int> NON_EXCLUSIVE_COHORT_KEY_LIST => new List<int>() { INCLUSIVE_COHORT, FRIEND_COHORT };

        public static List<int> NON_EXCLUSIVE_COHORT_CATEGORY_LIST => new List<int>() { ARBGroupCategory.INCLUSIVE, ARBGroupCategory.FRIEND };
    }
}
