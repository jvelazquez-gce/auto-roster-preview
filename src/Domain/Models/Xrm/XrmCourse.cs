using System;

namespace Domain.Models.Xrm
{
    public class XrmCourse
    {
        public int Id { get; set; }
        public Guid XrmCourseId { get; set; }
        public string CVCourseId { get; set; }
        
        public bool IsRosterBalanced { get; set; }
        public int Status { get; set; }
        public int? StatusReason { get; set; }
    }
}
