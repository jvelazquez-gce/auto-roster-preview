using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ClassesTogetherToGroupRules")]
    public class ClassesTogetherToGroupRule
    {
        public int Id { set; get; }
        public int AdCourseID { get; set; }
        public string CourseCode { set; get; }
        public int AdCourseIdToGroupBy { get; set; }
        public string CourseCodeToGroupBy { get; set; }
    }
}
