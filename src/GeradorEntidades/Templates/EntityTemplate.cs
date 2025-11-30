// =============================================================================
// GERADOR FULL-STACK v3.0 - ENTITY TEMPLATE
// Gera a entidade de domínio com atributos para Source Generator
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera a entidade C# para o Domain com Navigation Properties.
/// </summary>
public static class EntityTemplate
{
    /// <summary>
    /// Gera a entidade completa.
    /// </summary>
    public static GeneratedFile Generate(
        TabelaInfo tabela, 
        FullStackRequest request,
        List<string> navigationsGeradas)
    {
        var nomeEntidade = tabela.NomePascalCase;
        var modulo = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.Nome.Equals(request.Modulo, StringComparison.OrdinalIgnoreCase))
            ?? ModuloConfig.GetModulos().First();

        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        sb.AppendLine("using RhSensoERP.Shared.Core.Attributes;");
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {modulo.Namespace}.Core.Entities;");
        sb.AppendLine();

        // Documentação XML
        if (!string.IsNullOrWhiteSpace(tabela.Descricao))
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {tabela.Descricao}");
            sb.AppendLine("/// </summary>");
        }

        // Atributo GenerateCrud
        sb.AppendLine("[GenerateCrud(");
        sb.AppendLine($"    TableName = \"{tabela.NomeTabela.ToLower()}\",");
        sb.AppendLine($"    DisplayName = \"{request.DisplayName ?? nomeEntidade}\",");
        sb.AppendLine($"    CdSistema = \"{request.CdSistema}\",");
        sb.AppendLine($"    CdFuncao = \"{request.CdFuncao}\",");
        sb.AppendLine($"    IsLegacyTable = {request.IsLegacyTable.ToString().ToLower()},");
        sb.AppendLine($"    GenerateApiController = {request.GerarApiController.ToString().ToLower()}");
        sb.AppendLine(")]");

        // Classe
        sb.AppendLine($"public class {nomeEntidade}");
        sb.AppendLine("{");

        // =========================================================================
        // SEÇÃO 1: PROPRIEDADES ESCALARES (colunas normais)
        // =========================================================================
        sb.AppendLine("    #region Propriedades");
        sb.AppendLine();

        foreach (var coluna in tabela.Colunas)
        {
            GerarPropriedade(sb, coluna, tabela.PrimaryKey);
        }

        sb.AppendLine("    #endregion");

        // =========================================================================
        // SEÇÃO 2: NAVIGATION PROPERTIES (relacionamentos)
        // =========================================================================
        if (request.GerarNavigation && tabela.ForeignKeys.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("    #region Navigation Properties");
            sb.AppendLine();

            var fksFiltradas = FiltrarFksParaNavigation(tabela, request);
            var navigationNamesUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var tabelaParaNavigation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var fk in fksFiltradas)
            {
                var navigationName = GerarNomeNavigation(fk, navigationNamesUsados, tabelaParaNavigation);
                navigationNamesUsados.Add(navigationName);

                if (!tabelaParaNavigation.ContainsKey(fk.TabelaDestino))
                    tabelaParaNavigation[fk.TabelaDestino] = navigationName;

                var coluna = tabela.Colunas.FirstOrDefault(c =>
                    c.Nome.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));

                var isNullable = coluna?.IsNullable ?? true;
                var nullableMarker = isNullable ? "?" : "";

                // Comentário XML
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// Navegação para {fk.EntidadeDestino} via {fk.ColunaOrigem}");
                sb.AppendLine($"    /// </summary>");

                // Atributo ForeignKey
                var colunaPascal = coluna?.NomePascalCase ?? TabelaInfo.ToPascalCase(fk.ColunaOrigem);
                sb.AppendLine($"    [ForeignKey(nameof({colunaPascal}))]");

                // Propriedade virtual
                sb.AppendLine($"    public virtual {fk.EntidadeDestino}{nullableMarker} {navigationName} {{ get; set; }}");
                sb.AppendLine();

                navigationsGeradas.Add($"{navigationName} → {fk.EntidadeDestino}");
            }

            sb.AppendLine("    #endregion");
        }

        sb.AppendLine("}");

        return new GeneratedFile
        {
            FileName = $"{nomeEntidade}.cs",
            RelativePath = $"Domain/Entities/{nomeEntidade}.cs",
            Content = sb.ToString(),
            FileType = "Entity"
        };
    }

    #region Helper Methods

    private static void GerarPropriedade(StringBuilder sb, ColunaInfo coluna, ColunaInfo? primaryKey)
    {
        // Comentário XML
        if (!string.IsNullOrWhiteSpace(coluna.Descricao))
        {
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {coluna.Descricao}");
            sb.AppendLine($"    /// </summary>");
        }

        // Atributos
        if (coluna.IsPrimaryKey)
        {
            sb.AppendLine("    [Key]");
        }

        if (coluna.IsIdentity)
        {
            sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
        }
        else if (coluna.IsPrimaryKey && coluna.IsGuid)
        {
            // Guid PK sem Identity - será gerado pelo código
        }

        // Column attribute para mapear nome original
        if (!coluna.Nome.Equals(coluna.NomePascalCase, StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine($"    [Column(\"{coluna.Nome}\")]");
        }

        // MaxLength para strings
        if (coluna.IsTexto && coluna.Tamanho.HasValue && coluna.Tamanho > 0 && coluna.Tamanho < 10000)
        {
            sb.AppendLine($"    [MaxLength({coluna.Tamanho})]");
        }

        // Required para não-nullable (exceto PK e tipos valor)
        if (!coluna.IsNullable && !coluna.IsPrimaryKey && coluna.IsTexto)
        {
            sb.AppendLine("    [Required]");
        }

        // Propriedade
        var defaultValue = coluna.TipoCSharp switch
        {
            "string" => " = string.Empty;",
            "byte[]" => " = Array.Empty<byte>();",
            _ => ""
        };

        sb.AppendLine($"    public {coluna.TipoCSharp} {coluna.NomePascalCase} {{ get; set; }}{defaultValue}");
        sb.AppendLine();
    }

    private static List<ForeignKeyInfo> FiltrarFksParaNavigation(TabelaInfo tabela, FullStackRequest request)
    {
        // PASSO 1: Elimina FKs duplicadas
        var fksUnicas = tabela.ForeignKeys
            .GroupBy(fk => fk.ChaveUnica)
            .Select(g => g.First())
            .ToList();

        // PASSO 2: Ignora FKs compostas
        var fksSimplesOuPrimeiraDaComposta = fksUnicas
            .Where(fk => !fk.IsParteDeFkComposta)
            .ToList();

        // PASSO 3: Garante uma navegação por coluna
        var colunasJaProcessadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var fksFiltradas = new List<ForeignKeyInfo>();

        foreach (var fk in fksSimplesOuPrimeiraDaComposta)
        {
            if (!colunasJaProcessadas.Contains(fk.ColunaOrigem))
            {
                fksFiltradas.Add(fk);
                colunasJaProcessadas.Add(fk.ColunaOrigem);
            }
        }

        // PASSO 4: Filtra por tipo se solicitado
        if (request.ApenasNavigationPorGuid)
        {
            fksFiltradas = fksFiltradas.Where(fk => fk.IsFkByGuid).ToList();
        }

        // Aplica configurações customizadas de FK
        if (request.ConfiguracoesFk.Count > 0)
        {
            var configsIgnorar = request.ConfiguracoesFk
                .Where(c => c.Ignorar)
                .Select(c => c.ColunaOrigem.ToLower())
                .ToHashSet();

            fksFiltradas = fksFiltradas
                .Where(fk => !configsIgnorar.Contains(fk.ColunaOrigem.ToLower()))
                .ToList();
        }

        // PASSO 5: Ordenar FKs por Guid primeiro
        return fksFiltradas
            .OrderByDescending(fk => fk.IsFkByGuid)
            .ThenBy(fk => fk.ColunaOrigem)
            .ToList();
    }

    private static string GerarNomeNavigation(
        ForeignKeyInfo fk,
        HashSet<string> usados,
        Dictionary<string, string> tabelaParaNav)
    {
        // Começa com o nome sugerido pela FK
        var nomeBase = fk.NavigationPropertyName;

        // Se a tabela destino já tem navegação, adiciona sufixo
        if (tabelaParaNav.TryGetValue(fk.TabelaDestino, out var existente))
        {
            // Segunda navegação para mesma tabela: adiciona contexto
            if (fk.IsFkByCodigo)
                nomeBase = $"{nomeBase}PorCodigo";
            else
                nomeBase = $"{nomeBase}2";
        }

        // Garante unicidade
        var nomeFinal = nomeBase;
        var contador = 2;
        while (usados.Contains(nomeFinal))
        {
            nomeFinal = $"{nomeBase}{contador++}";
        }

        return nomeFinal;
    }

    #endregion
}
