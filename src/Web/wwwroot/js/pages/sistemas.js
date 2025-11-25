/**
 * ============================================================================
 * SISTEMAS - JavaScript Específico
 * ============================================================================
 * Arquivo: wwwroot/js/pages/sistemas.js
 * 
 * Implementação específica do CRUD de Sistemas.
 * Estende a classe CrudBase com customizações necessárias.
 * 
 * ============================================================================
 */

class SistemaCrud extends CrudBase {
    constructor(config) {
        super(config);
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * Sobrescreve método da classe base.
     */
    enablePrimaryKeyFields(enable) {
        $('#CdSistema').prop('readonly', !enable);
    }

    /**
     * Customização antes de submeter (opcional).
     */
    beforeSubmit(formData, isEdit) {
        // Exemplo: transformar dados antes de enviar
        // formData.CdSistema = formData.CdSistema.toUpperCase();
        return formData;
    }

    /**
     * Customização após submeter (opcional).
     */
    afterSubmit(data, isEdit) {
        // Exemplo: ações após salvar com sucesso
        console.log('Sistema salvo:', data);
    }
}

// Inicialização quando o documento estiver pronto
$(document).ready(function () {
    // Configuração das colunas do DataTables
    const columns = [
        // Coluna de seleção (checkbox)
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'dt-checkboxes-cell',
            render: function () {
                return '<input type="checkbox" class="dt-checkboxes">';
            }
        },
        // Código do Sistema
        {
            data: 'cdSistema',
            name: 'CdSistema',
            title: 'Código'
        },
        // Descrição do Sistema
        {
            data: 'dcSistema',
            name: 'DcSistema',
            title: 'Descrição'
        },
        // Status (Ativo/Inativo)
        {
            data: 'ativo',
            name: 'Ativo',
            title: 'Status',
            render: function (data) {
                return data
                    ? '<span class="badge-status active"><i class="fas fa-check-circle"></i> Ativo</span>'
                    : '<span class="badge-status inactive"><i class="fas fa-times-circle"></i> Inativo</span>';
            }
        },
        // Coluna de ações
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'text-end no-export',
            title: 'Ações',
            render: function (data, type, row) {
                let actions = '<div class="action-buttons">';
                
                // Botão Visualizar
                actions += `<button type="button" class="btn btn-sm btn-info btn-view" data-id="${row.cdSistema}" data-bs-toggle="tooltip" title="Visualizar">
                    <i class="fas fa-eye"></i>
                </button>`;
                
                // Botão Editar
                actions += `<button type="button" class="btn btn-sm btn-warning btn-edit" data-id="${row.cdSistema}" data-bs-toggle="tooltip" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>`;
                
                // Botão Excluir
                actions += `<button type="button" class="btn btn-sm btn-danger btn-delete" data-id="${row.cdSistema}" data-bs-toggle="tooltip" title="Excluir">
                    <i class="fas fa-trash"></i>
                </button>`;
                
                actions += '</div>';
                return actions;
            }
        }
    ];

    // Inicializa o CRUD de Sistemas
    window.sistemaCrud = new SistemaCrud({
        controllerName: 'Sistemas',
        entityName: 'Sistema',
        entityNamePlural: 'Sistemas',
        idField: 'cdSistema',
        tableSelector: '#tableCrud',
        columns: columns,
        permissions: {
            canCreate: window.crudPermissions?.canCreate || false,
            canEdit: window.crudPermissions?.canEdit || false,
            canDelete: window.crudPermissions?.canDelete || false,
            canView: window.crudPermissions?.canView || false
        },
        exportConfig: {
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: 'Sistemas'
        }
    });
});
