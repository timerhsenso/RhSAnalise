// src/Modules/GestaoDePessoas/Core/Entities/MotivoRescisao.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class MotivoRescisao : BaseEntity
    {
        public string CodigoRescisao { get; set; }
        public string DescricaoRescisao { get; set; }
        public string CodigoFGTS { get; set; }
        public string ComAviso { get; set; }
        public string CodigoRAIS { get; set; }
        public string DescricaoReduzida { get; set; }
        public string CodigoSaque { get; set; }
        public string CodigoCAGED { get; set; }
        public string CodigoSEFIP { get; set; }
        public string ComRecebimentoGRRF { get; set; }
        public string CodigoAfastamentoRCT { get; set; }
        public string CodigoESocial { get; set; }
        public string FlagTerminoContrato { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public MotivoRescisao()
        {
            Funcionarios = new HashSet<Funcionario>();
        }
    }
}