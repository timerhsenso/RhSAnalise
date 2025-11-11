// src/Modules/GestaoDePessoas/Core/Entities/Sindicato.cs

using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class Sindicato : BaseEntity
    {
        public string CodigoSindicato { get; set; }
        public string DescricaoSindicato { get; set; }
        public string Endereco { get; set; }
        public string CNPJ { get; set; }
        public string CodigoEntidade { get; set; }
        public string DataBase { get; set; }
        public int? FlagTipo { get; set; }
        public string CodigoTabelaBase { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }
        public virtual ICollection<Filial> Filiais { get; set; }

        public Sindicato()
        {
            Funcionarios = new HashSet<Funcionario>();
            Filiais = new HashSet<Filial>();
        }
    }
}