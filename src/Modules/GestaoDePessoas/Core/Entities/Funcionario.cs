// src/RhSensoERP.Domain/Entities/RHU/Funcionario.cs

using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Core.Primitives;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;
using RhSensoERP.Shared.Core.Primitives;
using System;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Funcionario : BaseEntity
    {
        // Chave Primária Composta
        public string Matricula { get; set; } // nomatric
        public int CodigoEmpresa { get; set; } // cdempresa
        public int CodigoFilial { get; set; } // cdfilial

        // Dados Básicos
        public string NomeColaborador { get; set; } // nmcolab
        public string NomeGuerra { get; set; } // nmguerra
        public string CodigoCentroCusto { get; set; } // cdccusto
        public int? TipoColaborador { get; set; } // tpcolab
        public string CodigoPessoa { get; set; } // nopessoa
        public string CodigoColaborador { get; set; } // cdcolab
        public string MatriculaSAP { get; set; } // matsap

        // Datas Contratuais
        public DateTime DataAdmissao { get; set; } // dtadmissao
        public DateTime? DataDemissao { get; set; } // dtdemissao
        public DateTime? DataAviso { get; set; } // dtaviso
        public DateTime? DataTransferencia { get; set; } // dttransf
        public DateTime? DataVencimentoContrato2 { get; set; } // dtvenccontr2
        public DateTime? DataHomologacao { get; set; } // dthomologacao
        public DateTime? DataLimiteAcesso { get; set; } // dtlimite_acesso
        public DateTime? DataAdmissaoHistorica { get; set; } // dtadmhis

        // Dados Bancários - Recebimento
        public string CodigoBancoRecebimento { get; set; } // cdbanrec
        public string CodigoAgenciaRecebimento { get; set; } // cdagerec
        public string NumeroContaRecebimento { get; set; } // noctarec
        public string DigitoVerificadorAgenciaRecebimento { get; set; } // dvagerec
        public string DigitoVerificadorContaRecebimento { get; set; } // dvctarec
        public string TipoContaRecebimento { get; set; } // tpctarec

        // Dados Bancários - FGTS
        public string CodigoBancoFGTS { get; set; } // cdbanfgts
        public string CodigoAgenciaFGTS { get; set; } // cdagefgts
        public string NumeroContaFGTS { get; set; } // noctafgts
        public string CodigoOpcaoFGTS { get; set; } // cdopcfgts
        public string DigitoVerificadorAgenciaFGTS { get; set; } // dvagefgts
        public DateTime? DataOpcaoFGTS { get; set; } // dtopcfgts
        public double? SaldoFGTS { get; set; } // vlsaldofgts
        public int? RecebeuFGTS { get; set; } // flrecfgts

        // Endereço
        public string Endereco { get; set; } // dcendereco
        public string NumeroEndereco { get; set; } // end_numero
        public string ComplementoEndereco { get; set; } // end_comp
        public string Bairro { get; set; } // dcbairro
        public string CodigoMunicipio { get; set; } // cdmunicip
        public string SiglaEstado { get; set; } // sgestado
        public string CEP { get; set; } // nocep
        public string PontoReferencia { get; set; } // dcptreferencia

        // Contatos
        public string Telefone { get; set; } // notelefone
        public string DDD { get; set; } // noddd
        public string Telefone2 { get; set; } // notelefone2
        public string DDD2 { get; set; } // noddd2
        public string DDD3 { get; set; } // noddd3
        public string Telefone3 { get; set; } // notelefone3
        public string Contato2 { get; set; } // contato2
        public string Contato3 { get; set; } // contato3
        public string Email { get; set; } // dcemail
        public string EmailAlternativo { get; set; } // emailalternativo
        public string Ramal { get; set; } // dcramal
        public string RamalTransporte { get; set; } // cdramal_transp

        // Dados Pessoais
        public string EstadoCivil { get; set; } // cdestcivil
        public DateTime? DataNascimento { get; set; } // dtnasc
        public string Sexo { get; set; } // cdsexo
        public string NomePai { get; set; } // nmpaicolab
        public string NomeMae { get; set; } // nmmaecolab
        public string TipoSanguineo { get; set; } // cdtpsangue
        public int? CodigoRaca { get; set; } // cod_raca
        public string CodigoDeficiente { get; set; } // cod_deficiente

        // Documentos Pessoais
        public string CPF { get; set; } // nocpf
        public string PIS { get; set; } // nopis
        public DateTime? DataInscricaoPIS { get; set; } // dtinscpis
        public string RG { get; set; } // norg
        public string OrgaoEmissorRG { get; set; } // sgorgrg
        public string EstadoRG { get; set; } // sgestrg
        public DateTime? DataRG { get; set; } // dtrg
        public DateTime? DataValidadeRGEstrangeiro { get; set; } // dtvrgext
        public string CartaoSUS { get; set; } // nocartaosus

        // Documentos Profissionais
        public string CodigoCargo { get; set; } // cdcargo
        public string NumeroCarteiraTrabalho { get; set; } // nocartprof
        public string SerieCarteiraTrabalho { get; set; } // noserie
        public string EstadoCarteiraTrabalho { get; set; } // sgestcart
        public DateTime? DataCarteiraTrabalho { get; set; } // dtcartprof

        // Título Eleitoral
        public string NumeroTituloEleitoral { get; set; } // notitelei
        public string SecaoTituloEleitoral { get; set; } // nosecaotit
        public string ZonaTituloEleitoral { get; set; } // nozonatit
        public string EstadoTituloEleitoral { get; set; } // sgesttit
        public DateTime? DataTituloEleitoral { get; set; } // dttitelei

        // Dados Trabalhistas
        public string CodigoCategoria { get; set; } // cdcategori
        public string CodigoCausaRescisao { get; set; } // cdcausres
        public int? HorasSemanais { get; set; } // nohssem
        public int? HorasMensais { get; set; } // nohsmes
        public string CodigoInstrucao { get; set; } // cdinstruc
        public string CodigoNacionalidade { get; set; } // cdnacion
        public string AnoChegadaPais { get; set; } // aachegpais
        public string CodigoSindicato { get; set; } // cdsindicat
        public int? FlagSindicato { get; set; } // flsindicat
        public string FlagContribuicaoSindical { get; set; } // flcontsin
        public string CodigoSituacao { get; set; } // cdsituacao
        public string CodigoVinculo { get; set; } // cdvincul
        public string CodigoNivel { get; set; } // cdnivel
        public int? FlagFrequencia { get; set; } // flfreq
        public string CodigoEmpregado { get; set; } // cdempreg
        public string CodigoTurma { get; set; } // cdturma
        public int? CodigoCarreira { get; set; } // cdcarreira

        // Local Nascimento
        public string MunicipioNascimento { get; set; } // dcmuninasc
        public string CodigoMunicipioNascimento { get; set; } // cdmuninasc
        public string EstadoNascimento { get; set; } // sgestadonasc

        // Dados Estrangeiro
        public string TipoVisto { get; set; } // cdtpvisto
        public DateTime? DataValidadeCapacidadeExtrangeiro { get; set; } // dtvcapfext

        // Controle
        public string MatriculaAnterior { get; set; } // nomatant
        public string CodigoOcorrencia { get; set; } // cdocorr
        public DateTime? DataUltimaExperiencia { get; set; } // dtultexper
        public DateTime? DataUltimaMovimentacao { get; set; } // dtultmov
        public string CodigoUsuario { get; set; } // cdusuario
        public int? FlagContratoExperiencia { get; set; } // flcexperie
        public DateTime? DataContratoExperiencia { get; set; } // dtcexperie
        public int? FlagPrevidenciaComplementar { get; set; } // flprevcomp
        public int? CodigoGrupoPPP { get; set; } // cdgrupo_ppp

        // IDs das Foreign Keys
        public Guid? IdCentroCusto { get; set; } // idcentrodecusto
        public Guid? IdCargo { get; set; } // idcargo
        public Guid? IdMunicipioNaturalidade { get; set; } // idmunicipionaturalidade
        public Guid? IdGrauInstrucao { get; set; } // idgraudeinstrucao
        public Guid? IdSituacao { get; set; } // idsituacao
        public Guid? IdMunicipioEndereco { get; set; } // idmunicipioendereco
        public Guid? IdVinculoEmpregaticio { get; set; } // idvinculoempregaticio
        public Guid? IdSindicato { get; set; } // idsindicato
        public Guid? IdBancoRecebimento { get; set; } // idbancorecebimento
        public Guid? IdAgenciaRecebimento { get; set; } // idagenciarecebimento
        public Guid? IdFilial { get; set; } // idfilial
        public Guid? IdMotivoRescisao { get; set; } // idmotivorescisao

        // Navegação - Relacionamentos
        public virtual Empresa Empresa { get; set; }
        public virtual Filial Filial { get; set; }
        public virtual CentroCusto CentroCusto { get; set; }
        public virtual Cargo Cargo { get; set; }
        public virtual Municipio MunicipioNaturalidade { get; set; }
        public virtual Municipio MunicipioEndereco { get; set; }
        public virtual GrauInstrucao GrauInstrucao { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual VinculoEmpregaticio VinculoEmpregaticio { get; set; }
        public virtual Sindicato Sindicato { get; set; }
        public virtual Banco BancoRecebimento { get; set; }
        public virtual Agencia AgenciaRecebimento { get; set; }
        public virtual MotivoRescisao MotivoRescisao { get; set; }
    }
}