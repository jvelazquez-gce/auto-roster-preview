using Domain.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("PreLoadStudentSections")]
    [Serializable]
    public class PreLoadStudentSection : ICloneable
    {
        public PreLoadStudentSection() {}

        public int PreLoadStudentSectionID { get; set; }
        public int AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public string SectionCode { get; set; }
        public int AdClassSchedID { get; set; }
        public DateTime StartDate { get; set; }

        [NotMapped]
        public string ProgramStartMonthAndDay => ProgramStartMonth.ToString() + ProgramStartDay.ToString();

        public int? ProgramStartDay { get; set; }
        public int? ProgramStartMonth { get; set; }
        public string ProgramLetterInitial { get; set; } = string.Empty;
        public int? SyStudentID { get; set; }
        public int? Si_PseudoRegistratonTrackingID { get; set; }
        public int? ForcastedNumberOfStudents { get; set; }
        public int CVueMaxStudents { get; set; }
        public int TargetStudentCount { get; set; }
        public int? AdProgramVersionID { get; set; }
        public int? LastAdClassSchedIDTaken { get; set; }
        public string ScheduleGroupName { get; set; }
        public int? AdEnrollID { get; set; }
        public int? AdEnrollSchedID { get; set; }
        public Guid? GroupNumber { get; set; }
        public int? GroupTypeKey { get; set; }
        public int? CounselorLocationCode { get; set; }
        public int? GroupStatusKey { get; set; }
        public int? GroupTargetStudentCount { get; set; }
        public int? GroupMinimumStudentCount { get; set; }
        public bool HasInstructor { get; set; } = false;
        public int? InstructorAssignedStatusID { get; set; }
        public int? AdTeacherID { get; set; } = 0;
        public string PrimaryInstructor { get; set; }
        public int? InstructorType { get; set; }
        public int? FacultyType { get; set; }
        public string FacultyTypeName { get; set; }

        public string Term { get; set; }

        [NotMapped]
        public int GroupCategory { get; set; }
        public bool IsTheCourseTaughtByFullTimeFaculty { get; set; } = false;
        public int? SysLmsVendorID { get; set; }
        public Guid ProcessID { get; set; }
        public int StatusID { get; set; } = SectionStatus.ACTIVE;
        public DateTime? EndDate { get; set; }
        public string LmsExtractStatus { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int AdTermID { get; set; }
        public bool ReuseEmptySections { get; set; }
        public int? LastAdCourseIDTaken { get; set; }

        public object Clone() { return this.MemberwiseClone(); }
    }
}
