using Domain.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("CourseSections")]
    public class CourseSection
    {
        public CourseSection()
        {
            AdClassSchedId = null;
            Active = false;
            FinalRegStudents = 0;
            FinalRegDropStudents = 0;
            IsManagedByARB = true;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public int JobID { get; set; }
        public int? AdClassSchedId { get; set; }
        public string CampusCode { get; set; }
        public int AdCourseID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CourseCode { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalSystemId { get; set; }
        public string TermCode { get; set; }
        public int? OriginalCVueMaximumNumberOfStudents { get; set; }
        public int MaximumNumberOfStudents { get; set; }
        public int? MinimumNumberOfStudents { get; set; }
        public int? ForcastedNumberOfStudents { get; set; }
        public GeneralStatus.PassFailType PassFailSetting { get; set; }
        public string PrimaryInstructor { get; set; }
        public int? InstructorType { get; set; }
        public int? AdTeacherID { get; set; }

        public string SectionCode { get; set; }

        public string SectionDeliveryMethod { get; set; }
        [NotMapped]
        public string ReasonToUnregisterStudents { get; set; }
        public bool HasInstructor { get; set; }
        public int? InstructorAssignedStatusID { get; set; }

        public bool HasSectionBeenCreatedOrUpdatedInCampusVue { get; set; }
        public int GroupCategory { get; set; }
        public int StatusID { get; set; }

        public int? SyLmsVendorID { get; set; }
        public int? UserID { get; set; }
        public string EmployeeType { get; set; }
        public bool Active { get; set; }
        public int FinalRegStudents { get; set; }
        public int FinalRegDropStudents { get; set; }
        public string LmsExtractStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsManagedByARB { get; set; }
        public bool ForecastSection { get; set; }
        public int AdTermID { get; set; }
        public virtual Status Status { get; set; }
    }
}
