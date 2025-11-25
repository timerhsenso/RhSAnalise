/**
 * ============================================================================
 * SISTEMAS - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/pages/sistemas.js
 * Versão: 3.0 (Com controle de botões baseado em permissões)
 * 
 * Implementação específica do CRUD de Sistemas.
 * Estende a classe CrudBase com customizações necessárias.
 * 
 * NOVO: Desabilita botões automaticamente com base nas permissões do usuário.
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

    // =========================================================================
    // VERIFICAÇÃO DE PERMISSÕES
    // =========================================================================
    
    // Verifica se as permissões foram injetadas pela View
    if (typeof window.crudPermissions === 'undefined') {
        console.error('❌ Permissões não foram carregadas! Usando valores padrão.');
        window.crudPermissions = {
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        };
    }

    console.log('🔐 Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // CONFIGURAÇÃO DAS COLUNAS DO DATATABLES
    // =========================================================================
    
    const columns = [
        // Coluna de seleção (checkbox)
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'dt-checkboxes-cell',
            width: '40px',
            render: function () {
                // ⭐ Só mostra checkbox se pode excluir
                if (window.crudPermissions.canDelete) {
                    return '<input type="checkbox" class="dt-checkboxes form-check-input">';
                }
                return '';
            }
        },
        // Código do Sistema
        {
            data: 'cdSistema',
            name: 'CdSistema',
            title: 'Código',
            width: '120px',
            render: function (data) {
                return `<strong>${data}</strong>`;
            }
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

                // ⭐ Botão Visualizar (sempre visível se pode consultar)
                if (window.crudPermissions.canView) {
                    actions += `<button type="button" class="btn btn-info btn-view" 
                        data-id="${row.cdSistema}" 
                        data-bs-toggle="tooltip" 
                        title="Visualizar">
                        <i class="fas fa-eye"></i>
                    </button>`;
                }

                // ⭐ Botão Editar (só aparece se pode editar)
                if (window.crudPermissions.canEdit) {
                    actions += `<button type="button" class="btn btn-warning btn-edit" 
                        data-id="${row.cdSistema}" 
                        data-bs-toggle="tooltip" 
                        title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>`;
                }

                // ⭐ Botão Excluir (só aparece se pode excluir)
                if (window.crudPermissions.canDelete) {
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

    // =========================================================================
    // INICIALIZAÇÃO DO CRUD
    // =========================================================================
    
    window.sistemaCrud = new SistemaCrud({
        controllerName: 'Sistemas',
        entityName: 'Sistema',
        entityNamePlural: 'Sistemas',
        idField: 'cdSistema',
        tableSelector: '#tableCrud',
        columns: columns,
        
        // ⭐ Permissões vindas do backend
        permissions: {
            canCreate: window.crudPermissions.canCreate,
            canEdit: window.crudPermissions.canEdit,
            canDelete: window.crudPermissions.canDelete,
            canView: window.crudPermissions.canView
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

    // =========================================================================
    // CONTROLE DE BOTÕES DA TOOLBAR
    // =========================================================================
    
    // ⭐ Desabilita botão "Novo" se não pode criar
    if (!window.crudPermissions.canCreate) {
        $('#btnNew').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para criar registros')
            .css('cursor', 'not-allowed');
        
        console.log('🔒 Botão "Novo" desabilitado (sem permissão de inclusão)');
    }

    // ⭐ Desabilita botão "Excluir Selecionados" se não pode excluir
    if (!window.crudPermissions.canDelete) {
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');
        
        console.log('🔒 Botão "Excluir Selecionados" desabilitado (sem permissão de exclusão)');
    }

    // =========================================================================
    // MÁSCARAS E VALIDAÇÕES
    // =========================================================================
    
    // Máscara para código (apenas maiúsculas e números)
    $('#CdSistema').on('input', function () {
        this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
    });

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================
    
    console.log('✅ CRUD de Sistemas inicializado com permissões:', {
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView
    });
});
