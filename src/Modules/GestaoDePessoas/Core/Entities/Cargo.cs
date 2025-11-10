// src/Modules/GestaoDePessoas/Core/Entities/Cargo.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Cargo : BaseEntity
    {
        public string CodigoCargo { get; set; }
        public string DescricaoCargo { get; set; }
        public string CodigoInstrucao { get; set; }
        public string CodigoCBO { get; set; }
        public string CodigoTabela { get; set; }
        public string CodigoNivelInicial { get; set; }
        public string CodigoNivelFinal { get; set; }
        public int FlagAtivo { get; set; }
        public string CodigoGrupoProfissional { get; set; }
        public string CodigoCBO6 { get; set; }
        public DateTime? DataInicioValidade { get; set; }
        public DateTime? DataFimValidade { get; set; }
        public int Tenant { get; set; }
        public Guid? IdCBO { get; set; }
        public Guid? IdGrauInstrucao { get; set; }
        public Guid? IdTabelaSalarial { get; set; }

        // Navegação
        public virtual GrauInstrucao GrauInstrucao { get; set; }
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Cargo()
        {
            Funcionarios = new HashSet<Funcionario>();
            FlagAtivo = 1;
            Tenant = 0;
        }
    }
}