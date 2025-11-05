// Copyright (c) RhSenso. Todos os direitos reservados.

using System;

namespace RhSensoERP.Shared.Core.Common;

/// <summary>Guards para validações simples.</summary>
public static class Guard
{
    public static T NotNull<T>(T value, string paramName) where T : class
        => value ?? throw new ArgumentNullException(paramName);

    public static string NotNullOrWhiteSpace(string value, string paramName)
        => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Valor obrigatório.", paramName) : value;

    public static void Against(bool condition, string message)
    {
        if (condition) throw new InvalidOperationException(message);
    }
}
