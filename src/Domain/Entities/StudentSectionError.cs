using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("StudentSectionErrors")]
    public class StudentSectionError
    {
        public StudentSectionError()
        {
            OneDayBeforeRunningLive = false;
        }
        public int Id { get; set; }
        public int AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public string SectionCode { get; set; }
        public int? AdClassSchedID { get; set; }
        public string OldSectionCode { get; set; }
        public int? OldSectionId { get; set; }
        public int? NewSectionId { get; set; }

        public DateTime StartDate { get; set; }
        public string ProgramStartMonthAndDay { get; set; }
        public int? ProgramStartDay { get; set; }
        public int? ProgramStartMonth { get; set; }
        public string ProgramLetterInitial { get; set; }
        public int? SyStudentID { get; set; }
        public int? Si_PseudoRegistratonTrackingID { get; set; }
        public int? ForcastedNumberOfStudents { get; set; }
        public int? CVueMaxStudents { get; set; }
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
        public bool HasInstructor { get; set; }
        public int? InstructorAssignedStatusID { get; set; }
        public int? AdTeacherID { get; set; }
        public string PrimaryInstructor { get; set; }
        public int? FacultyType { get; set; }
        public string FacultyTypeName { get; set; }

        public string Term { get; set; }
        public string Campus { get; set; }
        public int? NewGroupCategory { get; set; }

        public bool IsTheCourseTaughtByFullTimeFaculty { get; set; }
        public int? SysLmsVendorID { get; set; }
        public Guid ProcessID { get; set; }
        public int JobID { get; set; }
        public int StatusID { get; set; }
        public bool OneDayBeforeRunningLive { get; set; }

        public Guid? SectionGuid { get; set; }
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
        public string ErrorMessage { get; set; }

        public virtual Job Job { get; set; }
        public virtual Status Status { get; set; }
    }
}
