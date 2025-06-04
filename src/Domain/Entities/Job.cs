using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Jobs")]
    public class Job
    {
        public Job() { }
        [Display(Name = "Job Id")]
        public int Id { get; set; }
        [Display(Name = "Job start time")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Job completion time")]
        public DateTime? CompletionDate { get; set; }
        public int? StatusId { get; set; }
        [Display(Name = "Input data set ID: ")]
        public Guid ProcessID { get; set; }
        [NotMapped]
        public string JobMessage { get; set; }
        //[NotMapped]
        //public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<PreviewStudentSection> Sections { get; set; }
    }
}
