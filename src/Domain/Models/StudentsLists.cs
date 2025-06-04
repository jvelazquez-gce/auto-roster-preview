using System.Collections.Generic;

namespace Domain.Models
{
    public class StudentSectionLists
    {
        public StudentSectionLists()
        {
            RemoveStudents = new List<Entities.PreviewStudentSection>();
            AddedStudents = new List<Entities.PreviewStudentSection>();
        }
        public List<Entities.PreviewStudentSection> RemoveStudents { get; set; }
        public List<Entities.PreviewStudentSection> AddedStudents { get; set; }
    }
}
