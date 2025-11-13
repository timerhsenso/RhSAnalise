// src/API/Configuration/ModuleGroupConvention.cs
#nullable enable
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Define o ApiExplorer.GroupName automaticamente a partir do namespace do controller.
/// Não sobrescreve quando o controller já definiu via atributo [ApiExplorerSettings].
/// 
/// Hierarquia de detecção:
/// 1. Controllers em RhSensoERP.Modules.* → Nome do módulo
/// 2. Controllers em RhSensoERP.API.Controllers.* → Nome da pasta/segmento
/// 3. Controllers em RhSensoERP.API.Controllers → "API" (fallback)
/// </summary>
public sealed class ModuleGroupConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // Se já foi definido manualmente via [ApiExplorerSettings(GroupName = "...")], respeita
        if (!string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName))
            return;

        var ns = controller.ControllerType.Namespace ?? string.Empty;

        // Atribui o GroupName baseado no namespace
        var group = ResolveGroupName(ns);

        if (group is not null)
        {
            controller.ApiExplorer.GroupName = group;
        }
    }

    private static string? ResolveGroupName(string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            return null;

        // Helper para verificar se contém um token (case-insensitive)
        static bool Contains(string source, string token) =>
            source.Contains(token, StringComparison.OrdinalIgnoreCase);

        // ===== PRIORIDADE 1: Módulos em RhSensoERP.Modules.* =====
        if (Contains(namespaceName, ".Modules."))
        {
            if (Contains(namespaceName, "GestaoDePessoas")) return "GestaoDePessoas";
            if (Contains(namespaceName, "ControleDePonto")) return "ControleDePonto";
            if (Contains(namespaceName, "Avaliacoes")) return "Avaliacoes";
            if (Contains(namespaceName, "Esocial")) return "Esocial";
            if (Contains(namespaceName, "SaudeOcupacional")) return "SaudeOcupacional";
            if (Contains(namespaceName, "Treinamentos")) return "Treinamentos";
        }

        // ===== PRIORIDADE 2: Controllers em RhSensoERP.API.Controllers.* =====
        // Exemplo: RhSensoERP.API.Controllers.Identity.UsuariosController → Identity
        // Exemplo: RhSensoERP.API.Controllers.GestaoDePessoas.Tabelas.BancosController → GestaoDePessoas
        if (Contains(namespaceName, ".API.Controllers."))
        {
            // Extrai o segmento após ".Controllers."
            var parts = namespaceName.Split('.');
            var controllersIndex = Array.FindIndex(parts, p =>
                p.Equals("Controllers", StringComparison.OrdinalIgnoreCase));

            if (controllersIndex >= 0 && controllersIndex < parts.Length - 1)
            {
                var segment = parts[controllersIndex + 1];

                // Mapeia segmentos conhecidos
                return segment switch
                {
                    "Identity" => "Identity",
                    "GestaoDePessoas" => "GestaoDePessoas",
                    "ControleDePonto" => "ControleDePonto",
                    "Avaliacoes" => "Avaliacoes",
                    "Esocial" => "Esocial",
                    "SaudeOcupacional" => "SaudeOcupacional",
                    "Treinamentos" => "Treinamentos",
                    _ => segment // Retorna o segmento mesmo se não mapeado
                };
            }
        }

        // ===== PRIORIDADE 3: Controllers diretos em RhSensoERP.API.Controllers =====
        // Exemplo: RhSensoERP.API.Controllers.DiagnosticsController
        if (Contains(namespaceName, ".API.Controllers") &&
            !Contains(namespaceName, ".API.Controllers."))
        {
            // Verifica keywords conhecidas no namespace
            if (Contains(namespaceName, "Diagnostics")) return "Diagnostics";
            if (Contains(namespaceName, "Identity")) return "Identity";

            // Fallback: retorna "Diagnostics" para controllers não classificados
            return "Diagnostics";
        }

        // ===== PRIORIDADE 4: Identity Module específico =====
        if (Contains(namespaceName, ".Identity."))
        {
            return "Identity";
        }

        // ===== FALLBACK: Sem agrupamento =====
        return null;
    }
}