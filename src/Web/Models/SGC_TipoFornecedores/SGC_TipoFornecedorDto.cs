// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoFornecedor
// Data: 2025-12-02 15:55:54
// =============================================================================

namespace RhSensoERP.Web.Models.SGC_TipoFornecedores;

/// <summary>
/// DTO de leitura para Tipo Fornecedor.
/// Compatível com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.SGC_TipoFornecedores.SGC_TipoFornecedorDto
/// </summary>
public class SGC_TipoFornecedorDto
{
    /// <summary>
    /// Tipo Fornecedor
    /// </summary>
    public int IdTipoFornecedor { get; set; }

    /// <summary>
    /// Saas
    /// </summary>
    public Guid? IdSaas { get; set; }

    /// <summary>
    /// Codigo
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Icone
    /// </summary>
    public string Icone { get; set; } = string.Empty;

    /// <summary>
    /// Cor Hex
    /// </summary>
    public string CorHex { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    public int Ordem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Cadastro
    /// </summary>
    public DateTime DataCadastro { get; set; }

    /// <summary>
    /// Data Atualizacao
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Usuario Cadastro
    /// </summary>
    public Guid? IdUsuarioCadastro { get; set; }

    /// <summary>
    /// Usuario Atualizacao
    /// </summary>
    public Guid? IdUsuarioAtualizacao { get; set; }
}
