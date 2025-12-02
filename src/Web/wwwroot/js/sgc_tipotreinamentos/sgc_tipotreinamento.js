/**
 * ============================================================================
 * TIPO TREINAMENTO - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/sgc_tipotreinamentos/sgc_tipotreinamento.js
 * Versão: 3.1 (Suporte a PKs de texto)
 * Gerado por: GeradorFullStack v3.1
 * Data: 2025-12-02 19:47:20
 * 
 * Implementação específica do CRUD de Tipo Treinamento.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class SGC_TipoTreinamentoCrud extends CrudBase {
    constructor(config) {
        super(config);
        
        // =====================================================================
        // CORREÇÃO v3.1: Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = null;
        this.isPkTexto = false;
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * CORREÇÃO v3.1: PKs de texto são editáveis apenas na criação.
     */
    enablePrimaryKeyFields(enable) {
        if (!this.isPkTexto) return;
        
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;
        
        if (enable) {
            // Criação: campo editável
            $pkField.prop('readonly', false)
                    .prop('disabled', false)
                    .removeClass('bg-light');
            console.log('✏️ [SGC_TipoTreinamento] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [SGC_TipoTreinamento] Campo PK desabilitado (edição)');
        }
    }

    /**
     * Override: Abre modal para NOVO registro.
     * CORREÇÃO v3.1: Habilita PK de texto na criação.
     */
    openCreateModal() {
        super.openCreateModal();
        
        // Habilita PK de texto para digitação
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(true);
        }
    }

    /**
     * Override: Abre modal para EDIÇÃO.
     * CORREÇÃO v3.1: Desabilita PK de texto na edição.
     */
    async openEditModal(id) {
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(false);
        }
    }

    /**
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        // Converte campos inteiros
        ['validadeEmMeses'].forEach(field => {
            if (formData[field] !== undefined && formData[field] !== '') {
                formData[field] = parseInt(formData[field], 10);
            }
        });

        // Converte campos decimais
        ['cargaHorariaHoras'].forEach(field => {
            if (formData[field] !== undefined && formData[field] !== '') {
                formData[field] = parseFloat(formData[field].toString().replace(',', '.'));
            }
        });

        // Converte checkboxes para 0/1
        ['Ativo'].forEach(field => {
            const key = field.charAt(0).toLowerCase() + field.slice(1);
            const checkbox = document.getElementById(field);
            if (checkbox) {
                formData[key] = checkbox.checked ? 1 : 0;
            } else if (formData[key] === true || formData[key] === 'true' || formData[key] === 'on') {
                formData[key] = 1;
            } else if (formData[key] === false || formData[key] === 'false' || formData[key] === '' || formData[key] === undefined) {
                formData[key] = 0;
            }
        });


        console.log('📤 [SGC_TipoTreinamento] Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [SGC_TipoTreinamento] Registro salvo:', data);
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.id || row.Id || row.id || row.Id || '';
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

    console.log('🔐 [SGC_TipoTreinamento] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['id'] || row['Id'] || row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [SGC_TipoTreinamento] ID vazio para row:', row);
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
                    const id = getCleanId(row, 'id');
                    return `<input type="checkbox" class="dt-checkboxes form-check-input" data-id="${id}">`;
                }
                return '';
            }
        },
        // Codigo
        {
            data: 'codigo',
            name: 'Codigo',
            title: 'Codigo',
            orderable: true,
            className: 'text-left'
        },
        // Descricao
        {
            data: 'descricao',
            name: 'Descricao',
            title: 'Descricao',
            orderable: true,
            className: 'text-left'
        },
        // Carga Horaria Horas
        {
            data: 'cargaHorariaHoras',
            name: 'CargaHorariaHoras',
            title: 'Carga Horaria Horas',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                if (data == null) return '-';
                return 'R$ ' + parseFloat(data).toLocaleString('pt-BR', { minimumFractionDigits: 2 });
            }
        },
        // Validade Em Meses
        {
            data: 'validadeEmMeses',
            name: 'ValidadeEmMeses',
            title: 'Validade Em Meses',
            orderable: true,
            className: 'text-left'
        },
        // Ativo
        {
            data: 'ativo',
            name: 'Ativo',
            title: 'Ativo',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class="badge bg-success"><i class="fas fa-check"></i></span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times"></i></span>';
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
                const id = getCleanId(row, 'id');

                console.log('🔧 [SGC_TipoTreinamento] Renderizando ações | ID:', id, '| Row:', row);

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

    window.sgc_tipotreinamentoCrud = new SGC_TipoTreinamentoCrud({
        controllerName: 'SGC_TipoTreinamentos',
        entityName: 'Tipo Treinamento',
        idField: 'id',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {
            order: [[1, 'asc']]
        }
    });

    // =========================================================================
    // CONTROLE DE TOOLBAR BASEADO EM PERMISSÕES
    // =========================================================================

    // Desabilita botão "Novo" se não pode criar
    if (!window.crudPermissions.canCreate) {
        $('#btnNew').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para criar registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [SGC_TipoTreinamento] Botão "Novo" desabilitado (sem permissão de criação)');
    }

    // Desabilita botão "Excluir Selecionados" se não pode excluir
    if (!window.crudPermissions.canDelete) {
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [SGC_TipoTreinamento] Botão "Excluir Selecionados" desabilitado (sem permissão de exclusão)');
    }

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================

    console.log('✅ CRUD de SGC_TipoTreinamento v3.1 inicializado com permissões:', {
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView,
        pkTexto: false
    });
});
