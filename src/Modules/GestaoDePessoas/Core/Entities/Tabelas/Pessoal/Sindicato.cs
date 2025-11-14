// src/Modules/GestaoDePessoas/Core/Entities/Tabelas/Pessoal/Sindicato.cs

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

/// <summary>
/// Entidade de sindicato (patronal ou de empregados).
/// </summary>
public class Sindicato
{
    public Guid Id { get; set; }
    public string CodigoSindicato { get; set; } = string.Empty;
    public string DescricaoSindicato { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? CNPJ { get; set; }
    public string? CodigoEntidade { get; set; }
    public string? DataBase { get; set; }
    public int? FlagTipo { get; set; }
    public string? CodigoTabelaBase { get; set; }

    // Navegação
    public virtual ICollection<Funcionario> Funcionarios { get; set; }
    public virtual ICollection<Filial> Filiais { get; set; }

    public Sindicato()
    {
        Funcionarios = new HashSet<Funcionario>();
        Filiais = new HashSet<Filial>();
    }
}