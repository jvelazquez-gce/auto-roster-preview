using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Models.Helper
{
    public class OneStudentPerSectionResults
    {
        public List<int> SectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist { get; set; } = new List<int>();
        public List<CourseSection> CourseSectionsToDeleteOrInactivate = new List<CourseSection>();
        public List<int> SectionAdClassSchedIDsFromCVueToCancel { get; set; } = new List<int>();
        public List<CourseSection> CreatedSectionsToCancel { get; set; } = new List<CourseSection>();
        public OneStudentPerSectionPair NewlyPairRecordsSaveToDbToBeCreatedAndTransfer { get; set; } = new OneStudentPerSectionPair();
        public CalcModel CalcModel { get; set; } = new CalcModel();
        public List<LiveStudentSection> LiveStudentSectionList { get; set; } = new List<LiveStudentSection>();
        public List<OneStudentPerSectionRecord> OneStudentPerSectionList { get; set; } = new List<OneStudentPerSectionRecord>();

    }
}
