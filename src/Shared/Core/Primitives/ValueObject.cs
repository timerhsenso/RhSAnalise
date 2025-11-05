// Copyright (c) RhSenso. Todos os direitos reservados.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Objeto de Valor (DDD) com igualdade baseada em componentes.
/// Regras StyleCop atendidas: SA1201 (ordem), SA1615 (returns).
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Compara por valor: duas instâncias são iguais se os seus componentes são iguais e na mesma ordem.
    /// </summary>
    /// <param name="obj">Outro objeto.</param>
    /// <returns><c>true</c> se os componentes são iguais; caso contrário, <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is ValueObject other && Values().SequenceEqual(other.Values());

    /// <summary>
    /// Calcula um hash com base nos componentes retornados por <see cref="Values"/>.
    /// </summary>
    /// <returns>Hash code do objeto de valor.</returns>
    public override int GetHashCode()
        => Values().Aggregate(17, (acc, o) => (acc * 23) + (o?.GetHashCode() ?? 0));

    /// <summary>
    /// Retorna, em ordem determinística, os componentes que participam da igualdade.
    /// </summary>
    /// <returns>Sequência dos componentes de igualdade.</returns>
    protected abstract IEnumerable<object?> Values();

    // Operadores ficam por último (SA1201)
    /// <summary>
    /// Operador de igualdade por valor.
    /// </summary>
    /// <param name="left">Valor da esquerda.</param>
    /// <param name="right">Valor da direita.</param>
    /// <returns><c>true</c> se iguais por valor; caso contrário, <c>false</c>.</returns>
    [SuppressMessage("Major Code Smell", "S3875", Justification = "ValueObject compara por valor por design (DDD).")]
    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade por valor.
    /// </summary>
    /// <param name="left">Valor da esquerda.</param>
    /// <param name="right">Valor da direita.</param>
    /// <returns><c>true</c> se diferentes por valor; caso contrário, <c>false</c>.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
