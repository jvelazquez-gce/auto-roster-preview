using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Other
{
    [Table("Configurations")]
    public class Configuration
    {
        public int ID { get; set; }

        [Display(Name = "Configuration Name")]
        public string Name { get; set; }

        [Display(Name = "Configuration Value")]
        public string Value { get; set; }

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
