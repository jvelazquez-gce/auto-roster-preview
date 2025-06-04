using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Models.Helper
{
    public class LastClassTogetherGroup
    {
        public LastClassTogetherGroup() { StudentRecords = new List<PreviewStudentSection>(); }
        public int LastClassTakenId { get; set; }
        public int TotalStudents { get; set; }

        public List<PreviewStudentSection> StudentRecords { get; set; }
    }
}
