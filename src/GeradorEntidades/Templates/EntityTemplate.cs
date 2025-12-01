// =============================================================================
// GERADOR FULL-STACK v3.0 - ENTITY TEMPLATE
// Gera a entidade de domínio com atributos para Source Generator
// Compatível com o formato do gerador antigo
// =============================================================================

using GeradorEntidades.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera a entidade C# para o Domain com Navigation Properties.
/// </summary>
public static class EntityTemplate
{
    // Prefixos conhecidos para melhor conversão PascalCase
    private static readonly string[] PrefixosConhecidos = new[]
    {
        "cd", "dc", "dt", "nr", "nm", "fl", "vl", "qt", "sg", "no", "id", "tp", "st", "ds", "tx", "pc", "hr"
    };

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

        // Criar HashSet de colunas definidas como PK pelo usuário
        var pkDefinidas = request.ColunasPkDefinidas?
            .Select(p => p.Nome.ToLowerInvariant())
            .ToHashSet() ?? new HashSet<string>();

        // Calcular total de PKs (do banco + definidas pelo usuário)
        var totalPks = tabela.PrimaryKeyColumns.Count + pkDefinidas.Count;
        var isPkComposta = totalPks > 1;

        // Criar ordem das PKs para Column(Order = X)
        var pkOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var orderIndex = 0;
        foreach (var pk in tabela.PrimaryKeyColumns)
        {
            pkOrder[pk.Nome] = orderIndex++;
        }
        foreach (var pkDef in request.ColunasPkDefinidas ?? new List<PkColumnConfig>())
        {
            if (!pkOrder.ContainsKey(pkDef.Nome))
            {
                pkOrder[pkDef.Nome] = orderIndex++;
            }
        }

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
            sb.AppendLine($"/// {EscapeXml(tabela.Descricao)}");
            sb.AppendLine("/// </summary>");
        }

        // Atributo GenerateCrud
        sb.AppendLine("[GenerateCrud(");
        sb.AppendLine($"    TableName = \"{tabela.NomeTabela.ToLower()}\",");
        sb.AppendLine($"    DisplayName = \"{EscapeString(request.DisplayName ?? FormatDisplayName(nomeEntidade))}\",");
        sb.AppendLine($"    CdSistema = \"{request.CdSistema}\",");
        sb.AppendLine($"    CdFuncao = \"{request.CdFuncao}\",");
        sb.AppendLine($"    IsLegacyTable = {request.IsLegacyTable.ToString().ToLower()},");
        sb.AppendLine($"    GenerateApiController = {request.GerarApiController.ToString().ToLower()}");
        sb.AppendLine(")]");

        // Classe
        sb.AppendLine($"public class {nomeEntidade}");
        sb.AppendLine("{");

        // =========================================================================
        // PROPRIEDADES ESCALARES
        // =========================================================================
        foreach (var coluna in tabela.Colunas)
        {
            var isPkDefinidaUsuario = pkDefinidas.Contains(coluna.Nome.ToLowerInvariant());
            var isPrimaryKey = coluna.IsPrimaryKey || isPkDefinidaUsuario;
            var order = pkOrder.GetValueOrDefault(coluna.Nome, -1);

            GerarPropriedade(sb, coluna, isPrimaryKey, isPkComposta, order, isPkDefinidaUsuario);
        }

        // =========================================================================
        // NAVIGATION PROPERTIES
        // =========================================================================
        if (request.GerarNavigation && tabela.ForeignKeys.Count > 0)
        {
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
                var colunaPascal = FormatPascalCase(fk.ColunaOrigem);
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

    #region Geração de Propriedade

    private static void GerarPropriedade(
        StringBuilder sb,
        ColunaInfo coluna,
        bool isPrimaryKey,
        bool isPkComposta,
        int pkOrder,
        bool isPkDefinidaUsuario)
    {
        var nomePascal = FormatPascalCase(coluna.Nome);
        var displayName = FormatDisplayName(nomePascal);

        // Comentário XML (se tiver descrição)
        if (!string.IsNullOrWhiteSpace(coluna.Descricao))
        {
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {EscapeXml(coluna.Descricao)}");
            sb.AppendLine($"    /// </summary>");
        }

        // Comentário se PK foi definida pelo usuário
        if (isPkDefinidaUsuario)
        {
            sb.AppendLine("    // PK definida manualmente (não existe constraint no banco)");
        }

        // ===== ATRIBUTOS =====

        // [Key] - para PKs
        if (isPrimaryKey)
        {
            sb.AppendLine("    [Key]");
        }

        // [Required] - para campos não nullable (incluindo PKs string)
        if (!coluna.IsNullable)
        {
            // Para strings, sempre [Required]
            // Para tipos valor, só se não for PK (PK já é implicitamente required)
            if (coluna.IsTexto || coluna.IsBinary)
            {
                sb.AppendLine("    [Required]");
            }
        }

        // [Column("nome")] ou [Column("nome", Order = X)] para PK composta
        var nomeColunaDiferente = !coluna.Nome.Equals(nomePascal, StringComparison.OrdinalIgnoreCase);
        if (isPkComposta && isPrimaryKey)
        {
            sb.AppendLine($"    [Column(\"{coluna.Nome.ToLower()}\", Order = {pkOrder})]");
        }
        else if (nomeColunaDiferente)
        {
            sb.AppendLine($"    [Column(\"{coluna.Nome.ToLower()}\")]");
        }

        // [DatabaseGenerated] para Identity
        if (coluna.IsIdentity)
        {
            sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
        }

        // [StringLength(n)] para strings com tamanho definido
        if (coluna.IsTexto && coluna.Tamanho.HasValue && coluna.Tamanho > 0)
        {
            if (coluna.Tamanho <= 8000) // SQL Server max para varchar/nvarchar
            {
                sb.AppendLine($"    [StringLength({coluna.Tamanho})]");
            }
        }

        // ===== PROPRIEDADE =====
        var tipoCSharp = coluna.TipoCSharp;
        var defaultValue = GetDefaultValue(tipoCSharp);

        sb.AppendLine($"    public {tipoCSharp} {nomePascal} {{ get; set; }}{defaultValue}");
        sb.AppendLine();
    }

    #endregion

    #region Formatação PascalCase

    /// <summary>
    /// Converte nome de coluna para PascalCase respeitando prefixos conhecidos.
    /// Ex: cdtptabela → CdTpTabela, dctabela → DcTabela
    /// </summary>
    public static string FormatPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Se já tem letras maiúsculas misturadas, provavelmente já está em PascalCase
        if (HasMixedCase(input))
        {
            return char.ToUpper(input[0]) + input[1..];
        }

        // Se tem underscore, processa como snake_case
        if (input.Contains('_'))
        {
            return ProcessSnakeCase(input);
        }

        // Converter para minúsculas para processar
        var lower = input.ToLowerInvariant();
        var result = new StringBuilder();
        var i = 0;

        while (i < lower.Length)
        {
            // Verificar se começa com um prefixo conhecido
            var prefixoEncontrado = false;
            foreach (var prefixo in PrefixosConhecidos)
            {
                if (i + prefixo.Length <= lower.Length &&
                    lower.Substring(i, prefixo.Length) == prefixo)
                {
                    // Adiciona prefixo com primeira letra maiúscula
                    result.Append(char.ToUpper(prefixo[0]));
                    result.Append(prefixo[1..]);
                    i += prefixo.Length;
                    prefixoEncontrado = true;
                    break;
                }
            }

            if (!prefixoEncontrado)
            {
                // Se não é prefixo, capitaliza a primeira letra do "restante"
                if (result.Length == 0 || i == 0)
                {
                    result.Append(char.ToUpper(lower[i]));
                }
                else
                {
                    result.Append(lower[i]);
                }
                i++;
            }
        }

        return result.ToString();
    }

    private static bool HasMixedCase(string input)
    {
        bool hasUpper = false;
        bool hasLower = false;
        foreach (var c in input)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (char.IsLower(c)) hasLower = true;
            if (hasUpper && hasLower) return true;
        }
        return false;
    }

    private static string ProcessSnakeCase(string input)
    {
        var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0) continue;

            // Primeiro tenta aplicar prefixos conhecidos
            var processed = FormatPascalCasePart(part.ToLowerInvariant());
            sb.Append(processed);
        }

        return sb.ToString();
    }

    private static string FormatPascalCasePart(string part)
    {
        if (string.IsNullOrEmpty(part)) return part;

        var result = new StringBuilder();
        var i = 0;

        while (i < part.Length)
        {
            var prefixoEncontrado = false;
            foreach (var prefixo in PrefixosConhecidos)
            {
                if (i + prefixo.Length <= part.Length &&
                    part.Substring(i, prefixo.Length) == prefixo)
                {
                    result.Append(char.ToUpper(prefixo[0]));
                    result.Append(prefixo[1..]);
                    i += prefixo.Length;
                    prefixoEncontrado = true;
                    break;
                }
            }

            if (!prefixoEncontrado)
            {
                result.Append(result.Length == 0 ? char.ToUpper(part[i]) : part[i]);
                i++;
            }
        }

        return result.ToString();
    }

    #endregion

    #region Display Name

    /// <summary>
    /// Formata nome PascalCase para display amigável.
    /// Ex: CdTpTabela → Código do Tipo de Tabela
    /// </summary>
    public static string FormatDisplayName(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase)) return pascalCase;

        // Primeiro separa as palavras por maiúsculas
        var comEspacos = Regex.Replace(pascalCase, "([a-z])([A-Z])", "$1 $2");

        // Substitui prefixos conhecidos por nomes completos
        var resultado = comEspacos
            .Replace("Cd ", "Código ")
            .Replace("Dc ", "Descrição ")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "Nome ")
            .Replace("Fl ", "Flag ")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Sg ", "Sigla ")
            .Replace("No ", "Número ")
            .Replace("Tp ", "Tipo ")
            .Replace("St ", "Status ")
            .Replace("Ds ", "Descrição ")
            .Replace("Tx ", "Taxa ")
            .Replace("Pc ", "Percentual ")
            .Replace("Hr ", "Hora ")
            .Replace("Id ", "");

        // Remove "de de" ou "do do" duplicados
        resultado = Regex.Replace(resultado, @"\b(de|do|da)\s+\1\b", "$1", RegexOptions.IgnoreCase);

        // Adiciona "de" ou "do" entre palavras quando apropriado
        resultado = AjustarPreposicoes(resultado);

        return resultado.Trim();
    }

    private static string AjustarPreposicoes(string texto)
    {
        // Palavras que pedem "da/do" antes
        var palavrasFemininas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Tabela", "Data", "Empresa", "Filial", "Pessoa", "Função", "Descrição",
            "Quantidade", "Taxa", "Hora", "Sigla", "Situação", "Ação", "Operação"
        };

        var palavrasMasculinas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Tipo", "Código", "Número", "Valor", "Status", "Nome", "Registro",
            "Sistema", "Módulo", "Cargo", "Setor", "Departamento", "Usuário"
        };

        var palavras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        var resultado = new List<string>();

        for (int i = 0; i < palavras.Count; i++)
        {
            var atual = palavras[i];
            resultado.Add(atual);

            // Se não é a última palavra e a próxima precisa de preposição
            if (i < palavras.Count - 1)
            {
                var proxima = palavras[i + 1];

                // Não adiciona preposição se já tem uma
                if (proxima.Equals("de", StringComparison.OrdinalIgnoreCase) ||
                    proxima.Equals("do", StringComparison.OrdinalIgnoreCase) ||
                    proxima.Equals("da", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Verifica se a próxima palavra pede preposição
                if (palavrasFemininas.Contains(proxima) &&
                    !atual.Equals("de", StringComparison.OrdinalIgnoreCase) &&
                    !atual.Equals("da", StringComparison.OrdinalIgnoreCase))
                {
                    resultado.Add("da");
                }
                else if (palavrasMasculinas.Contains(proxima) &&
                         !atual.Equals("de", StringComparison.OrdinalIgnoreCase) &&
                         !atual.Equals("do", StringComparison.OrdinalIgnoreCase))
                {
                    resultado.Add("do");
                }
            }
        }

        return string.Join(" ", resultado);
    }

    #endregion

    #region Helpers

    private static string GetDefaultValue(string tipoCSharp)
    {
        return tipoCSharp switch
        {
            "string" => " = string.Empty;",
            "byte[]" => " = Array.Empty<byte>();",
            _ => ""
        };
    }

    private static string EscapeXml(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    private static string EscapeString(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Replace("\"", "\\\"");
    }

    #endregion

    #region FK Filtering

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