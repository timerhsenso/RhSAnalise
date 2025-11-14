using AutoMapper;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.Mappings
{
    /// <summary>Profile de mapeamento Sindicato</summary>
    public sealed class SindicatoProfile : Profile
    {
        public SindicatoProfile()
        {
            CreateMap<Sindicato, SindicatoDto>().ReverseMap();

            CreateMap<CreateSindicatoDto, Sindicato>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateSindicatoDto, Sindicato>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CodigoSindicato, opt => opt.Ignore());
        }
    }
}
