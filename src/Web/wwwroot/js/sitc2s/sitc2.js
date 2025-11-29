/**
 * ============================================================================
 * SITUAÇÃO DE FREQUÊNCIA - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/sitc2s/sitc2.js
 * Versão: 2.5 (Seguindo padrão de sistemas.js)
 * Gerado por: RhSensoERP.CrudTool v2.5
 * Data: 2025-11-28 21:48:47
 * 
 * Implementação específica do CRUD de Situação de Frequência.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class Sitc2Crud extends CrudBase {
    constructor(config) {
        super(config);
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * Sobrescreve método da classe base.
     */
    enablePrimaryKeyFields(enable) {
        // Id é Guid gerado automaticamente, geralmente não editável
        $('#Id').prop('readonly', !enable);
        
        if (!enable) {
            $('#Id').addClass('bg-light');
        } else {
            $('#Id').removeClass('bg-light');
        }
    }

    /**
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        // Converte campos inteiros
        ['cdEmpresa', 'cdFilial', 'flSituacao', 'flProcessado', 'flImportado'].forEach(field => {
            if (formData[field] !== undefined && formData[field] !== '') {
                formData[field] = parseInt(formData[field], 10);
            }
        });

        // Trata IdFuncionario nullable (Guid)
        if (formData.idFuncionario === '' || formData.idFuncionario === undefined) {
            formData.idFuncionario = null;
        }

        // Trata campos de data opcionais
        ['dtImportacao', 'dtProcessamento'].forEach(field => {
            if (formData[field] === '') {
                formData[field] = null;
            }
        });


        console.log('📤 [Sitc2] Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [Sitc2] Registro salvo:', data);
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

    console.log('🔐 [Sitc2] Permissões ativas:', window.crudPermissions);

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
            console.warn('⚠️ [Sitc2] ID vazio para row:', row);
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
        // Código Empresa
        {
            data: 'cdEmpresa',
            name: 'CdEmpresa',
            title: 'Código Empresa',
            width: '120px',
            orderable: true,
            className: 'text-right'
        },
        // Código Filial
        {
            data: 'cdFilial',
            name: 'CdFilial',
            title: 'Código Filial',
            width: '120px',
            orderable: true,
            className: 'text-right'
        },
        // Matrícula
        {
            data: 'noMatric',
            name: 'NoMatric',
            title: 'Matrícula',
            width: '100px',
            orderable: true,
            className: 'text-left'
        },
        // Data Frequência
        {
            data: 'dtFrequen',
            name: 'DtFrequen',
            title: 'Data Frequência',
            width: '120px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR');
            }
        },
        // Situação
        {
            data: 'flSituacao',
            name: 'FlSituacao',
            title: 'Situação',
            width: '100px',
            orderable: true,
            className: 'text-center'
        },
        // Código Usuário
        {
            data: 'cdUsuario',
            name: 'CdUsuario',
            title: 'Código Usuário',
            width: '120px',
            orderable: true,
            className: 'text-left'
        },
        // Data Última Movimentação
        {
            data: 'dtUltMov',
            name: 'DtUltMov',
            title: 'Data Última Movimentação',
            width: '150px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
            }
        },
        // Processado
        {
            data: 'flProcessado',
            name: 'FlProcessado',
            title: 'Processado',
            width: '100px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class="badge bg-success"><i class="fas fa-check"></i></span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times"></i></span>';
            }
        },
        // Importado
        {
            data: 'flImportado',
            name: 'FlImportado',
            title: 'Importado',
            width: '100px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class="badge bg-success"><i class="fas fa-check"></i></span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times"></i></span>';
            }
        },
        // Data Importação
        {
            data: 'dtImportacao',
            name: 'DtImportacao',
            title: 'Data Importação',
            width: '150px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
            }
        },
        // Data Processamento
        {
            data: 'dtProcessamento',
            name: 'DtProcessamento',
            title: 'Data Processamento',
            width: '150px',
            orderable: true,
            className: 'text-center',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
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

                console.log('🔧 [Sitc2] Renderizando ações | ID:', id, '| Row:', row);

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

    window.sitc2Crud = new Sitc2Crud({
        controllerName: 'Sitc2s',
        entityName: 'Situação de Frequência',
        entityNamePlural: 'Situação de Frequência',
        idField: 'id',
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
            filename: 'Sitc2s'
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

        console.log('🔒 [Sitc2] Botão "Novo" desabilitado (sem permissão de inclusão)');
    }

    // Desabilita botão "Excluir Selecionados" se não pode excluir
    if (!window.crudPermissions.canDelete) {
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [Sitc2] Botão "Excluir Selecionados" desabilitado (sem permissão de exclusão)');
    }

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================

    console.log('✅ CRUD de Sitc2 v2.5 inicializado com permissões:', {
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView
    });
});
