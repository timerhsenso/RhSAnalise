// src/Modules/GestaoDePessoas/Core/Entities/Agencia.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal
{
    public class Agencia : BaseEntity
    {
        public string CodigoBanco { get; set; }
        public string CodigoAgencia { get; set; }
        public string DigitoVerificador { get; set; }
        public string NomeAgencia { get; set; }
        public string CodigoMunicipio { get; set; }
        public string NumeroConta { get; set; }
        public Guid? IdBanco { get; set; }

        // Navegação
        public virtual Banco Banco { get; set; }
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public Agencia()
        {
            Funcionarios = new HashSet<Funcionario>();
        }
    }
}