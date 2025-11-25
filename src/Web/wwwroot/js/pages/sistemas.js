/**
 * ============================================================================
 * SISTEMAS - JavaScript Específico
 * ============================================================================
 * Arquivo: wwwroot/js/pages/sistemas.js
 * Versão: 2.0 (Corrigido)
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

        // Em modo edição, também adiciona estilo visual
        if (!enable) {
            $('#CdSistema').addClass('bg-light');
        } else {
            $('#CdSistema').removeClass('bg-light');
        }
    }

    /**
     * Customização antes de submeter.
     * Converte código para maiúsculas.
     */
    beforeSubmit(formData, isEdit) {
        // Converte código para maiúsculas
        if (formData.CdSistema) {
            formData.CdSistema = formData.CdSistema.toUpperCase().trim();
        }

        // Garante que o campo Ativo seja booleano
        formData.Ativo = formData.Ativo === true || formData.Ativo === 'true' || formData.Ativo === 'on';

        console.log('📤 Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ Sistema salvo:', data);
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
            width: '40px',
            render: function () {
                return '<input type="checkbox" class="dt-checkboxes form-check-input">';
            }
        },
        // Código do Sistema
        {
            data: 'cdSistema',  // camelCase - como vem da API
            name: 'CdSistema',
            title: 'Código',
            width: '120px',
            render: function (data) {
                return `<strong>${data}</strong>`;
            }
        },
        // Descrição do Sistema
        {
            data: 'dcSistema',  // camelCase - como vem da API
            name: 'DcSistema',
            title: 'Descrição'
        },
        // Status (Ativo/Inativo)
        {
            data: 'ativo',  // camelCase - como vem da API
            name: 'Ativo',
            title: 'Status',
            width: '100px',
            className: 'text-center',
            render: function (data) {
                return data
                    ? '<span class="badge bg-success"><i class="fas fa-check-circle me-1"></i>Ativo</span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times-circle me-1"></i>Inativo</span>';
            }
        },
        // Coluna de ações
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'text-end no-export',
            title: 'Ações',
            width: '130px',
            render: function (data, type, row) {
                let actions = '<div class="btn-group btn-group-sm" role="group">';

                // Botão Visualizar
                if (window.crudPermissions?.canView !== false) {
                    actions += `<button type="button" class="btn btn-info btn-view" 
                        data-id="${row.cdSistema}" 
                        data-bs-toggle="tooltip" 
                        title="Visualizar">
                        <i class="fas fa-eye"></i>
                    </button>`;
                }

                // Botão Editar
                if (window.crudPermissions?.canEdit !== false) {
                    actions += `<button type="button" class="btn btn-warning btn-edit" 
                        data-id="${row.cdSistema}" 
                        data-bs-toggle="tooltip" 
                        title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>`;
                }

                // Botão Excluir
                if (window.crudPermissions?.canDelete !== false) {
                    actions += `<button type="button" class="btn btn-danger btn-delete" 
                        data-id="${row.cdSistema}" 
                        data-bs-toggle="tooltip" 
                        title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>`;
                }

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
        idField: 'cdSistema',  // camelCase - como vem da API
        tableSelector: '#tableCrud',
        columns: columns,
        permissions: {
            canCreate: window.crudPermissions?.canCreate !== false,
            canEdit: window.crudPermissions?.canEdit !== false,
            canDelete: window.crudPermissions?.canDelete !== false,
            canView: window.crudPermissions?.canView !== false
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

    // Máscara para código (apenas maiúsculas e números)
    $('#CdSistema').on('input', function () {
        this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
    });

    console.log('✅ CRUD de Sistemas inicializado');
});