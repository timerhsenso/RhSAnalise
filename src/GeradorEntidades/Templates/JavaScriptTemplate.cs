// =============================================================================
// GERADOR FULL-STACK v3.1 - JAVASCRIPT TEMPLATE
// Baseado em RhSensoERP.CrudTool v2.5
// CORREÇÃO v3.1: PKs de texto (código manual) com controle de readonly na edição
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera JavaScript que estende a classe CrudBase existente.
/// 
/// CORREÇÕES v3.1:
/// - PKs de texto aparecem no formulário
/// - PKs de texto são readonly na edição, editáveis na criação
/// - Método enablePrimaryKeyFields() controla o estado
/// - Gera classe que extends CrudBase (padrão sistemas.js)
/// - Usa window.crudPermissions (não pagePermissions)
/// </summary>
public static class JavaScriptTemplate
{
    /// <summary>
    /// Gera arquivo JavaScript que estende CrudBase.
    /// </summary>
    public static GeneratedFile Generate(EntityConfig entity)
    {
        var columns = GenerateColumns(entity);
        var beforeSubmitLogic = GenerateBeforeSubmitLogic(entity);
        var idField = entity.PrimaryKey?.Name ?? "Id";
        var idFieldLower = char.ToLower(idField[0]) + idField.Substring(1);

        // Verifica se a PK é de texto (não Identity e não Guid)
        var isPkTexto = entity.PrimaryKey != null && !entity.PrimaryKey.IsIdentity && !entity.PrimaryKey.IsGuid;
        var pkFieldId = entity.PrimaryKey?.Name ?? "Id";

        var content = $@"/**
 * ============================================================================
 * {entity.DisplayName.ToUpper()} - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/{entity.PluralNameLower}/{entity.NameLower}.js
 * Versão: 3.1 (Suporte a PKs de texto)
 * Gerado por: GeradorFullStack v3.1
 * Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
 * 
 * Implementação específica do CRUD de {entity.DisplayName}.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class {entity.Name}Crud extends CrudBase {{
    constructor(config) {{
        super(config);
        
        // =====================================================================
        // CORREÇÃO v3.1: Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = {(isPkTexto ? $"'{pkFieldId}'" : "null")};
        this.isPkTexto = {(isPkTexto ? "true" : "false")};
    }}

    /**
     * Habilita/desabilita campos de chave primária.
     * CORREÇÃO v3.1: PKs de texto são editáveis apenas na criação.
     */
    enablePrimaryKeyFields(enable) {{
        if (!this.isPkTexto) return;
        
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;
        
        if (enable) {{
            // Criação: campo editável
            $pkField.prop('readonly', false)
                    .prop('disabled', false)
                    .removeClass('bg-light');
            console.log('✏️ [{entity.Name}] Campo PK habilitado para edição (criação)');
        }} else {{
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [{entity.Name}] Campo PK desabilitado (edição)');
        }}
    }}

    /**
     * Override: Abre modal para NOVO registro.
     * CORREÇÃO v3.1: Habilita PK de texto na criação.
     */
    openCreateModal() {{
        super.openCreateModal();
        
        // Habilita PK de texto para digitação
        if (this.isPkTexto) {{
            this.enablePrimaryKeyFields(true);
        }}
    }}

    /**
     * Override: Abre modal para EDIÇÃO.
     * CORREÇÃO v3.1: Desabilita PK de texto na edição.
     */
    async openEditModal(id) {{
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {{
            this.enablePrimaryKeyFields(false);
        }}
    }}

    /**
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {{
{beforeSubmitLogic}
        console.log('📤 [{entity.Name}] Dados a enviar:', formData);
        return formData;
    }}

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {{
        console.log('✅ [{entity.Name}] Registro salvo:', data);
    }}

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {{
        const id = row[this.config.idField] || row.{idFieldLower} || row.{entity.PrimaryKey?.Name ?? "Id"} || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
    }}
}}

// Inicialização quando o documento estiver pronto
$(document).ready(function () {{

    // =========================================================================
    // VERIFICAÇÃO DE PERMISSÕES
    // =========================================================================

    // Verifica se as permissões foram injetadas pela View
    if (typeof window.crudPermissions === 'undefined') {{
        console.error('❌ Permissões não foram carregadas! Usando valores padrão.');
        window.crudPermissions = {{
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        }};
    }}

    console.log('🔐 [{entity.Name}] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {{
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['{idFieldLower}'] || row['{entity.PrimaryKey?.Name ?? "Id"}'] || row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {{
            console.warn('⚠️ [{entity.Name}] ID vazio para row:', row);
        }}

        return id;
    }}

    // =========================================================================
    // CONFIGURAÇÃO DAS COLUNAS DO DATATABLES
    // =========================================================================

    const columns = [
        // Coluna de seleção (checkbox)
        {{
            data: null,
            orderable: false,
            searchable: false,
            className: 'dt-checkboxes-cell',
            width: '40px',
            render: function (data, type, row) {{
                // Só mostra checkbox se pode excluir
                if (window.crudPermissions.canDelete) {{
                    const id = getCleanId(row, '{idFieldLower}');
                    return `<input type=""checkbox"" class=""dt-checkboxes form-check-input"" data-id=""${{id}}"">`;
                }}
                return '';
            }}
        }},
{columns}
        // Coluna de ações
        {{
            data: null,
            orderable: false,
            searchable: false,
            className: 'text-end no-export',
            title: 'Ações',
            width: '130px',
            render: function (data, type, row) {{
                const id = getCleanId(row, '{idFieldLower}');

                console.log('🔧 [{entity.Name}] Renderizando ações | ID:', id, '| Row:', row);

                let actions = '<div class=""btn-group btn-group-sm"" role=""group"">';

                // Botão Visualizar
                if (window.crudPermissions.canView) {{
                    actions += `<button type=""button"" class=""btn btn-info btn-view"" 
                        data-id=""${{id}}"" 
                        data-bs-toggle=""tooltip"" 
                        title=""Visualizar"">
                        <i class=""fas fa-eye""></i>
                    </button>`;
                }}

                // Botão Editar
                if (window.crudPermissions.canEdit) {{
                    actions += `<button type=""button"" class=""btn btn-warning btn-edit"" 
                        data-id=""${{id}}"" 
                        data-bs-toggle=""tooltip"" 
                        title=""Editar"">
                        <i class=""fas fa-edit""></i>
                    </button>`;
                }}

                // Botão Excluir
                if (window.crudPermissions.canDelete) {{
                    actions += `<button type=""button"" class=""btn btn-danger btn-delete"" 
                        data-id=""${{id}}"" 
                        data-bs-toggle=""tooltip"" 
                        title=""Excluir"">
                        <i class=""fas fa-trash""></i>
                    </button>`;
                }}

                actions += '</div>';
                return actions;
            }}
        }}
    ];

    // =========================================================================
    // INICIALIZAÇÃO DO CRUD
    // =========================================================================

    window.{entity.NameLower}Crud = new {entity.Name}Crud({{
        controllerName: '{entity.PluralName}',
        entityName: '{entity.DisplayName}',
        idField: '{idFieldLower}',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {{
            order: [[1, 'asc']]
        }}
    }});

    // =========================================================================
    // CONTROLE DE TOOLBAR BASEADO EM PERMISSÕES
    // =========================================================================

    // Desabilita botão ""Novo"" se não pode criar
    if (!window.crudPermissions.canCreate) {{
        $('#btnNew').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para criar registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [{entity.Name}] Botão ""Novo"" desabilitado (sem permissão de criação)');
    }}

    // Desabilita botão ""Excluir Selecionados"" se não pode excluir
    if (!window.crudPermissions.canDelete) {{
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [{entity.Name}] Botão ""Excluir Selecionados"" desabilitado (sem permissão de exclusão)');
    }}

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================

    console.log('✅ CRUD de {entity.Name} v3.1 inicializado com permissões:', {{
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView,
        pkTexto: {(isPkTexto ? "true" : "false")}
    }});
}});
";

        return new GeneratedFile
        {
            FileName = $"{entity.NameLower}.js",
            RelativePath = $"Web/wwwroot/js/{entity.PluralNameLower}/{entity.NameLower}.js",
            Content = content,
            FileType = "JavaScript"
        };
    }

    #region Helper Methods

    /// <summary>
    /// Gera configuração das colunas do DataTable.
    /// </summary>
    private static string GenerateColumns(EntityConfig entity)
    {
        var sb = new StringBuilder();

        var listProps = entity.Properties
            .Where(p => p.List?.Show == true)
            .OrderBy(p => p.List!.Order)
            .ToList();

        foreach (var prop in listProps)
        {
            var align = prop.List!.Align ?? "left";
            var sortable = prop.List.Sortable ? "true" : "false";
            var format = prop.List.Format ?? "text";
            var width = !string.IsNullOrEmpty(prop.List.Width) ? $"\n            width: '{prop.List.Width}'," : "";

            // Nome da propriedade em camelCase para o JSON
            var dataName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);

            string render = format switch
            {
                "date" => $@",
            render: function (data) {{
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR');
            }}",
                "datetime" => $@",
            render: function (data) {{
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', {{ hour: '2-digit', minute: '2-digit' }});
            }}",
                "currency" => $@",
            render: function (data) {{
                if (data == null) return '-';
                return 'R$ ' + parseFloat(data).toLocaleString('pt-BR', {{ minimumFractionDigits: 2 }});
            }}",
                "percentage" => $@",
            render: function (data) {{
                if (data == null) return '-';
                return parseFloat(data).toFixed(2) + '%';
            }}",
                "boolean" => $@",
            render: function (data) {{
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class=""badge bg-success""><i class=""fas fa-check""></i></span>'
                    : '<span class=""badge bg-secondary""><i class=""fas fa-times""></i></span>';
            }}",
                _ => ""
            };

            // ✅ CORREÇÃO: Vírgula sempre no final de cada coluna
            sb.AppendLine($@"        // {prop.DisplayName}
        {{
            data: '{dataName}',
            name: '{prop.Name}',
            title: '{prop.DisplayName}',{width}
            orderable: {sortable},
            className: 'text-{align}'{render}
        }},");
        }

        // Remove a última vírgula e quebra de linha, mas mantém uma vírgula final
        // para separar da coluna de ações
        var result = sb.ToString().TrimEnd('\r', '\n');

        return result;
    }

    /// <summary>
    /// Gera lógica do beforeSubmit para tratamento de dados.
    /// CORREÇÃO v3.1: Inclui PKs de texto no tratamento.
    /// </summary>
    private static string GenerateBeforeSubmitLogic(EntityConfig entity)
    {
        var sb = new StringBuilder();

        // =========================================================================
        // CORREÇÃO v3.1: PKs de texto que são inteiros devem ser tratadas
        // =========================================================================

        // Campos inteiros (inclui PKs de texto que são int)
        var intProps = entity.Properties
            .Where(p => (p.IsInt || p.IsLong) && p.Form?.Show == true)
            .Where(p => !p.IsPrimaryKey || (!p.IsIdentity && !p.IsGuid)) // Inclui PKs de texto
            .ToList();

        if (intProps.Any())
        {
            var intFieldNames = string.Join(", ", intProps.Select(p => $"'{char.ToLower(p.Name[0]) + p.Name.Substring(1)}'"));
            sb.AppendLine($@"        // Converte campos inteiros
        [{intFieldNames}].forEach(field => {{
            if (formData[field] !== undefined && formData[field] !== '') {{
                formData[field] = parseInt(formData[field], 10);
            }}
        }});
");
        }

        // Campos decimais
        var decimalProps = entity.Properties
            .Where(p => p.IsDecimal && p.Form?.Show == true)
            .ToList();

        if (decimalProps.Any())
        {
            var decFieldNames = string.Join(", ", decimalProps.Select(p => $"'{char.ToLower(p.Name[0]) + p.Name.Substring(1)}'"));
            sb.AppendLine($@"        // Converte campos decimais
        [{decFieldNames}].forEach(field => {{
            if (formData[field] !== undefined && formData[field] !== '') {{
                formData[field] = parseFloat(formData[field].toString().replace(',', '.'));
            }}
        }});
");
        }

        // Campos booleanos/checkbox (int 0/1)
        var boolProps = entity.Properties
            .Where(p => p.IsBool && p.Form?.Show == true)
            .ToList();

        if (boolProps.Any())
        {
            var boolFieldNames = string.Join(", ", boolProps.Select(p => $"'{p.Name}'"));
            sb.AppendLine($@"        // Converte checkboxes para 0/1
        [{boolFieldNames}].forEach(field => {{
            const key = field.charAt(0).toLowerCase() + field.slice(1);
            const checkbox = document.getElementById(field);
            if (checkbox) {{
                formData[key] = checkbox.checked ? 1 : 0;
            }} else if (formData[key] === true || formData[key] === 'true' || formData[key] === 'on') {{
                formData[key] = 1;
            }} else if (formData[key] === false || formData[key] === 'false' || formData[key] === '' || formData[key] === undefined) {{
                formData[key] = 0;
            }}
        }});
");
        }

        // Campos Guid nullable (não PK)
        var guidProps = entity.Properties
            .Where(p => p.IsGuid && p.Form?.Show == true && !p.IsPrimaryKey && p.IsNullable)
            .ToList();

        if (guidProps.Any())
        {
            foreach (var prop in guidProps)
            {
                var propNameLower = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        // Trata {prop.Name} nullable (Guid)
        if (formData.{propNameLower} === '' || formData.{propNameLower} === undefined) {{
            formData.{propNameLower} = null;
        }}
");
            }
        }

        // Campos DateTime opcionais
        var dateProps = entity.Properties
            .Where(p => p.IsDateTime && p.Form?.Show == true && p.IsNullable)
            .ToList();

        if (dateProps.Any())
        {
            var dateFieldNames = string.Join(", ", dateProps.Select(p => $"'{char.ToLower(p.Name[0]) + p.Name.Substring(1)}'"));
            sb.AppendLine($@"        // Trata campos de data opcionais
        [{dateFieldNames}].forEach(field => {{
            if (formData[field] === '') {{
                formData[field] = null;
            }}
        }});
");
        }

        // =========================================================================
        // CORREÇÃO v3.1: Garante que PK de texto seja incluída no formData
        // =========================================================================
        var pkTexto = entity.PrimaryKey != null && !entity.PrimaryKey.IsIdentity && !entity.PrimaryKey.IsGuid
            ? entity.PrimaryKey
            : null;

        if (pkTexto != null && pkTexto.IsString)
        {
            var pkNameLower = char.ToLower(pkTexto.Name[0]) + pkTexto.Name.Substring(1);
            sb.AppendLine($@"        // Garante que PK de texto seja string trimada
        if (formData.{pkNameLower}) {{
            formData.{pkNameLower} = String(formData.{pkNameLower}).trim();
        }}
");
        }

        return sb.ToString();
    }

    #endregion
}