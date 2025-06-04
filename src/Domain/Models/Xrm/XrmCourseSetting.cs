using System;

namespace Domain.Models.Xrm
{
    public class XrmCourseSetting
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Guid XrmId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int Status { get; set; }
        public int? StatusReason { get; set; }
        public string CourseRosterBalanceSettingCode { get; set; }
        public DateTime EarliestClassStartDate { get; set; }
        public DateTime? LatestClassStartDate { get; set; }
        public int TargetStudentCount { get; set; }
        public int MaxStudentCount { get; set; }
        public int? MinStudentCount { get; set; }
        public int RosterBalanceDaysInAdvanceOverride { get; set; }
        public Guid? CourseID { get; set; }
    }
}
