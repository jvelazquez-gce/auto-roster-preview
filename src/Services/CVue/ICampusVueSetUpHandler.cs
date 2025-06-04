using Domain.Entities;
using Domain.Models.Helper;

namespace Services.CVue
{
    public interface ICampusVueSetUpHandler
    {
        void CreateLiveJobRecords(CalcModel calcModel, Job job);
    }
}
