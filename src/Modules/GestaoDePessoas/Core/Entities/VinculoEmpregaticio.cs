// src/Modules/GestaoDePessoas/Core/Entities/VinculoEmpregaticio.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class VinculoEmpregaticio : BaseEntity
    {
        public string CodigoVinculo { get; set; }
        public string DescricaoVinculo { get; set; }
        public string CodigoSEFIP { get; set; }
        public string CodigoClasse { get; set; }
        public int FlagRAIS { get; set; }
        public int NaturezaAtividade { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public VinculoEmpregaticio()
        {
            Funcionarios = new HashSet<Funcionario>();
            FlagRAIS = 0;
            NaturezaAtividade = 0;
        }
    }
}