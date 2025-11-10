// src/Modules/GestaoDePessoas/Core/Entities/Filial.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Filial : BaseEntity
    {
        public int CodigoEmpresa { get; set; }
        public int CodigoFilial { get; set; }
        public string NomeFantasia { get; set; }
        public string DescricaoEstabelecimento { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string SiglaEstado { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public string Fax { get; set; }
        public string MatriculaINPS { get; set; }
        public string CNPJ { get; set; }
        public string CodigoMunicipio { get; set; }
        public string CodigoAtividadeINPS { get; set; }
        public string CodigoAtividadeIBGE { get; set; }
        public string CodigoNaturezaJuridica { get; set; }
        public string InscricaoEstadual { get; set; }
        public string CodigoMunicipioRAIS { get; set; }
        public string NumeroProprietario { get; set; }
        public string InscricaoCEI { get; set; }
        public string CodigoAtividadeIR { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string CodigoTabelaSalarial { get; set; }
        public string CodigoCalculoDigital { get; set; }
        public string NumeroEndereco { get; set; }
        public string CodigoBancoFGTS { get; set; }
        public string CodigoAgenciaFGTS { get; set; }
        public string CodigoIdentificadorEmpresaCEF { get; set; }
        public string FlagRecolhimentoFGTS { get; set; }
        public string CodigoSimples { get; set; }
        public string FlagCNAE { get; set; }
        public string CodigoFPAS { get; set; }
        public string CodigoAreaTrabalho { get; set; }
        public string CodigoTerceiros { get; set; }
        public string CodigoGPS { get; set; }
        public decimal PercentualConvenio { get; set; }
        public decimal PercentualSAT { get; set; }
        public decimal PercentualTerceiros { get; set; }
        public decimal PercentualEmpresa { get; set; }
        public string NumeroCAGED { get; set; }
        public string Declaracao { get; set; }
        public string Alteracao { get; set; }
        public string InscricaoTerceiros { get; set; }
        public string NomeServico { get; set; }
        public string EnderecoServico { get; set; }
        public string BairroServico { get; set; }
        public string CEPServico { get; set; }
        public string CidadeServico { get; set; }
        public string EstadoServico { get; set; }
        public string TipoInscricaoTerceiros { get; set; }
        public string UltimaMatricula { get; set; }
        public string UltimaFicha { get; set; }
        public string CheckMatricula { get; set; }
        public string CheckNumeroRegistro { get; set; }
        public string CodigoContaAdicional { get; set; }
        public double? PercentualConvenioFloat { get; set; }
        public double? PercentualSATFloat { get; set; }
        public double? PercentualTerceirosFloat { get; set; }
        public double? PercentualEmpresaFloat { get; set; }
        public int? CodigoFornecedor { get; set; }
        public int? CodigoTipoInscricao { get; set; }
        public string CodigoMotivoOcorrenciaHE { get; set; }
        public string CodigoMotivoOcorrenciaFalta { get; set; }
        public int? TipoOcorrenciaHE { get; set; }
        public int? TipoOcorrenciaFalta { get; set; }
        public int FlagAdicionalNoturno { get; set; }
        public string InicioAdicionalNoturno { get; set; }
        public string FimAdicionalNoturno { get; set; }
        public int FlagLimiteTroca { get; set; }
        public int? LimiteTroca { get; set; }
        public short? CodigoEmpresaContabil { get; set; }
        public short? CodigoFilialContabil { get; set; }
        public decimal? ValorHoraAdicional { get; set; }
        public int? FlagDescontaAlmoco { get; set; }
        public int FlagMinimoHE { get; set; }
        public int? ValorMinimoHE { get; set; }
        public int? TipoOcorrenciaAtraso { get; set; }
        public string CodigoMotivoOcorrenciaAtraso { get; set; }
        public string Email { get; set; }
        public string QuantidadeHoraMaxima { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Host { get; set; }
        public decimal? PercentualFAP { get; set; }
        public string CodigoSindicatoResponsavel { get; set; }
        public int? FlagAtivoFilial { get; set; }
        public string DDD { get; set; }
        public string CodigoTipoSistemaRAIS { get; set; }
        public DateTime? DataInicioValidade { get; set; }
        public decimal? PercentualFilantropia { get; set; }
        public string ComplementoEndereco { get; set; }
        public Guid? IdEmpresa { get; set; }
        public Guid? IdMunicipioEndereco { get; set; }
        public Guid? IdSindicato { get; set; }
        public Guid? IdLotacaoTributaria { get; set; }
        public short? IndicadorSubstituicaoPatronalObra { get; set; }
        public string NumeroProcessoPCD { get; set; }
        public string NumeroProcessoAprendiz { get; set; }
        public short? TipoCAEPessoaFisica { get; set; }

        // Navegação
        public virtual Empresa Empresa { get; set; }
        public virtual Municipio MunicipioEndereco { get; set; }
        public virtual Sindicato Sindicato { get; set; }
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Filial()
        {
            Funcionarios = new HashSet<Funcionario>();
            FlagAdicionalNoturno = 0;
            FlagLimiteTroca = 0;
            FlagMinimoHE = 0;
            PercentualConvenio = 0;
            PercentualSAT = 0;
            PercentualTerceiros = 0;
            PercentualEmpresa = 0;
        }
    }
}