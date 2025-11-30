# 🚀 Gerador Full-Stack v3.0 - RhSensoERP

Ferramenta web unificada para geração de código Full-Stack completo a partir do banco de dados.

## 📦 O que foi gerado

Esta ferramenta substitui o fluxo anterior de 2 ferramentas (GeradorEntidades + GeradorTool) por uma única aplicação web que gera **todos os arquivos** de uma vez:

### Backend (Domain)
- ✅ `{Entidade}.cs` - Entidade com atributos para Source Generator

### Frontend (Web)
- ✅ `{Plural}Controller.cs` - Controller MVC com permissões
- ✅ `{Entidade}Dto.cs` - DTO de leitura
- ✅ `Create{Entidade}Request.cs` - Request de criação
- ✅ `Update{Entidade}Request.cs` - Request de atualização
- ✅ `{Plural}ListViewModel.cs` - ViewModel para listagem
- ✅ `I{Entidade}ApiService.cs` - Interface do serviço
- ✅ `{Entidade}ApiService.cs` - Implementação do serviço
- ✅ `Index.cshtml` - View Razor
- ✅ `{entidade}.js` - JavaScript com CrudBase

## 🛠️ Estrutura do Projeto

```
GeradorFullStack/
├── Controllers/
│   └── HomeController.cs          # Controller principal
├── Models/
│   └── Models.cs                  # Todos os models (TabelaInfo, EntityConfig, etc)
├── Services/
│   ├── DatabaseService.cs         # Leitura de metadados do banco
│   ├── CodeGeneratorService.cs    # Gerador legado (Entidade + JSON)
│   └── FullStackGeneratorService.cs # Orquestrador Full-Stack
├── Templates/
│   ├── EntityTemplate.cs          # Template da Entidade
│   ├── WebControllerTemplate.cs   # Template do Controller
│   ├── WebModelsTemplate.cs       # Templates de DTOs/ViewModels
│   ├── WebServicesTemplate.cs     # Templates de Services
│   ├── ViewTemplate.cs            # Template da View
│   └── JavaScriptTemplate.cs      # Template do JavaScript
├── Views/
│   └── Home/
│       └── Index.cshtml           # UI principal
├── Program.cs
├── appsettings.json
└── GeradorFullStack.csproj
```

## 🚀 Como Usar

### 1. Configurar Connection String

Edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SUA_BASE;User Id=usuario;Password=senha;TrustServerCertificate=true"
  }
}
```

### 2. Executar

```bash
dotnet run
```

Acesse: `https://localhost:5001`

### 3. Fluxo de Uso

1. **Selecionar Tabela** - Clique em uma tabela na lista à esquerda
2. **Ver Detalhes** - Analise colunas, FKs, índices
3. **Configurar** - Clique em "Gerar Full-Stack" ou "Configurar Geração"
4. **Personalizar** - Ajuste as seções colapsáveis:
   - 🏷️ Identificação (CdFuncao, Título)
   - 📂 Menu (Módulo, Ícone, Ordem)
   - 📋 Colunas da Listagem
   - 📝 Campos do Formulário
   - 🔗 Relacionamentos (FKs)
   - ⚡ Opções de Geração
5. **Preview** - Visualize o código antes de gerar
6. **Download** - Baixe o ZIP com todos os arquivos

## ⚠️ Alertas Visuais

| Situação | Visual | Comportamento |
|----------|--------|---------------|
| **Sem PK** | 🔴 Vermelho | Geração bloqueada |
| **PK Composta** | 🟡 Amarelo | Aviso de limitações |
| **FK Composta** | 🟠 Laranja | Navegação ignorada |

## 📁 Estrutura do ZIP Gerado

```
{Entidade}_FullStack_{timestamp}.zip
├── Domain/
│   └── Entities/
│       └── {Entidade}.cs
├── Web/
│   ├── Controllers/
│   │   └── {Plural}Controller.cs
│   ├── Models/
│   │   └── {Plural}/
│   │       ├── {Entidade}Dto.cs
│   │       ├── Create{Entidade}Request.cs
│   │       ├── Update{Entidade}Request.cs
│   │       └── {Plural}ListViewModel.cs
│   ├── Services/
│   │   └── {Plural}/
│   │       ├── I{Entidade}ApiService.cs
│   │       └── {Entidade}ApiService.cs
│   ├── Views/
│   │   └── {Plural}/
│   │       └── Index.cshtml
│   └── wwwroot/
│       └── js/
│           └── {plural}/
│               └── {entidade}.js
└── README.md
```

## 🔧 Após Download

### 1. Copiar Arquivos

Copie cada pasta para o local correspondente no projeto RhSensoERP.

### 2. Registrar Service no DI

Em `ServiceCollectionExtensions.cs` ou `Program.cs`:

```csharp
services.AddHttpClient<I{Entidade}ApiService, {Entidade}ApiService>(client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});
```

### 3. Compilar

O Source Generator irá criar automaticamente:
- API Controller
- DTOs do backend
- AutoMapper profiles
- Repository

## 🔄 Geração em Lote

Selecione múltiplas tabelas com os checkboxes e clique em "Gerar Selecionadas" para gerar várias entidades de uma vez.

## 📝 Notas

- A ferramenta mantém compatibilidade com o modo legado (Entidade + JSON)
- Templates migrados do `RhSensoERP.CrudTool`
- Suporta todas as opções do sistema anterior
- UI moderna com tema escuro e seções colapsáveis

---

**Versão:** 3.0  
**Compatível com:** RhSensoERP v2, .NET 8, SQL Server 2019+
