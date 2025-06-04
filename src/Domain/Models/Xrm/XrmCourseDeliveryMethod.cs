using System;

namespace Domain.Models.Xrm
{
    public class XrmCourseDeliveryMethod
    {
        public int Id { get; set; }
        public string CourseDeliveryMethodCode { get; set; }
        public Guid CourseDeliveryMethodId { get; set; }
        public Guid CourseCodeLKP { get; set; }
        public string CourseCodeLKPName { get; set; }
        public string DeliveryMethodLKPName { get; set; }
        public int FacultyType { get; set; }
        public string FacultyTypeName { get; set; }
    }
}
