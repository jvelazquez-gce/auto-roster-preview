using System;
using Domain.Constants;

namespace Domain.Entities
{
    public class ClassGroupStudentSection
    {
        public int Id { get; set; }
        public int AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public string SectionCode { get; set; }
        public string OldSectionCode { get; set; }
        public int? OldSectionId { get; set; } = null;
        public int? NewSectionId { get; set; } = null;
        public DateTime StartDate { get; set; }
        public string ProgramStartMonthAndDay { get; set; }
        public string ProgramLetterInitial { get; set; }

        public int SyStudentID { get; set; }
        public int? Si_PseudoRegistratonTrackingID { get; set; }
        public int? ForcastedNumberOfStudents { get; set; }

        public int TargetStudentCount { get; set; }
        public int AdProgramVersionID { get; set; }
        public int? LastAdClassSchedIDTaken { get; set; }
        public string ScheduleGroupName { get; set; }
        public int AdEnrollID { get; set; }
        public int? AdEnrollSchedID { get; set; }
        public Guid? GroupNumber { get; set; }
        public int? GroupTypeKey { get; set; }
        public int? CounselorLocationCode { get; set; }
        public int? GroupStatusKey { get; set; }
        public int? GroupTargetStudentCount { get; set; }
        public int? GroupMinimumStudentCount { get; set; }
        public int JobID { get; set; }
        public int StatusID { get; set; }
        public string Term { get; set; }
        public string Campus { get; set; } = GeneralStatus.CAMPUS_ONLINE;
        public int? AdTeacherID { get; set; }
        public int? InstructorAssignedStatusID { get; set; }
        public int GroupCategory { get; set; } = ARBGroupCategory.GENERAL;
        public bool IsTheCourseTaughtByFullTimeFaculty { get; set; }
        public int SysLmsVendorID { get; set; }
        public bool OneDayBeforeRunningLive { get; set; } = false;
        public Guid SectionGuid { get; set; }
        public Guid ProcessID { get; set; }
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
        public int? LastAdCourseIDTaken { get; set; }

        public virtual Status Status { get; set; }
    }
}
