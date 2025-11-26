// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE COMMANDS
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class CommandsTemplate
{
    public static string GenerateCreateCommand(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Create Command");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.CommandsNamespace};");
        sb.AppendLine();

        // Command Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Command para criar {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Create{entity.ClassName}Command(Create{entity.ClassName}Request Request) : IRequest<Result<{pkType}>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Create{entity.ClassName}Command.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Create{entity.ClassName}CommandHandler : IRequestHandler<Create{entity.ClassName}Command, Result<{pkType}>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine($"    private readonly ILogger<Create{entity.ClassName}CommandHandler> _logger;");
        sb.AppendLine();
        sb.AppendLine($"    public Create{entity.ClassName}CommandHandler(");
        sb.AppendLine($"        I{entity.ClassName}Repository repository,");
        sb.AppendLine($"        ILogger<Create{entity.ClassName}CommandHandler> logger)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _logger = logger;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<{pkType}>> Handle(Create{entity.ClassName}Command command, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            var request = command.Request;");
        sb.AppendLine();

        // Verificar duplicidade se tiver PK string
        if (pkType == "string")
        {
            sb.AppendLine($"            // Verificar se já existe");
            sb.AppendLine($"            var exists = await _repository.ExistsAsync(request.{pkName}, ct);");
            sb.AppendLine("            if (exists)");
            sb.AppendLine("            {");
            sb.AppendLine($"                return Result<{pkType}>.Failure(\"DUPLICATE\", \"{entity.DisplayName} já existe com este código.\");");
            sb.AppendLine("            }");
            sb.AppendLine();
        }

        sb.AppendLine($"            var entity = new {entity.Namespace}.{entity.ClassName}");
        sb.AppendLine("            {");
        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInCreateDto && !p.IsReadOnly && !IsBaseEntity(p.Name)))
        {
            sb.AppendLine($"                {prop.Name} = request.{prop.Name},");
        }
        sb.AppendLine("            };");
        sb.AppendLine();
        sb.AppendLine("            await _repository.AddAsync(entity, ct);");
        sb.AppendLine();
        sb.AppendLine($"            _logger.LogInformation(\"{entity.DisplayName} criado: {{Codigo}}\", entity.{pkName});");
        sb.AppendLine();
        sb.AppendLine($"            return Result<{pkType}>.Success(entity.{pkName});");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine($"            _logger.LogError(ex, \"Erro ao criar {entity.DisplayName}\");");
        sb.AppendLine($"            return Result<{pkType}>.Failure(\"CREATE_ERROR\", \"Erro ao criar {entity.DisplayName}.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateUpdateCommand(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Update Command");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.CommandsNamespace};");
        sb.AppendLine();

        // Command Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Command para atualizar {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Update{entity.ClassName}Command({pkType} {pkName}, Update{entity.ClassName}Request Request) : IRequest<Result<bool>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Update{entity.ClassName}Command.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Update{entity.ClassName}CommandHandler : IRequestHandler<Update{entity.ClassName}Command, Result<bool>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine($"    private readonly ILogger<Update{entity.ClassName}CommandHandler> _logger;");
        sb.AppendLine();
        sb.AppendLine($"    public Update{entity.ClassName}CommandHandler(");
        sb.AppendLine($"        I{entity.ClassName}Repository repository,");
        sb.AppendLine($"        ILogger<Update{entity.ClassName}CommandHandler> logger)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _logger = logger;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<bool>> Handle(Update{entity.ClassName}Command command, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine($"            var entity = await _repository.GetBy{pkName}Async(command.{pkName}, ct);");
        sb.AppendLine("            if (entity == null)");
        sb.AppendLine("            {");
        sb.AppendLine($"                return Result<bool>.Failure(\"NOT_FOUND\", \"{entity.DisplayName} não encontrado.\");");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            // Atualizar propriedades");

        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInUpdateDto && !p.IsReadOnly && !p.IsKey && !IsBaseEntity(p.Name)))
        {
            sb.AppendLine($"            entity.{prop.Name} = command.Request.{prop.Name};");
        }

        sb.AppendLine();
        sb.AppendLine("            _repository.Update(entity);");
        sb.AppendLine("            await _repository.SaveChangesAsync(ct);");
        sb.AppendLine();
        sb.AppendLine($"            _logger.LogInformation(\"{entity.DisplayName} atualizado: {{Codigo}}\", command.{pkName});");
        sb.AppendLine();
        sb.AppendLine("            return Result<bool>.Success(true);");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine($"            _logger.LogError(ex, \"Erro ao atualizar {entity.DisplayName}: {{Codigo}}\", command.{pkName});");
        sb.AppendLine("            return Result<bool>.Failure(\"UPDATE_ERROR\", \"Erro ao atualizar {entity.DisplayName}.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateDeleteCommand(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Delete Command");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.CommandsNamespace};");
        sb.AppendLine();

        // Command Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Command para excluir {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Delete{entity.ClassName}Command({pkType} {pkName}) : IRequest<Result<bool>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Delete{entity.ClassName}Command.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Delete{entity.ClassName}CommandHandler : IRequestHandler<Delete{entity.ClassName}Command, Result<bool>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine($"    private readonly ILogger<Delete{entity.ClassName}CommandHandler> _logger;");
        sb.AppendLine();
        sb.AppendLine($"    public Delete{entity.ClassName}CommandHandler(");
        sb.AppendLine($"        I{entity.ClassName}Repository repository,");
        sb.AppendLine($"        ILogger<Delete{entity.ClassName}CommandHandler> logger)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _logger = logger;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<bool>> Handle(Delete{entity.ClassName}Command command, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine($"            var entity = await _repository.GetBy{pkName}Async(command.{pkName}, ct);");
        sb.AppendLine("            if (entity == null)");
        sb.AppendLine("            {");
        sb.AppendLine($"                return Result<bool>.Failure(\"NOT_FOUND\", \"{entity.DisplayName} não encontrado.\");");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            _repository.Remove(entity);");
        sb.AppendLine("            await _repository.SaveChangesAsync(ct);");
        sb.AppendLine();
        sb.AppendLine($"            _logger.LogInformation(\"{entity.DisplayName} excluído: {{Codigo}}\", command.{pkName});");
        sb.AppendLine();
        sb.AppendLine("            return Result<bool>.Success(true);");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine($"            _logger.LogError(ex, \"Erro ao excluir {entity.DisplayName}: {{Codigo}}\", command.{pkName});");
        sb.AppendLine("            return Result<bool>.Failure(\"DELETE_ERROR\", \"Erro ao excluir {entity.DisplayName}.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateDeleteBatchCommand(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Delete Batch Command");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.CommandsNamespace};");
        sb.AppendLine();

        // BatchDeleteResult
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Resultado da exclusão em lote.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {entity.ClassName}BatchDeleteResult");
        sb.AppendLine("{");
        sb.AppendLine("    public int TotalRequested { get; set; }");
        sb.AppendLine("    public int TotalDeleted { get; set; }");
        sb.AppendLine($"    public List<{pkType}> NotFound {{ get; set; }} = new();");
        sb.AppendLine("}");
        sb.AppendLine();

        // Command Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Command para excluir múltiplos {entity.PluralName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Delete{entity.PluralName}Command(List<{pkType}> Codigos) : IRequest<Result<{entity.ClassName}BatchDeleteResult>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Delete{entity.PluralName}Command.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Delete{entity.PluralName}CommandHandler : IRequestHandler<Delete{entity.PluralName}Command, Result<{entity.ClassName}BatchDeleteResult>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine($"    private readonly ILogger<Delete{entity.PluralName}CommandHandler> _logger;");
        sb.AppendLine();
        sb.AppendLine($"    public Delete{entity.PluralName}CommandHandler(");
        sb.AppendLine($"        I{entity.ClassName}Repository repository,");
        sb.AppendLine($"        ILogger<Delete{entity.PluralName}CommandHandler> logger)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _logger = logger;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<{entity.ClassName}BatchDeleteResult>> Handle(Delete{entity.PluralName}Command command, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine($"            var result = new {entity.ClassName}BatchDeleteResult");
        sb.AppendLine("            {");
        sb.AppendLine("                TotalRequested = command.Codigos.Count");
        sb.AppendLine("            };");
        sb.AppendLine();
        sb.AppendLine("            foreach (var codigo in command.Codigos)");
        sb.AppendLine("            {");
        sb.AppendLine($"                var entity = await _repository.GetBy{pkName}Async(codigo, ct);");
        sb.AppendLine("                if (entity == null)");
        sb.AppendLine("                {");
        sb.AppendLine("                    result.NotFound.Add(codigo);");
        sb.AppendLine("                    continue;");
        sb.AppendLine("                }");
        sb.AppendLine();
        sb.AppendLine("                _repository.Remove(entity);");
        sb.AppendLine("                result.TotalDeleted++;");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            await _repository.SaveChangesAsync(ct);");
        sb.AppendLine();
        sb.AppendLine($"            _logger.LogInformation(\"{entity.PluralName} excluídos: {{Total}}\", result.TotalDeleted);");
        sb.AppendLine();
        sb.AppendLine($"            return Result<{entity.ClassName}BatchDeleteResult>.Success(result);");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine($"            _logger.LogError(ex, \"Erro ao excluir {entity.PluralName} em lote\");");
        sb.AppendLine($"            return Result<{entity.ClassName}BatchDeleteResult>.Failure(\"DELETE_BATCH_ERROR\", \"Erro ao excluir {entity.PluralName}.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static readonly string[] BaseEntityProps = { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" };
    private static bool IsBaseEntity(string name) => BaseEntityProps.Contains(name);
}
