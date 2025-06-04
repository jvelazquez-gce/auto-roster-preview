using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Rules")]
    public class Rule
    {
        [Key]
        public long RuleId { get; set; }
        // adProgramVersionID
        public string FieldName { get; set; }
        // 3315
        public string FieldExpectedValue { get; set; }
        // "int"
        public string FieldExpectedValueType { get; set; }
        public int AdCourseID { get; set; }
        public string CourseCode { get; set; }
        public string SectionPrefix { get; set; }
        public string SectionBaseCounterFirstDigitString { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
