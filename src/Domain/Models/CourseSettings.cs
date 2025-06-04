using System;

namespace Domain.Models
{
    public class CourseSettings
    {
        public int Id { get; set; }
        public Guid XrmId { get; set; }
        public int StatusId { get; set; }
        public int? AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public DateTime EarliestClassStartDate { get; set; }
        public DateTime LatestClassStartDate { get; set; }
        public int TargetStudentCount { get; set; }
        public int MaxStudentCount { get; set; }
        public int MinStudentCount { get; set; }
        public int RosterBalanaceDaysInAdvance { get; set; }
        public bool IsGlobalSetting { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateOn { get; set; }
        public DateTime ImportedOn { get; set; }
    }
}
