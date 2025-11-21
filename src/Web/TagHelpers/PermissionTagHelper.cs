// src/Web/TagHelpers/PermissionTagHelper.cs

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Claims;

namespace RhSensoERP.Web.TagHelpers;

/// <summary>
/// Tag Helper para controlar a visibilidade de elementos com base em permissões do usuário.
/// Uso: &lt;div permission-function="BANCOS" permission-action="I"&gt;...&lt;/div&gt;
/// </summary>
[HtmlTargetElement(Attributes = "permission-function,permission-action")]
public class PermissionTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Código da função (ex: "BANCOS").
    /// </summary>
    [HtmlAttributeName("permission-function")]
    public string? PermissionFunction { get; set; }

    /// <summary>
    /// Ação requerida (I=Incluir, A=Alterar, E=Excluir, C=Consultar).
    /// </summary>
    [HtmlAttributeName("permission-action")]
    public char PermissionAction { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(PermissionFunction))
        {
            output.SuppressOutput();
            return;
        }

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            output.SuppressOutput();
            return;
        }

        // Busca a claim de permissão do usuário
        var permissionClaim = user.FindFirst($"permission:{PermissionFunction}");
        if (permissionClaim == null || !permissionClaim.Value.Contains(PermissionAction))
        {
            output.SuppressOutput();
            return;
        }

        // Remove os atributos customizados do HTML final
        output.Attributes.RemoveAll("permission-function");
        output.Attributes.RemoveAll("permission-action");
    }
}

/// <summary>
/// Tag Helper para controlar a visibilidade de elementos com base em múltiplas permissões (OR).
/// Uso: &lt;div permission-function="BANCOS" permission-actions="IA"&gt;...&lt;/div&gt;
/// </summary>
[HtmlTargetElement(Attributes = "permission-function,permission-actions")]
public class PermissionAnyTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionAnyTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Código da função (ex: "BANCOS").
    /// </summary>
    [HtmlAttributeName("permission-function")]
    public string? PermissionFunction { get; set; }

    /// <summary>
    /// Ações requeridas (ex: "IA" para Incluir OU Alterar).
    /// </summary>
    [HtmlAttributeName("permission-actions")]
    public string? PermissionActions { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(PermissionFunction) || string.IsNullOrWhiteSpace(PermissionActions))
        {
            output.SuppressOutput();
            return;
        }

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            output.SuppressOutput();
            return;
        }

        // Busca a claim de permissão do usuário
        var permissionClaim = user.FindFirst($"permission:{PermissionFunction}");
        if (permissionClaim == null)
        {
            output.SuppressOutput();
            return;
        }

        // Verifica se o usuário tem pelo menos uma das permissões
        var hasAnyPermission = PermissionActions.Any(action => permissionClaim.Value.Contains(action));
        if (!hasAnyPermission)
        {
            output.SuppressOutput();
            return;
        }

        // Remove os atributos customizados do HTML final
        output.Attributes.RemoveAll("permission-function");
        output.Attributes.RemoveAll("permission-actions");
    }
}
