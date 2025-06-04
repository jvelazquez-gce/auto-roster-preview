using System;

namespace Domain.TestModels
{
    public class DataTest
    {
        public DataTest()
        {
            UseMaxStudentCount = false;
            IsTheCourseTaughtByFullTimeFaculty = false;
        }

        public int Id { get; set; }
        public int AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public string SectionCode { get; set; }
        public int AdClassSchedID { get; set; }
        public DateTime StartDate { get; set; }
        public int SyStudentID { get; set; }
        public int TargetStudentCount { get; set; }
        public int MaxStudentCount { get; set; }
        public int MinStudentCount { get; set; }
        public int AdProgramVersionID { get; set; }
        public int? LastAdClassSchedIDTaken { get; set; }
        public string ScheduleGroupName { get; set; }
        public int AdEnrollID { get; set; }
        public Guid? GroupNumber { get; set; }
        public int? GroupTypeKey { get; set; }
        public int? GroupFacultyLocationKey { get; set; }
        public int? GroupStatusKey { get; set; }
        public int? GroupTargetStudentCount { get; set; }
        public int? GroupMinStudentCount { get; set; }
        public bool UseMaxStudentCount { get; set; }
        public bool IsTheCourseTaughtByFullTimeFaculty { get; set; }
    }
}
