// =============================================================================
// GERADOR FULL-STACK v3.1 - VIEW TEMPLATE
// Baseado em RhSensoERP.CrudTool v2.5
// CORREÇÃO v3.1: PKs de texto (código manual) aparecem no formulário
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera View Razor compatível com CrudBase.js e BaseListViewModel.
/// 
/// CORREÇÕES v3.1:
/// - PKs de texto aparecem no formulário (readonly na edição, editável na criação)
/// - PKs Identity/Guid continuam ocultas (auto-geradas)
/// - Layout = "_CrudLayout"
/// - Usa @await Html.PartialAsync("_CrudTable", Model)
/// - window.crudPermissions (não pagePermissions)
/// </summary>
public static class ViewTemplate
{
    /// <summary>
    /// Alias para GenerateIndex - mantém compatibilidade com FullStackGeneratorService.
    /// </summary>
    public static GeneratedFile Generate(EntityConfig entity) => GenerateIndex(entity);

    /// <summary>
    /// Gera a View Index.cshtml seguindo o padrão do projeto.
    /// </summary>
    public static GeneratedFile GenerateIndex(EntityConfig entity)
    {
        var formFields = GenerateFormFields(entity);
        var iconClass = "fas fa-list";

        var content = $@"@model RhSensoERP.Web.Models.{entity.PluralName}.{entity.PluralName}ListViewModel
@{{
    ViewData[""Title""] = Model.PageTitle;
    Layout = ""_CrudLayout"";
    ViewData[""EntityName""] = ""{entity.DisplayName}"";
    ViewData[""IconClass""] = ""{iconClass}"";
}}

@* ============================================================================
   Permissões injetadas para JavaScript (ANTES do script específico)
   ============================================================================ *@
<script>
    window.crudPermissions = {{
        canCreate: @Model.CanCreate.ToString().ToLower(),
        canEdit: @Model.CanEdit.ToString().ToLower(),
        canDelete: @Model.CanDelete.ToString().ToLower(),
        canView: @Model.CanView.ToString().ToLower(),
        actions: ""@Model.UserPermissions""
    }};
</script>

@* ============================================================================
   Tabela usando partial _CrudTable (toolbar + datatable)
   ============================================================================ *@
@await Html.PartialAsync(""_CrudTable"", Model)

@* ============================================================================
   Conteúdo do Modal (apenas campos do formulário)
   O wrapper do modal está no _CrudLayout
   ============================================================================ *@
@section ModalContent {{
    <input type=""hidden"" id=""Id"" name=""Id"" />
    <div class=""row"">
{formFields}
    </div>
}}

@* ============================================================================
   Script específico da entidade
   ============================================================================ *@
@section PageScripts {{
    <script src=""~/js/{entity.PluralNameLower}/{entity.NameLower}.js"" asp-append-version=""true""></script>
}}
";

        return new GeneratedFile
        {
            FileName = "Index.cshtml",
            RelativePath = $"Web/Views/{entity.PluralName}/Index.cshtml",
            Content = content,
            FileType = "View"
        };
    }

    #region Helper Methods

    /// <summary>
    /// Gera os campos do formulário para o modal.
    /// CORREÇÃO v3.1: PKs de texto (código manual) aparecem no formulário.
    /// </summary>
    private static string GenerateFormFields(EntityConfig entity)
    {
        var sb = new StringBuilder();

        // =========================================================================
        // CORREÇÃO v3.1: Lógica para incluir PKs de texto no formulário
        // =========================================================================
        // PKs auto-geradas (Identity ou Guid) são puladas
        // PKs de texto (código manual) DEVEM aparecer no formulário
        var formProps = entity.Properties
            .Where(p =>
            {
                // Se não deve mostrar no form, pula
                if (p.Form?.Show != true) return false;

                // Se é PK auto-gerada (Identity ou Guid), pula
                if (p.IsPrimaryKey && (p.IsIdentity || p.IsGuid)) return false;

                // Caso contrário, inclui (inclusive PKs de texto)
                return true;
            })
            .OrderBy(p => p.Form!.Order)
            .ToList();

        foreach (var prop in formProps)
        {
            var config = prop.Form!;
            var colSize = config.ColSize;
            var inputType = config.InputType;
            var placeholder = config.Placeholder ?? $"Digite {prop.DisplayName.ToLower()}...";
            var required = prop.Required ? "required" : "";
            var maxLength = prop.MaxLength.HasValue ? $@" maxlength=""{prop.MaxLength.Value}""" : "";
            var step = prop.IsDecimal ? @" step=""0.01""" : "";

            // =========================================================================
            // CORREÇÃO v3.1: PKs de texto são obrigatórias e readonly na edição
            // =========================================================================
            var isPkTexto = prop.IsPrimaryKey && !prop.IsIdentity && !prop.IsGuid;

            // PKs de texto são sempre obrigatórias
            if (isPkTexto)
            {
                required = "required";
            }

            // Disabled: do config OU PK de texto (readonly na edição, habilitado na criação)
            // O JavaScript controla isso: desabilita na edição, habilita na criação
            var disabled = config.Disabled ? "disabled" : "";

            // Para PKs de texto, adiciona atributo data-pk-text para controle via JS
            var pkTextAttr = isPkTexto ? @" data-pk-text=""true""" : "";

            var helpText = !string.IsNullOrEmpty(config.HelpText)
                ? $@"
            <small class=""form-text text-muted"">{config.HelpText}</small>"
                : "";

            // Badge para PK de texto
            var pkBadge = isPkTexto
                ? @" <span class=""badge bg-warning text-dark"" title=""Chave primária - editável apenas na criação"">PK</span>"
                : "";

            string inputHtml;

            if (inputType == "textarea")
            {
                inputHtml = $@"<textarea class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                       rows=""{config.Rows}"" placeholder=""{placeholder}"" {required} {disabled}{maxLength}{pkTextAttr}></textarea>";
            }
            else if (inputType == "select")
            {
                inputHtml = $@"<select class=""form-select"" id=""{prop.Name}"" name=""{prop.Name}"" {required} {disabled}{pkTextAttr}>
                <option value="""">Selecione...</option>
                <!-- Preencher via JavaScript -->
            </select>";
            }
            else if (inputType == "checkbox")
            {
                sb.AppendLine($@"        <div class=""col-md-{colSize} mb-3"">
            <div class=""form-check"">
                <input type=""checkbox"" class=""form-check-input"" id=""{prop.Name}"" name=""{prop.Name}"" {disabled}{pkTextAttr} />
                <label class=""form-check-label"" for=""{prop.Name}"">{prop.DisplayName}{pkBadge}</label>
            </div>{helpText}
        </div>");
                continue;
            }
            else
            {
                inputHtml = $@"<input type=""{inputType}"" class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                   placeholder=""{placeholder}"" {required} {disabled}{maxLength}{step}{pkTextAttr} />";
            }

            var requiredStar = prop.Required || isPkTexto ? @" <span class=""text-danger"">*</span>" : "";

            sb.AppendLine($@"        <div class=""col-md-{colSize} mb-3"">
            <label for=""{prop.Name}"" class=""form-label"">
                {prop.DisplayName}{requiredStar}{pkBadge}
            </label>
            {inputHtml}{helpText}
        </div>");
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    #endregion
}