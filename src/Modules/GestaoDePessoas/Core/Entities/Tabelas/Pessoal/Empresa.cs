// src/Modules/GestaoDePessoas/Core/Entities/Empresa.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal
{
    public class Empresa : BaseEntity
    {
        public int CodigoEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeFantasia { get; set; }
        public string TipoCheque { get; set; }
        public string TipoDARF { get; set; }
        public string TipoGRPS { get; set; }
        public string TipoRescisao { get; set; }
        public string BrowseFuncionario { get; set; }
        public string TotalOrcamento { get; set; }
        public string CalculoFerias { get; set; }
        public string ArquivoLogo { get; set; }
        public string ArquivoLogoCracha { get; set; }
        public int? FlagFolhaAposentadoriaSocial { get; set; }
        public string TipoInscricaoEmpregador { get; set; }
        public string NumeroInscricaoEmpregador { get; set; }
        public string FlagAtivo { get; set; }
        public byte[] Logo { get; set; }
        public byte[] LogoCracha { get; set; }
        public string ClassificacaoTributaria { get; set; }
        public string CNPJEmpresaFiscalResponsavel { get; set; }
        public DateTime? DataDOU { get; set; }
        public DateTime? DataEmissaoCertificado { get; set; }
        public DateTime? DataProtocoloRenovacao { get; set; }
        public DateTime? DataVencimentoCertificado { get; set; }
        public string IdentificadorMinimoLei { get; set; }
        public int? IndicadorAcordoIsencaoMulta { get; set; }
        public int? IndicadorConstrutora { get; set; }
        public int? IndicadorCooperativa { get; set; }
        public int? IndicadorDesoneracaoFolha { get; set; }
        public int? IndicadorOpcaoCCP { get; set; }
        public string IndicadorPorteEmpresa { get; set; }
        public int? IndicadorOptoRegistroEletronico { get; set; }
        public string NaturezaJuridica { get; set; }
        public string NumeroCertificado { get; set; }
        public string NumeroProtocoloRenovacao { get; set; }
        public string NumeroRegistroETT { get; set; }
        public string PaginaDOU { get; set; }
        public int Tenant { get; set; }

        // Navegação
        public virtual ICollection<Filial> Filiais { get; set; }
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Empresa()
        {
            Filiais = new HashSet<Filial>();
            Funcionarios = new HashSet<Funcionario>();
            Tenant = 0;
        }
    }
}