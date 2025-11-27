// =============================================================================
// RHSENSOERP CRUD TOOL - MAIN GENERATOR
// =============================================================================
using RhSensoERP.CrudTool.Models;
using RhSensoERP.CrudTool.Templates;
using Spectre.Console;

namespace RhSensoERP.CrudTool.Generators;

/// <summary>
/// Gerador principal de CRUD
/// </summary>
public class CrudGenerator
{
    private readonly CrudConfig _config;
    private readonly string _solutionRoot;
    private readonly string _apiPath;
    private readonly string _webPath;

    public CrudGenerator(CrudConfig config)
    {
        _config = config;
        _solutionRoot = Path.GetFullPath(config.SolutionRoot);
        _apiPath = Path.Combine(_solutionRoot, config.ApiProject);
        _webPath = Path.Combine(_solutionRoot, config.WebProject);
    }

    /// <summary>
    /// Gera todos os arquivos para uma Entity
    /// </summary>
    public async Task GenerateAsync(EntityConfig entity)
    {
        // API Controller
        if (entity.Generate.ApiController)
        {
            var apiControllerPath = Path.Combine(
                _apiPath, 
                "Controllers", 
                entity.Module, 
                $"{entity.PluralName}Controller.cs");
            
            var content = ApiControllerTemplate.Generate(entity);
            await WriteFileAsync(apiControllerPath, content);
        }

        // Web Controller
        if (entity.Generate.WebController)
        {
            var webControllerPath = Path.Combine(
                _webPath, 
                "Controllers", 
                $"{entity.PluralName}Controller.cs");
            
            var content = WebControllerTemplate.Generate(entity);
            await WriteFileAsync(webControllerPath, content);
        }

        // Web Models
        if (entity.Generate.WebModels)
        {
            var modelsPath = Path.Combine(_webPath, "Models", entity.PluralName);
            
            // DTO
            await WriteFileAsync(
                Path.Combine(modelsPath, $"{entity.Name}Dto.cs"),
                WebModelsTemplate.GenerateDto(entity));
            
            // CreateDto
            await WriteFileAsync(
                Path.Combine(modelsPath, $"Create{entity.Name}Dto.cs"),
                WebModelsTemplate.GenerateCreateDto(entity));
            
            // UpdateDto
            await WriteFileAsync(
                Path.Combine(modelsPath, $"Update{entity.Name}Dto.cs"),
                WebModelsTemplate.GenerateUpdateDto(entity));
            
            // ListViewModel
            await WriteFileAsync(
                Path.Combine(modelsPath, $"{entity.PluralName}ListViewModel.cs"),
                WebModelsTemplate.GenerateListViewModel(entity));
        }

        // Web Services
        if (entity.Generate.WebServices)
        {
            var servicesPath = Path.Combine(_webPath, "Services", entity.PluralName);
            
            // Interface
            await WriteFileAsync(
                Path.Combine(servicesPath, $"I{entity.Name}ApiService.cs"),
                WebServicesTemplate.GenerateInterface(entity));
            
            // Implementation
            await WriteFileAsync(
                Path.Combine(servicesPath, $"{entity.Name}ApiService.cs"),
                WebServicesTemplate.GenerateImplementation(entity));
        }
    }

    /// <summary>
    /// Escreve arquivo criando diretórios se necessário
    /// </summary>
    private static async Task WriteFileAsync(string path, string content)
    {
        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await File.WriteAllTextAsync(path, content);
        
        var relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
        AnsiConsole.MarkupLine($"  [green]✓[/] {relativePath}");
    }
}
