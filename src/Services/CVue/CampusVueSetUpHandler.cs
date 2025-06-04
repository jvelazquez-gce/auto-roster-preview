using System.Collections.Generic;
using System.Linq;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;

namespace Services.CVue
{
    public class CampusVueSetUpHandler : ICampusVueSetUpHandler
    {
        private readonly IMapper _mapper;
        public CampusVueSetUpHandler(IMapper mapper, Parameters p)
        {
            _mapper = mapper;
        }

        public void CreateLiveJobRecords(CalcModel calcModel, Job job)
        {
            _mapper.Map<List<PreviewStudentSection>, List<LiveStudentSection>>(calcModel.PreviewStudentRecords, calcModel.LiveStudentRecords);

            if (calcModel.OneStudentPerSectionStudentRecords.Any())
            {
                var oneStudentPerSectionLiveRecords = new List<LiveStudentSection>();
                _mapper.Map<List<OneStudentPerSectionRecord>, List<LiveStudentSection>>(calcModel.OneStudentPerSectionStudentRecords, oneStudentPerSectionLiveRecords);
                oneStudentPerSectionLiveRecords.ForEach(r => r.JobID = job.Id);
                calcModel.LiveStudentRecords.AddRange(oneStudentPerSectionLiveRecords);
            }

            if (calcModel.LastClassGroupStudentSections.Any())
            {
                var lastClassGroupStudentSections = new List<LiveStudentSection>();
                _mapper.Map<List<LastClassGroupStudentSection>, List<LiveStudentSection>>(calcModel.LastClassGroupStudentSections, lastClassGroupStudentSections);
                lastClassGroupStudentSections.ForEach(r => r.JobID = job.Id);
                calcModel.LiveStudentRecords.AddRange(lastClassGroupStudentSections);
            }
        }
    }
}
