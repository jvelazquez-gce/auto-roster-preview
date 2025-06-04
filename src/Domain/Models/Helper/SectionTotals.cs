using System;

namespace Domain.Models.Helper
{
    public class SectionTotals : ICloneable
    {
        public string SectionCode { get; set; }
        public int GroupCategory { get; set; }
        public int TotalStudentsInSection { get; set; }
        public int TotalEmptySeats { get; set; }
        public object Clone() { return this.MemberwiseClone(); }

    }
}
