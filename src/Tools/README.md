# RhSensoERP CRUD Tool v1.0

Ferramenta CLI profissional para geração de código CRUD completo (API + Web) para o projeto RhSensoERP.

## 🎯 Objetivo

Gerar arquivos Web/API **diretamente nos projetos corretos**, diferente do Source Generator que gera apenas no projeto da Entity.

## 📁 Estrutura

```
RhSensoERP.CrudTool/
├── RhSensoERP.CrudTool.csproj    # Projeto Console .NET 8
├── Program.cs                     # Entry point com Spectre.Console
├── Models/
│   └── CrudConfig.cs              # Modelos do JSON de configuração
├── Generators/
│   └── CrudGenerator.cs           # Orquestra a geração
├── Templates/
│   ├── ApiControllerTemplate.cs   # Template API Controller
│   ├── WebControllerTemplate.cs   # Template Web Controller
│   ├── WebModelsTemplate.cs       # Templates DTOs + ViewModel
│   └── WebServicesTemplate.cs     # Templates Services
├── crud-schema.json               # JSON Schema para validação
└── crud-config.example.json       # Exemplo de configuração
```

## 🚀 Instalação

### Opção 1: Como projeto na solution

1. Copie a pasta `RhSensoERP.CrudTool` para `src/Tools/`
2. Adicione à solution:
   ```bash
   dotnet sln add src/Tools/RhSensoERP.CrudTool/RhSensoERP.CrudTool.csproj
   ```

### Opção 2: Como dotnet tool global

```bash
cd src/Tools/RhSensoERP.CrudTool
dotnet pack
dotnet tool install --global --add-source ./nupkg RhSensoERP.CrudTool
```

## 📝 Uso

### 1. Crie o arquivo de configuração

Crie um arquivo `crud-config.json` na raiz da solution:

```json
{
  "$schema": "./crud-schema.json",
  "solutionRoot": ".",
  "apiProject": "src/RhSensoERP.API",
  "webProject": "src/Web",
  "entities": [
    {
      "name": "Sistema",
      "displayName": "Sistema",
      "pluralName": "Sistemas",
      "module": "Identity",
      "tableName": "tsistema",
      "cdSistema": "SEG",
      "cdFuncao": "SEG_FM_TSISTEMA",
      "primaryKey": {
        "property": "CdSistema",
        "type": "string",
        "column": "cdsistema"
      },
      "properties": [
        {
          "name": "CdSistema",
          "type": "string",
          "column": "cdsistema",
          "displayName": "Código",
          "maxLength": 10,
          "required": true,
          "isPrimaryKey": true
        },
        {
          "name": "DcSistema",
          "type": "string",
          "column": "dcsistema",
          "displayName": "Descrição",
          "maxLength": 100,
          "required": true
        },
        {
          "name": "Ativo",
          "type": "string",
          "column": "ativo",
          "displayName": "Ativo",
          "maxLength": 1,
          "defaultValue": "S"
        }
      ],
      "generate": {
        "apiController": true,
        "webController": true,
        "webModels": true,
        "webServices": true
      }
    }
  ]
}
```

### 2. Execute a ferramenta

```bash
# Como projeto
dotnet run --project src/Tools/RhSensoERP.CrudTool

# Como tool global
rhsenso-crud

# Especificando arquivo de configuração
rhsenso-crud ./meu-config.json
```

### 3. Saída esperada

```
   ____ ____  _   _ ____    _____           _ 
  / ___|  _ \| | | |  _ \  |_   _|__   ___ | |
 | |   | |_) | | | | | | |   | |/ _ \ / _ \| |
 | |___|  _ <| |_| | |_| |   | | (_) | (_) | |
  \____|_| \_\\___/|____/    |_|\___/ \___/|_|

📄 Lendo configuração: crud-config.json
✓ Configuração válida - 1 entity(s)

┌─────────┬──────────┬─────┬─────┐
│ Entity  │ Module   │ API │ Web │
├─────────┼──────────┼─────┼─────┤
│ Sistema │ Identity │ ✓   │ ✓   │
└─────────┴──────────┴─────┴─────┘

Gerar arquivos? [y/n]: y

  ✓ src/RhSensoERP.API/Controllers/Identity/SistemasController.cs
  ✓ src/Web/Controllers/SistemasController.cs
  ✓ src/Web/Models/Sistemas/SistemaDto.cs
  ✓ src/Web/Models/Sistemas/CreateSistemaDto.cs
  ✓ src/Web/Models/Sistemas/UpdateSistemaDto.cs
  ✓ src/Web/Models/Sistemas/SistemasListViewModel.cs
  ✓ src/Web/Services/Sistemas/ISistemaApiService.cs
  ✓ src/Web/Services/Sistemas/SistemaApiService.cs

✓ Geração concluída com sucesso!
```

## 📋 Arquivos Gerados

| Arquivo | Projeto | Descrição |
|---------|---------|-----------|
| `SistemasController.cs` | API | Controller REST com MediatR |
| `SistemasController.cs` | Web | Controller MVC com BaseCrudController |
| `SistemaDto.cs` | Web | DTO de leitura |
| `CreateSistemaDto.cs` | Web | DTO de criação com validações |
| `UpdateSistemaDto.cs` | Web | DTO de atualização com validações |
| `SistemasListViewModel.cs` | Web | ViewModel para listagem |
| `ISistemaApiService.cs` | Web | Interface do serviço |
| `SistemaApiService.cs` | Web | Implementação do serviço |

## ⚙️ Configuração

### Flags de geração

| Flag | Padrão | Descrição |
|------|--------|-----------|
| `apiController` | true | Gera Controller da API |
| `webController` | true | Gera Controller Web MVC |
| `webModels` | true | Gera DTOs e ViewModel |
| `webServices` | true | Gera Interface + Service |
| `view` | false | (Futuro) Gera Razor View |
| `javascript` | false | (Futuro) Gera DataTables JS |

### Tipos de PK suportados

- `string` - Para códigos alfanuméricos (ex: CdSistema)
- `int` - Para IDs inteiros
- `long` - Para IDs long
- `Guid` - Para IDs GUID

## 🔧 Próximos passos após geração

### 1. Registre o Service no DI

```csharp
// Program.cs ou Startup.cs
services.AddHttpClient<ISistemaApiService, SistemaApiService>(client =>
{
    client.BaseAddress = new Uri(Configuration["ApiUrl"]!);
});
```

### 2. Crie a View

```
Views/Sistemas/Index.cshtml
```

### 3. Crie o JavaScript

```
wwwroot/js/pages/sistemas.js
```

## 📊 Comparação com Source Generator

| Aspecto | Source Generator | CRUD Tool |
|---------|------------------|-----------|
| Execução | Automática no build | Manual via CLI |
| Projeto destino | Mesmo da Entity | Projetos corretos |
| Backend (DTOs, Commands...) | ✅ | ❌ |
| API Controller | ⚠️ Projeto errado | ✅ |
| Web Controller | ⚠️ Projeto errado | ✅ |
| Web Models/Services | ⚠️ Projeto errado | ✅ |

**Recomendação:** Use ambos!
- Source Generator para Backend (automático)
- CRUD Tool para Web/API (manual)

## 🛠️ Customização

### Adicionar novo template

1. Crie a classe em `Templates/`
2. Implemente método `Generate(EntityConfig entity)`
3. Chame no `CrudGenerator.cs`

### Modificar template existente

Edite diretamente o arquivo em `Templates/`. Os templates usam interpolação C# para substituição de variáveis.

## 📄 Licença

RhSenso Team © 2025
