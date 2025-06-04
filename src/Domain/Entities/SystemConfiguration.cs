using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("SystemConfigurations")]
    public class SystemConfiguration
    {
        public int ID { get; set; }

        [Display(Name = "ARB Enabled")]
        public bool RunARBProcess { get; set; }


        [Display(Name = "Days before Live job runs")]
        public int DaysBeforeLiveJobRuns { get; set; }


        [Display(Name = "Default Instructor Email")]
        public string DefaultInstructorEmail { get; set; }


        [Display(Name = "Default Instructor ID")]
        public int DefaultInstructorID { get; set; }


        [Display(Name = "Cohort min size")]
        public int CohortSectionMinSize { get; set; }

        [Display(Name = "Records to transfer batch size")]
        public int MaxTransferRecordsPerBatch { get; set; }

        [Display(Name = "Sections to Create/update batch size")]
        public int CMCMaxSectionsPerBatch { get; set; }

        [Display(Name = "Number of days when to start deleting courses")]
        public int NumberOfDaysWhenToStartDeletingCourses { get; set; }

        [Display(Name = "Max number of forecasted sections")]
        public int MaxNumberOfForecastSections { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }


        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }


        [Display(Name = "Updated on")]
        public DateTime UpdatedOn { get; set; }


        [Display(Name = "Updated by")]
        public string UpdatedBy { get; set; }
    }
}
