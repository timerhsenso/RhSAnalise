using AutoMapper;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Application.Mapping;

/// <summary>Profile AutoMapper para Sistema.</summary>
public sealed class SistemaProfile : Profile
{
    public SistemaProfile()
    {
        CreateMap<Sistema, SistemaDto>();

        CreateMap<CreateSistemaRequest, Sistema>()
            .ForMember(d => d.CdSistema, opt => opt.MapFrom(s => s.CdSistema.Trim()))
            .ForMember(d => d.DcSistema, opt => opt.MapFrom(s => s.DcSistema.Trim()))
            .ForMember(d => d.Ativo, opt => opt.MapFrom(s => s.Ativo));

        // Removido ForAllOtherMembers para compatibilidade ampla.
        CreateMap<UpdateSistemaRequest, Sistema>()
            .ForMember(d => d.DcSistema, opt => opt.MapFrom(s => s.DcSistema.Trim()))
            .ForMember(d => d.Ativo, opt => opt.MapFrom(s => s.Ativo));
    }
}
