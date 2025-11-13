// src/Modules/GestaoDePessoas/Application/Mappings/MunicipioProfile.cs

using AutoMapper;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.Mappings;

/// <summary>
/// Profile AutoMapper para Municipio.
/// </summary>
public sealed class MunicipioProfile : Profile
{
    public MunicipioProfile()
    {
        CreateMap<Municipio, MunicipioDto>();
        CreateMap<MunicipioDto, Municipio>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id é gerado no controller
    }
}