using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.OneStudentPerSection
{
    public interface IOneStudentPerSectionCVueSectionsHandler
    {
        OneStudentPerSectionResults AddUpdateAndInactivateCampusVueSectionsToCancel(Parameters p);

        OneStudentPerSectionResults InactivateAndDeleteCVueSectionsToBeCancelAndGetDeletedSectionsThatNeedToBeReAdded(Parameters p, List<int> SectionAdClassSchedIDsFromCVueToCancel);

        List<CourseSection> CreateCourseSectionsToCancel(Parameters p, List<int> SectionAdClassSchedIDsFromCVueToAddToDbToCancel);
    }
}
