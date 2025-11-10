// src/Modules/GestaoDePessoas/Core/Entities/Situacao.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Situacao : BaseEntity
    {
        public string CodigoSituacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public string FlagDemissao { get; set; }
        public string FlagAfastamento { get; set; }
        public int? QuantidadeDiasBeneficio { get; set; }
        public int? QuantidadeDiasPrevistos { get; set; }
        public string CodigoFGTS { get; set; }
        public string CodigoSEFIP { get; set; }
        public string CodigoSEFIP2 { get; set; }
        public string FlagPagamentoFerias { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Situacao()
        {
            Funcionarios = new HashSet<Funcionario>();
        }
    }
}