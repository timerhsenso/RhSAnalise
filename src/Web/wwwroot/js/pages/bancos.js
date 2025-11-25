/**
 * ============================================================================
 * BANCOS - JavaScript Específico
 * ============================================================================
 * Arquivo: wwwroot/js/pages/bancos.js
 * 
 * Implementação específica do CRUD de Bancos.
 * Estende a classe CrudBase com customizações necessárias.
 * 
 * ============================================================================
 */

class BancoCrud extends CrudBase {
    constructor(config) {
        super(config);
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * Sobrescreve método da classe base.
     */
    enablePrimaryKeyFields(enable) {
        $('#CodigoBanco').prop('readonly', !enable);
    }

    /**
     * Customização antes de submeter (opcional).
     */
    beforeSubmit(formData, isEdit) {
        // Garante que o código do banco tenha 3 dígitos
        if (formData.CodigoBanco) {
            formData.CodigoBanco = formData.CodigoBanco.padStart(3, '0');
        }
        return formData;
    }

    /**
     * Customização após submeter (opcional).
     */
    afterSubmit(data, isEdit) {
        // Exemplo: ações após salvar com sucesso
        console.log('Banco salvo:', data);
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
        // Código do Banco
        {
            data: 'codigoBanco',
            name: 'CodigoBanco',
            title: 'Código'
        },
        // Nome do Banco
        {
            data: 'nome',
            name: 'Nome',
            title: 'Nome'
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
        // Data de Criação
        {
            data: 'dataCriacao',
            name: 'DataCriacao',
            title: 'Data Criação',
            render: function (data) {
                return data ? new Date(data).toLocaleDateString('pt-BR') : '-';
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
                actions += `<button type="button" class="btn btn-sm btn-info btn-view" data-id="${row.codigoBanco}" data-bs-toggle="tooltip" title="Visualizar">
                    <i class="fas fa-eye"></i>
                </button>`;
                
                // Botão Editar
                actions += `<button type="button" class="btn btn-sm btn-warning btn-edit" data-id="${row.codigoBanco}" data-bs-toggle="tooltip" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>`;
                
                // Botão Excluir
                actions += `<button type="button" class="btn btn-sm btn-danger btn-delete" data-id="${row.codigoBanco}" data-bs-toggle="tooltip" title="Excluir">
                    <i class="fas fa-trash"></i>
                </button>`;
                
                actions += '</div>';
                return actions;
            }
        }
    ];

    // Inicializa o CRUD de Bancos
    window.bancoCrud = new BancoCrud({
        controllerName: 'Bancos',
        entityName: 'Banco',
        entityNamePlural: 'Bancos',
        idField: 'codigoBanco',
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
            filename: 'Bancos'
        }
    });
});
