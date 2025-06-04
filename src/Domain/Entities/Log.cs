using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Logs")]
    public class Log
    {
        public int Id { set; get; }
        public int JobID { get; set; }
        public Guid ProcessID { get; set; }
        public int CourseID { get; set; }
        public DateTime CourseStartDate { get; set; }
        public DateTime Date { get; set; }
        public string Thread { set; get; }
        public string Level { get; set; }
        public string Logger { set; get; }
        public string Message { get; set; }
        public string Exception { set; get; }
    }
}
