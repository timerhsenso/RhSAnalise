// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE REPOSITORY
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class RepositoryTemplate
{
    public static string GenerateInterface(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Repository Interface");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine($"using {entity.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Interface do repositório de {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public interface I{entity.ClassName}Repository");
        sb.AppendLine("{");

        // GetById
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Obtém {entity.DisplayName} pelo código.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    Task<{entity.ClassName}?> GetBy{pkName}Async({pkType} {ToCamelCase(pkName)}, CancellationToken ct = default);");
        sb.AppendLine();

        // GetPaged
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Lista {entity.PluralName} com paginação.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    Task<(IEnumerable<{entity.ClassName}> Items, int TotalCount)> GetPagedAsync(");
        sb.AppendLine("        int page,");
        sb.AppendLine("        int pageSize,");
        sb.AppendLine("        string? search = null,");
        sb.AppendLine("        string? sortBy = null,");
        sb.AppendLine("        bool desc = false,");
        sb.AppendLine("        CancellationToken ct = default);");
        sb.AppendLine();

        // Search
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Busca {entity.PluralName} por termo.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    Task<List<{entity.ClassName}>> SearchAsync(string? term, int take, CancellationToken ct = default);");
        sb.AppendLine();

        // Exists
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Verifica se {entity.DisplayName} existe.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    Task<bool> ExistsAsync({pkType} {ToCamelCase(pkName)}, CancellationToken ct = default);");
        sb.AppendLine();

        // Add
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Adiciona {entity.DisplayName}.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    Task AddAsync({entity.ClassName} entity, CancellationToken ct = default);");
        sb.AppendLine();

        // Update
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Atualiza {entity.DisplayName}.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    void Update({entity.ClassName} entity);");
        sb.AppendLine();

        // Remove
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Remove {entity.DisplayName}.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    void Remove({entity.ClassName} entity);");
        sb.AppendLine();

        // SaveChanges
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Salva as alterações.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    Task<int> SaveChangesAsync(CancellationToken ct = default);");

        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateImplementation(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";
        var camelPk = ToCamelCase(pkName);

        // Propriedades string para busca
        var searchableProps = entity.ScalarProperties
            .Where(p => p.IsString && !p.IsKey)
            .Take(3)
            .ToList();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Repository Implementation");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace};");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine($"using {entity.DbContextNamespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.RepositoryImplementationNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Repositório de {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {entity.ClassName}Repository : I{entity.ClassName}Repository");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly {entity.DbContextName} _db;");
        sb.AppendLine();
        sb.AppendLine($"    public {entity.ClassName}Repository({entity.DbContextName} db) => _db = db;");
        sb.AppendLine();

        // GetById
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public Task<{entity.ClassName}?> GetBy{pkName}Async({pkType} {camelPk}, CancellationToken ct) =>");
        sb.AppendLine($"        _db.Set<{entity.ClassName}>()");
        sb.AppendLine("           .AsNoTracking()");
        sb.AppendLine($"           .FirstOrDefaultAsync(e => e.{pkName} == {camelPk}, ct);");
        sb.AppendLine();

        // GetPaged
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public async Task<(IEnumerable<{entity.ClassName}> Items, int TotalCount)> GetPagedAsync(");
        sb.AppendLine("        int page,");
        sb.AppendLine("        int pageSize,");
        sb.AppendLine("        string? search,");
        sb.AppendLine("        string? sortBy,");
        sb.AppendLine("        bool desc,");
        sb.AppendLine("        CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var query = _db.Set<{entity.ClassName}>().AsNoTracking();");
        sb.AppendLine();

        // Search filter
        if (searchableProps.Any())
        {
            sb.AppendLine("        // Filtro de busca");
            sb.AppendLine("        if (!string.IsNullOrWhiteSpace(search))");
            sb.AppendLine("        {");
            sb.AppendLine("            var term = search.Trim().ToLower();");
            sb.Append("            query = query.Where(e => ");
            
            var conditions = new List<string>();
            if (pk?.IsString == true)
                conditions.Add($"e.{pkName}.ToLower().Contains(term)");
            
            foreach (var prop in searchableProps)
                conditions.Add($"e.{prop.Name}.ToLower().Contains(term)");
            
            sb.Append(string.Join(" || ", conditions));
            sb.AppendLine(");");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        // Total count
        sb.AppendLine("        var totalCount = await query.CountAsync(ct);");
        sb.AppendLine();

        // Sorting
        sb.AppendLine("        // Ordenação");
        sb.AppendLine("        query = ApplySorting(query, sortBy, desc);");
        sb.AppendLine();

        // Pagination
        sb.AppendLine("        // Paginação");
        sb.AppendLine("        var items = await query");
        sb.AppendLine("            .Skip((page - 1) * pageSize)");
        sb.AppendLine("            .Take(pageSize)");
        sb.AppendLine("            .ToListAsync(ct);");
        sb.AppendLine();
        sb.AppendLine("        return (items, totalCount);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Search
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public async Task<List<{entity.ClassName}>> SearchAsync(string? term, int take, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var query = _db.Set<{entity.ClassName}>().AsNoTracking();");
        sb.AppendLine();
        if (searchableProps.Any())
        {
            sb.AppendLine("        if (!string.IsNullOrWhiteSpace(term))");
            sb.AppendLine("        {");
            sb.AppendLine("            term = term.Trim();");
            sb.Append("            query = query.Where(e => ");
            
            var conditions = new List<string>();
            if (pk?.IsString == true)
                conditions.Add($"e.{pkName}.Contains(term)");
            
            foreach (var prop in searchableProps)
                conditions.Add($"e.{prop.Name}.Contains(term)");
            
            sb.Append(string.Join(" || ", conditions));
            sb.AppendLine(");");
            sb.AppendLine("        }");
            sb.AppendLine();
        }
        sb.AppendLine($"        return await query.OrderBy(e => e.{pkName}).Take(take <= 0 ? 20 : take).ToListAsync(ct);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Exists
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public Task<bool> ExistsAsync({pkType} {camelPk}, CancellationToken ct) =>");
        sb.AppendLine($"        _db.Set<{entity.ClassName}>().AsNoTracking().AnyAsync(e => e.{pkName} == {camelPk}, ct);");
        sb.AppendLine();

        // Add
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public async Task AddAsync({entity.ClassName} entity, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine($"        await _db.Set<{entity.ClassName}>().AddAsync(entity, ct);");
        sb.AppendLine("        await _db.SaveChangesAsync(ct);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Update
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public void Update({entity.ClassName} entity) => _db.Set<{entity.ClassName}>().Update(entity);");
        sb.AppendLine();

        // Remove
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine($"    public void Remove({entity.ClassName} entity) => _db.Set<{entity.ClassName}>().Remove(entity);");
        sb.AppendLine();

        // SaveChanges
        sb.AppendLine($"    /// <inheritdoc />");
        sb.AppendLine("    public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);");
        sb.AppendLine();

        // ApplySorting helper
        sb.AppendLine("    // Helper de ordenação");
        sb.AppendLine($"    private static IQueryable<{entity.ClassName}> ApplySorting(IQueryable<{entity.ClassName}> query, string? sortBy, bool desc)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (string.IsNullOrWhiteSpace(sortBy))");
        sb.AppendLine($"            return query.OrderBy(e => e.{pkName});");
        sb.AppendLine();
        sb.AppendLine("        return sortBy.ToLower() switch");
        sb.AppendLine("        {");

        foreach (var prop in entity.ScalarProperties.Where(p => !p.IsNavigation))
        {
            var propLower = prop.Name.ToLower();
            sb.AppendLine($"            \"{propLower}\" => desc ? query.OrderByDescending(e => e.{prop.Name}) : query.OrderBy(e => e.{prop.Name}),");
        }

        sb.AppendLine($"            _ => query.OrderBy(e => e.{pkName})");
        sb.AppendLine("        };");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
