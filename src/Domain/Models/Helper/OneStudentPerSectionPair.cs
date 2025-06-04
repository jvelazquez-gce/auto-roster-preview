using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Models.Helper
{
    public class OneStudentPerSectionPair
    {
        public List<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
        public List<OneStudentPerSectionRecord> OneStudentPerSectionList { get; set; } = new List<OneStudentPerSectionRecord>();
        public int StatusID { get; set; }
    }
}
