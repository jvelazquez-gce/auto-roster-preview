using Domain.Constants;
using System;
using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Models.Helper
{
    public class QueryInputParams
    {
        public DateTime CutOffStartDateTime { get; set; }
        public DateTime CutOffEndDateTime { get; set; }
        public DateTime DateToStartDeletingCourses { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime AddedOn { get; set; }
        public int AdCourseID { get; set; }
        public Job Job { get; set; }
        public Feature FeatureName { get; set; }
        public List<int> IdsToDelete { get; set; }
        public bool ExcludeSuperSection { get; set; }
    }
}
