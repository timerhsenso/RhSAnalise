// src/Modules/GestaoDePessoas/Core/Entities/Banco.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Banco : BaseEntity
    {
        public string CodigoBanco { get; set; }
        public string DescricaoBanco { get; set; }

        // Navegação
        public virtual ICollection<Agencia> Agencias { get; set; }
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Banco()
        {
            Agencias = new HashSet<Agencia>();
            Funcionarios = new HashSet<Funcionario>();
        }
    }
}