using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Models.Helper
{
    public class CalcModel
    {
        public List<Rule> Rules { get; set; } = new List<Rule>();
        public List<LastClassGroupStudentSection> LastClassGroupStudentSections { get; set; } = new List<LastClassGroupStudentSection>();
        public List<ClassGroupStudentSection> ClassGroupStudentSections { get; set; } = new List<ClassGroupStudentSection>();
        public List<OneStudentPerSectionRecord> OneStudentPerSectionStudentRecords { get; set; } = new List<OneStudentPerSectionRecord>();
        public List<PreviewStudentSection> PreviewStudentRecords { get; set; } = new List<PreviewStudentSection>();
        public List<LiveStudentSection> LiveStudentRecords { get; set; } = new List<LiveStudentSection>();
        public List<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
        public List<CourseSection> CourseSectionsThatWereNotReUsed { get; set; } = new List<CourseSection>();
        public List<CourseSection> CourseSectionsToBeCancelled { get; set; } = new List<CourseSection>();
        public List<CourseSection> ForeCastedSections { get; set; } = new List<CourseSection>();
        public List<PreLoadStudentSection> PreloadRecordsWithoutStudentIDs { get; set; } = new List<PreLoadStudentSection>();
        public List<PreLoadStudentSection> SectionsToExclude { get; set; } = new List<PreLoadStudentSection>();
        public int TotalStudentsRegistered { get; set; }
        public int TotalSectionsNeeded { get; set; }
        public int ForecastSectionsNeeded { get; set; }
        public int MaxStudentsPerSection { get; set; }
        public int CourseID { get; set; }
        public DateTime StartDate { get; set; }
        public bool CourseStartDateListOnlyHasClassesWithZeroStudents { get; set; } = false;
        public int JobStatusId { get; set; }
    }
}
