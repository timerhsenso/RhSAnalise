// src/Modules/GestaoDePessoas/Core/Entities/Municipio.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal
{
    public class Municipio 
    {
        public Guid Id { get; set; }
        public string CodigoMunicipio { get; set; }
        public string SiglaEstado { get; set; }
        public string NomeMunicipio { get; set; }
        public int? CodigoIBGE { get; set; }

        // Navegação - múltiplas coleções para diferentes relacionamentos
        public virtual ICollection<Funcionario> FuncionariosNaturalidade { get; set; }
        public virtual ICollection<Funcionario> FuncionariosEndereco { get; set; }
        public virtual ICollection<Filial> Filiais { get; set; }

        public Municipio()
        {
            FuncionariosNaturalidade = new HashSet<Funcionario>();
            FuncionariosEndereco = new HashSet<Funcionario>();
            Filiais = new HashSet<Filial>();
        }
    }
}