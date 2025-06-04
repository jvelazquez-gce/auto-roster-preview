using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using System;

namespace Infrastructure.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PreLoadStudentSection, StudentSectionError>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB));

            CreateMap<PreviewStudentSection, StudentSectionError>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB));

            CreateMap<LiveStudentSection, StudentSectionError>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB));

            CreateMap<PreLoadStudentSection, PreviewStudentSection>()
                .ForMember(dest => dest.OldSectionCode, opt => opt.MapFrom(src => src.SectionCode))
                .ForMember(dest => dest.OldSectionId, opt => opt.MapFrom(src => src.AdClassSchedID))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.AdTeacherID, opt => opt.NullSubstitute(0));

            CreateMap<PreLoadStudentSection, LiveStudentSection>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0));

            CreateMap<PreviewStudentSection, LiveStudentSection>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0));

            CreateMap<OneStudentPerSectionRecord, LiveStudentSection>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0));

            CreateMap<LastClassGroupStudentSection, LiveStudentSection>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0));

            CreateMap<PreLoadStudentSection, LastClassGroupStudentSection>()
                .ForMember(dest => dest.OldSectionCode, opt => opt.MapFrom(src => src.SectionCode))
                .ForMember(dest => dest.OldSectionId, opt => opt.MapFrom(src => src.AdClassSchedID))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB));

            CreateMap<OneStudentPerSectionRecord, StudentSectionError>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0));

            CreateMap<PreLoadStudentSection, OneStudentPerSectionRecord>()
                .ForMember(dest => dest.OldSectionCode, opt => opt.MapFrom(src => src.SectionCode))
                .ForMember(dest => dest.OldSectionId, opt => opt.MapFrom(src => src.AdClassSchedID))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB));

            CreateMap<PreLoadStudentSection, ClassGroupStudentSection>()
                .ForMember(dest => dest.OldSectionCode, opt => opt.MapFrom(src => src.SectionCode))
                .ForMember(dest => dest.OldSectionId, opt => opt.MapFrom(src => src.AdClassSchedID))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => GeneralStatus.ARB_JOB));
        }
    }
}