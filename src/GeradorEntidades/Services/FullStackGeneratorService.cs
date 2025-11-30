// =============================================================================
// GERADOR FULL-STACK v3.0 - FULL STACK GENERATOR SERVICE
// Orquestra todos os templates para gerar código completo
// =============================================================================

using GeradorEntidades.Models;
using GeradorEntidades.Templates;

namespace GeradorEntidades.Services;

/// <summary>
/// Serviço que orquestra a geração de todos os arquivos do Full-Stack.
/// </summary>
public class FullStackGeneratorService
{
    private readonly ILogger<FullStackGeneratorService> _logger;

    public FullStackGeneratorService(ILogger<FullStackGeneratorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gera todos os arquivos do Full-Stack.
    /// </summary>
    public FullStackResult Generate(TabelaInfo tabela, FullStackRequest request)
    {
        var result = new FullStackResult
        {
            NomeTabela = tabela.NomeTabela,
            NomeEntidade = tabela.NomePascalCase
        };

        try
        {
            _logger.LogInformation("Iniciando geração Full-Stack para {Tabela}", tabela.NomeTabela);

            // Validações
            var validationErrors = ValidateRequest(tabela, request);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.Error = string.Join("; ", validationErrors);
                return result;
            }

            // Warnings
            result.Warnings = GetWarnings(tabela);

            // Preenche configurações default se não fornecidas
            EnsureDefaultConfigurations(tabela, request);

            // Cria EntityConfig a partir dos dados
            var entityConfig = EntityConfig.FromTabela(tabela, request);

            // =========================================================================
            // GERAÇÃO: BACKEND
            // =========================================================================

            if (request.GerarEntidade)
            {
                _logger.LogDebug("Gerando Entidade...");
                var navigations = new List<string>();
                result.Entidade = EntityTemplate.Generate(tabela, request, navigations);
                result.NavigationsGeradas = navigations;
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - CONTROLLERS
            // =========================================================================

            if (request.GerarWebController)
            {
                _logger.LogDebug("Gerando WebController...");
                result.WebController = WebControllerTemplate.Generate(entityConfig);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - MODELS
            // =========================================================================

            if (request.GerarWebModels)
            {
                _logger.LogDebug("Gerando WebModels...");
                result.Dto = WebModelsTemplate.GenerateDto(entityConfig);
                result.CreateRequest = WebModelsTemplate.GenerateCreateRequest(entityConfig);
                result.UpdateRequest = WebModelsTemplate.GenerateUpdateRequest(entityConfig);
                result.ListViewModel = WebModelsTemplate.GenerateListViewModel(entityConfig);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - SERVICES
            // =========================================================================

            if (request.GerarWebServices)
            {
                _logger.LogDebug("Gerando WebServices...");
                result.ServiceInterface = WebServicesTemplate.GenerateInterface(entityConfig);
                result.ServiceImplementation = WebServicesTemplate.GenerateImplementation(entityConfig);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - VIEW
            // =========================================================================

            if (request.GerarView)
            {
                _logger.LogDebug("Gerando View...");
                result.View = ViewTemplate.Generate(entityConfig);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - JAVASCRIPT
            // =========================================================================

            if (request.GerarJavaScript)
            {
                _logger.LogDebug("Gerando JavaScript...");
                result.JavaScript = JavaScriptTemplate.Generate(entityConfig);
            }

            result.Success = true;

            _logger.LogInformation(
                "Geração Full-Stack concluída para {Tabela}: {FileCount} arquivos gerados",
                tabela.NomeTabela,
                result.AllFiles.Count());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na geração Full-Stack para {Tabela}", tabela.NomeTabela);
            result.Success = false;
            result.Error = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Valida o request antes da geração.
    /// </summary>
    private List<string> ValidateRequest(TabelaInfo tabela, FullStackRequest request)
    {
        var errors = new List<string>();

        // Tabela sem PK - BLOQUEIA
        if (!tabela.HasPrimaryKey)
        {
            errors.Add($"Tabela '{tabela.NomeTabela}' não possui Primary Key definida. Geração bloqueada.");
        }

        // CdFuncao obrigatório
        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            errors.Add("Código da Função (CdFuncao) é obrigatório.");
        }

        return errors;
    }

    /// <summary>
    /// Gera warnings (não bloqueiam, mas informam).
    /// </summary>
    private List<string> GetWarnings(TabelaInfo tabela)
    {
        var warnings = new List<string>();

        // PK Composta
        if (tabela.HasCompositePrimaryKey)
        {
            var pkCols = string.Join(", ", tabela.PrimaryKeyColumns.Select(c => c.Nome));
            warnings.Add($"Tabela possui PK composta ({pkCols}). Algumas funcionalidades podem ter limitações.");
        }

        // FK Composta
        if (tabela.HasCompositeForeignKeys)
        {
            var fksCompostas = tabela.ForeignKeys
                .Where(fk => fk.IsParteDeFkComposta)
                .Select(fk => fk.Nome)
                .Distinct()
                .ToList();

            warnings.Add($"Tabela possui FK(s) composta(s) ({string.Join(", ", fksCompostas)}). Navegações não serão geradas para essas FKs.");
        }

        // Tabela sem descrição
        if (string.IsNullOrWhiteSpace(tabela.Descricao))
        {
            warnings.Add("Tabela não possui Extended Property 'MS_Description'. Considere documentar a tabela.");
        }

        return warnings;
    }

    /// <summary>
    /// Garante que configurações default sejam preenchidas.
    /// </summary>
    private void EnsureDefaultConfigurations(TabelaInfo tabela, FullStackRequest request)
    {
        // DisplayName
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            request.DisplayName = FormatDisplayName(tabela.NomePascalCase);
        }

        // Módulo
        var modulo = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.CdSistema.Equals(request.CdSistema, StringComparison.OrdinalIgnoreCase));

        if (modulo != null)
        {
            if (string.IsNullOrWhiteSpace(request.Modulo))
                request.Modulo = modulo.Nome;
            if (string.IsNullOrWhiteSpace(request.ModuloRota))
                request.ModuloRota = modulo.Rota;
        }

        // Colunas de Listagem - default se não configurado
        if (request.ColunasListagem.Count == 0)
        {
            var order = 0;
            foreach (var coluna in tabela.Colunas.Where(c => !c.IsPrimaryKey && !c.IsGuid))
            {
                request.ColunasListagem.Add(new ColumnListConfig
                {
                    Nome = coluna.Nome,
                    Visible = true,
                    Order = order++,
                    Title = FormatDisplayName(coluna.NomePascalCase),
                    Format = GetDefaultFormat(coluna),
                    Sortable = true
                });

                // Limita a 8 colunas por default
                if (order >= 8) break;
            }
        }

        // Colunas de Formulário - default se não configurado
        if (request.ColunasFormulario.Count == 0)
        {
            var order = 0;
            foreach (var coluna in tabela.Colunas.Where(c => !c.IsPrimaryKey && !c.IsComputed))
            {
                request.ColunasFormulario.Add(new ColumnFormConfig
                {
                    Nome = coluna.Nome,
                    Visible = true,
                    Order = order++,
                    Label = FormatDisplayName(coluna.NomePascalCase),
                    InputType = GetDefaultInputType(coluna),
                    ColSize = GetDefaultColSize(coluna),
                    Required = !coluna.IsNullable
                });
            }
        }
    }

    #region Helpers

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]))
            {
                sb.Append(' ');
            }

            sb.Append(c);
        }

        return sb.ToString()
            .Replace("Cd ", "Código ")
            .Replace("Dc ", "")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "")
            .Replace("Fl ", "")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Sg ", "Sigla ")
            .Replace("No ", "Número ")
            .Replace("Id ", "")
            .Trim();
    }

    private static string GetDefaultFormat(ColunaInfo coluna)
    {
        if (coluna.IsData) return "date";
        if (coluna.Tipo.ToLower() is "decimal" or "money" or "numeric") return "currency";
        if (coluna.IsBool) return "boolean";
        return "text";
    }

    private static string GetDefaultInputType(ColunaInfo coluna)
    {
        if (coluna.IsData) return coluna.Tipo.ToLower() == "date" ? "date" : "datetime-local";
        if (coluna.IsBool) return "checkbox";
        if (coluna.IsNumerico) return "number";
        if (coluna.IsTexto && coluna.Tamanho > 255) return "textarea";
        if (coluna.ForeignKey != null) return "select";
        return "text";
    }

    private static int GetDefaultColSize(ColunaInfo coluna)
    {
        if (coluna.IsBool) return 2;
        if (coluna.IsData) return 4;
        if (coluna.IsNumerico) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho < 20) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho > 255) return 12;
        return 6;
    }

    #endregion
}
