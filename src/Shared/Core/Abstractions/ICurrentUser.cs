// Copyright (c) RhSenso. Todos os direitos reservados.

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Usuário atual (contexto da requisição).</summary>
public interface ICurrentUser
{
    /// <summary>Identificador do usuário autenticado (ou null).</summary>
    string? UserId { get; }

    /// <summary>Nome do usuário.</summary>
    string? UserName { get; }
}
