// src/API/Configuration/ModuleGroupConvention.cs
#nullable enable
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Define o ApiExplorer.GroupName automaticamente a partir do namespace do controller,
/// sem sobrescrever quando o controller já definiu via atributo.
/// Suporta "RhSensoERP.Modules.*" e "RhSensoERP.API.Controllers.*".
/// </summary>
public sealed class ModuleGroupConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // Se já foi definido manualmente, respeita
        if (!string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName))
            return;

        var ns = controller.ControllerType.Namespace ?? string.Empty;

        static bool Has(string source, string token) =>
            source.Contains(token, StringComparison.OrdinalIgnoreCase);

        string? group = ns switch
        {
            var s when Has(s, ".GestaoDePessoas.") || Has(s, "Modules.GestaoDePessoas") => "GestaoDePessoas",
            var s when Has(s, ".Identity.") || Has(s, "Identity") => "Identity",
            var s when Has(s, ".Diagnostics.") || Has(s, "Diagnostics") => "Diagnostics",
            var s when Has(s, ".ControleDePonto.") || Has(s, "Modules.ControleDePonto") => "ControleDePonto",
            var s when Has(s, ".Avaliacoes.") || Has(s, "Modules.Avaliacoes") => "Avaliacoes",
            var s when Has(s, ".Esocial.") || Has(s, "Modules.Esocial") => "Esocial",
            var s when Has(s, ".SaudeOcupacional.") || Has(s, "Modules.SaudeOcupacional") => "SaudeOcupacional",
            var s when Has(s, ".Treinamentos.") || Has(s, "Modules.Treinamentos") => "Treinamentos",
            _ => null
        };

        if (group is not null)
            controller.ApiExplorer.GroupName = group;
    }
}
