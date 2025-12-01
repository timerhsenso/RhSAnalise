/**
 * ============================================================================
 * TABELA DE OCUPAÇÃO - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/tcbo1s/tcbo1.js
 * Versão: 2.5 (Seguindo padrão de sistemas.js)
 * Gerado por: GeradorFullStack v3.0
 * Data: 2025-12-01 01:28:26
 * 
 * Implementação específica do CRUD de Tabela de Ocupação.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class Tcbo1Crud extends CrudBase {
    constructor(config) {
        super(config);
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * Sobrescreve método da classe base.
     */
    enablePrimaryKeyFields(enable) {
        // Cdcbo é chave primária, geralmente não editável
        $('#Cdcbo').prop('readonly', !enable);
        
        if (!enable) {
            $('#Cdcbo').addClass('bg-light');
        } else {
            $('#Cdcbo').removeClass('bg-light');
        }
    }

    /**
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {

        console.log('📤 [Tcbo1] Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [Tcbo1] Registro salvo:', data);
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.cdcbo || row.Cdcbo || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
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

    console.log('🔐 [Tcbo1] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['cdcbo'] || row['Cdcbo'] || row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [Tcbo1] ID vazio para row:', row);
        }

        return id;
    }

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
            render: function (data, type, row) {
                // Só mostra checkbox se pode excluir
                if (window.crudPermissions.canDelete) {
                    const id = getCleanId(row, 'cdcbo');
                    return `<input type="checkbox" class="dt-checkboxes form-check-input" data-id="${id}">`;
                }
                return '';
            }
        },
        // Cdcbo
        {
            data: 'cdcbo',
            name: 'Cdcbo',
            title: 'Cdcbo',
            orderable: true,
            className: 'text-left'
        },
        // Dccbo
        {
            data: 'dccbo',
            name: 'Dccbo',
            title: 'Dccbo',
            orderable: true,
            className: 'text-left'
        },
        // Si Nonimo
        {
            data: 'siNonimo',
            name: 'SiNonimo',
            title: 'Si Nonimo',
            orderable: true,
            className: 'text-left'
        },
        // Id
        {
            data: 'id',
            name: 'Id',
            title: 'Id',
            orderable: true,
            className: 'text-left'
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
                const id = getCleanId(row, 'cdcbo');

                console.log('🔧 [Tcbo1] Renderizando ações | ID:', id, '| Row:', row);

                let actions = '<div class="btn-group btn-group-sm" role="group">';

                // Botão Visualizar
                if (window.crudPermissions.canView) {
                    actions += `<button type="button" class="btn btn-info btn-view" 
                        data-id="${id}" 
                        data-bs-toggle="tooltip" 
                        title="Visualizar">
                        <i class="fas fa-eye"></i>
                    </button>`;
                }

                // Botão Editar
                if (window.crudPermissions.canEdit) {
                    actions += `<button type="button" class="btn btn-warning btn-edit" 
                        data-id="${id}" 
                        data-bs-toggle="tooltip" 
                        title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>`;
                }

                // Botão Excluir
                if (window.crudPermissions.canDelete) {
                    actions += `<button type="button" class="btn btn-danger btn-delete" 
                        data-id="${id}" 
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

    window.tcbo1Crud = new Tcbo1Crud({
        controllerName: 'Tcbo1s',
        entityName: 'Tabela de Ocupação',
        entityNamePlural: 'Tabela de Ocupação',
        idField: 'cdcbo',
        tableSelector: '#tableCrud',
        columns: columns,

        // Permissões vindas do backend
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
            filename: 'Tcbo1s'
        }
    });

    // =========================================================================
    // CONTROLE DE BOTÕES DA TOOLBAR
    // =========================================================================

    // Desabilita botão "Novo" se não pode criar
    if (!window.crudPermissions.canCreate) {
        $('#btnCreate, #btnNew').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para criar registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [Tcbo1] Botão "Novo" desabilitado (sem permissão de inclusão)');
    }

    // Desabilita botão "Excluir Selecionados" se não pode excluir
    if (!window.crudPermissions.canDelete) {
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [Tcbo1] Botão "Excluir Selecionados" desabilitado (sem permissão de exclusão)');
    }

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================

    console.log('✅ CRUD de Tcbo1 v2.5 inicializado com permissões:', {
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView
    });
});
