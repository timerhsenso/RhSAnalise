/**
 * ============================================================================
 * CRUD BASE V2 - JavaScript Reutilizável Aprimorado
 * ============================================================================
 * Arquivo: wwwroot/js/crud-base.js
 * Versão: 2.0
 * Data: 24/11/2025
 * 
 * Classe base para gerenciar operações CRUD com DataTables.
 * Inclui suporte completo para exportação, validação client-side,
 * e loading states consistentes.
 * 
 * ============================================================================
 * USO:
 * ============================================================================
 * 
 * // Em um arquivo JS específico (ex: sistemas.js)
 * class SistemaCrud extends CrudBase {
 *     constructor(config) {
 *         super(config);
 *     }
 * 
 *     // Sobrescrever métodos conforme necessário
 *     buildFormData($form) {
 *         const data = super.buildFormData($form);
 *         // Customizações específicas
 *         return data;
 *     }
 * }
 * 
 * // Inicialização
 * $(document).ready(function() {
 *     window.crudInstance = new SistemaCrud({
 *         controllerName: 'Sistemas',
 *         entityName: 'Sistema',
 *         entityNamePlural: 'Sistemas',
 *         idField: 'cdSistema',
 *         columns: [ ... ],
 *         formFields: [ ... ],
 *         permissions: { ... },
 *         exportConfig: { ... }
 *     });
 * });
 * 
 * ============================================================================
 */

class CrudBase {
    /**
     * Construtor da classe CrudBase.
     * @param {Object} config - Configurações do CRUD
     * @param {string} config.controllerName - Nome do controller (ex: 'Sistemas')
     * @param {string} config.entityName - Nome da entidade no singular (ex: 'Sistema')
     * @param {string} config.entityNamePlural - Nome da entidade no plural (ex: 'Sistemas')
     * @param {string} config.idField - Nome do campo ID (ex: 'cdSistema')
     * @param {string} [config.tableSelector='#tableCrud'] - Seletor da tabela
     * @param {Array} config.columns - Definição das colunas do DataTables
     * @param {Array} [config.formFields=[]] - Definição dos campos do formulário para validação
     * @param {Object} config.permissions - Permissões do usuário {canCreate, canEdit, canDelete, canView}
     * @param {Object} [config.actions] - Nomes das actions customizadas
     * @param {Object} [config.exportConfig] - Configurações de exportação
     * @param {Object} [config.additionalConfig] - Configurações adicionais do DataTables
     * @param {Function} [config.onRowClick] - Callback ao clicar em uma linha
     * @param {Function} [config.beforeSubmit] - Callback antes de submeter formulário
     * @param {Function} [config.afterSubmit] - Callback após submeter formulário
     */
    constructor(config) {
        // Validação de configurações obrigatórias
        this.validateConfig(config);

        this.config = {
            controllerName: config.controllerName,
            entityName: config.entityName,
            entityNamePlural: config.entityNamePlural,
            idField: config.idField,
            tableSelector: config.tableSelector || '#tableCrud',
            columns: config.columns || [],
            formFields: config.formFields || [],
            permissions: config.permissions || {},
            actions: {
                list: config.actions?.list || 'List',
                create: config.actions?.create || 'Create',
                edit: config.actions?.edit || 'Edit',
                view: config.actions?.view || 'GetById',
                delete: config.actions?.delete || 'Delete',
                deleteMultiple: config.actions?.deleteMultiple || 'DeleteMultiple'
            },
            exportConfig: {
                enabled: config.exportConfig?.enabled !== false,
                excel: config.exportConfig?.excel !== false,
                pdf: config.exportConfig?.pdf !== false,
                csv: config.exportConfig?.csv !== false,
                print: config.exportConfig?.print !== false,
                filename: config.exportConfig?.filename || config.entityNamePlural
            },
            additionalConfig: config.additionalConfig || {},
            onRowClick: config.onRowClick,
            beforeSubmit: config.beforeSubmit,
            afterSubmit: config.afterSubmit
        };

        this.table = null;
        this.currentMode = null; // 'create' | 'edit' | 'view'
        this.currentId = null;

        this.init();
    }

    /**
     * Valida configurações obrigatórias.
     */
    validateConfig(config) {
        const required = ['controllerName', 'entityName', 'entityNamePlural', 'idField', 'columns'];
        const missing = required.filter(key => !config[key]);

        if (missing.length > 0) {
            throw new Error(`CrudBase: Configurações obrigatórias ausentes: ${missing.join(', ')}`);
        }
    }

    /**
     * Inicializa o DataTables e eventos.
     */
    init() {
        const self = this;

        // Configuração de botões de exportação
        const buttons = [];
        if (self.config.exportConfig.enabled) {
            if (self.config.exportConfig.excel) {
                buttons.push({
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel me-1"></i> Excel',
                    className: 'btn btn-success btn-sm',
                    exportOptions: {
                        columns: ':visible:not(.no-export)'
                    },
                    filename: self.config.exportConfig.filename
                });
            }
            if (self.config.exportConfig.pdf) {
                buttons.push({
                    extend: 'pdf',
                    text: '<i class="fas fa-file-pdf me-1"></i> PDF',
                    className: 'btn btn-danger btn-sm',
                    exportOptions: {
                        columns: ':visible:not(.no-export)'
                    },
                    filename: self.config.exportConfig.filename,
                    orientation: 'landscape',
                    pageSize: 'A4'
                });
            }
            if (self.config.exportConfig.csv) {
                buttons.push({
                    extend: 'csv',
                    text: '<i class="fas fa-file-csv me-1"></i> CSV',
                    className: 'btn btn-info btn-sm',
                    exportOptions: {
                        columns: ':visible:not(.no-export)'
                    },
                    filename: self.config.exportConfig.filename
                });
            }
            if (self.config.exportConfig.print) {
                buttons.push({
                    extend: 'print',
                    text: '<i class="fas fa-print me-1"></i> Imprimir',
                    className: 'btn btn-secondary btn-sm',
                    exportOptions: {
                        columns: ':visible:not(.no-export)'
                    }
                });
            }
        }

        // Configuração base do DataTables
        const dataTablesConfig = {
            processing: true,
            serverSide: true,
            ajax: {
                url: `/${self.config.controllerName}/${self.config.actions.list}`,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                error: function (xhr, error, thrown) {
                    console.error('Erro ao carregar dados:', error, xhr);
                    self.showError('Erro ao carregar dados da tabela. Verifique sua conexão.');
                }
            },
            columns: self.config.columns,
            language: self.getPortugueseLanguage(),
            responsive: true,
            select: {
                style: 'multi',
                selector: 'td:first-child'
            },
            order: [[1, 'asc']],
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Todos']],
            dom: buttons.length > 0
                ? '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>><"row"<"col-sm-12"B>>rtip'
                : '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
            buttons: buttons,
            drawCallback: function () {
                self.setupTooltips();
                self.updateSelectedCount();
            },
            ...self.config.additionalConfig
        };

        // Inicializa o DataTables
        this.table = $(self.config.tableSelector).DataTable(dataTablesConfig);

        // Eventos de seleção
        this.table.on('select deselect', function () {
            self.updateSelectedCount();
        });

        // Evento de clique em linha (se configurado)
        if (self.config.onRowClick) {
            $(self.config.tableSelector).on('click', 'tbody tr', function () {
                const data = self.table.row(this).data();
                if (data) {
                    self.config.onRowClick(data);
                }
            });
        }

        // Eventos dos botões
        this.setupEventHandlers();

        // Inicializa validação de formulários
        this.setupFormValidation();
    }

    /**
     * Configura os event handlers dos botões.
     */
    setupEventHandlers() {
        const self = this;

        // Botão de criar
        $(document).on('click', '#btnCreate', function () {
            self.openCreateModal();
        });

        // Botão de visualizar
        $(document).on('click', '.btn-view', function (e) {
            e.stopPropagation();
            const id = $(this).data('id');
            self.openViewModal(id);
        });

        // Botão de editar
        $(document).on('click', '.btn-edit', function (e) {
            e.stopPropagation();
            const id = $(this).data('id');
            self.openEditModal(id);
        });

        // Botão de excluir
        $(document).on('click', '.btn-delete', function (e) {
            e.stopPropagation();
            const id = $(this).data('id');
            self.deleteSingle(id);
        });

        // Botão de excluir selecionados
        $(document).on('click', '#btnDeleteSelected', function () {
            self.deleteSelected();
        });

        // Botão de atualizar
        $(document).on('click', '#btnRefresh', function () {
            self.refresh();
        });

        // Submissão de formulário
        $(document).on('submit', '#formCrud', function (e) {
            e.preventDefault();
            self.submitForm($(this));
        });

        // Limpeza ao fechar modal
        $('#modalCrud').on('hidden.bs.modal', function () {
            self.resetForm();
        });
    }

    /**
     * Configura validação de formulários.
     */
    setupFormValidation() {
        const self = this;

        if ($('#formCrud').length > 0) {
            $('#formCrud').validate({
                errorClass: 'is-invalid',
                validClass: 'is-valid',
                errorElement: 'div',
                errorPlacement: function (error, element) {
                    error.addClass('invalid-feedback');
                    element.closest('.mb-3').append(error);
                },
                highlight: function (element) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function (element) {
                    $(element).removeClass('is-invalid').addClass('is-valid');
                },
                submitHandler: function (form) {
                    self.submitForm($(form));
                }
            });
        }
    }

    /**
     * Abre modal de criação.
     */
    openCreateModal() {
        this.currentMode = 'create';
        this.currentId = null;

        $('#modalTitle').text(`Novo ${this.config.entityName}`);
        $('#formCrud')[0].reset();
        $('#Id').val('');

        // Habilita campos de chave primária
        this.enablePrimaryKeyFields(true);

        this.resetValidation();
        $('#modalCrud').modal('show');
    }

    /**
     * Abre modal de visualização.
     */
    openViewModal(id) {
        const self = this;
        this.currentMode = 'view';
        this.currentId = id;

        $.ajax({
            url: `/${self.config.controllerName}/${self.config.actions.view}/${id}`,
            type: 'GET',
            beforeSend: function () {
                self.showLoading();
            },
            success: function (response) {
                self.hideLoading();

                if (response.success && response.data) {
                    self.populateViewModal(response.data);
                    $('#modalView').modal('show');
                } else {
                    self.showError(response.message || 'Erro ao carregar registro.');
                }
            },
            error: function (xhr) {
                self.hideLoading();
                self.showError('Erro ao carregar registro.');
            }
        });
    }

    /**
     * Abre modal de edição.
     */
    openEditModal(id) {
        const self = this;
        this.currentMode = 'edit';
        this.currentId = id;

        $.ajax({
            url: `/${self.config.controllerName}/${self.config.actions.view}/${id}`,
            type: 'GET',
            beforeSend: function () {
                self.showLoading();
            },
            success: function (response) {
                self.hideLoading();

                if (response.success && response.data) {
                    $('#modalTitle').text(`Editar ${self.config.entityName}`);
                    self.populateForm(response.data);

                    // Desabilita campos de chave primária
                    self.enablePrimaryKeyFields(false);

                    self.resetValidation();
                    $('#modalCrud').modal('show');
                } else {
                    self.showError(response.message || 'Erro ao carregar registro.');
                }
            },
            error: function (xhr) {
                self.hideLoading();
                self.showError('Erro ao carregar registro.');
            }
        });
    }

    /**
     * Popula o formulário com dados.
     */
    populateForm(data) {
        $('#Id').val(data[this.config.idField]);

        // Popula campos automaticamente baseado nos nomes
        for (const key in data) {
            const $field = $(`#${key}`);
            if ($field.length > 0) {
                if ($field.is(':checkbox')) {
                    $field.prop('checked', data[key]);
                } else {
                    $field.val(data[key]);
                }
            }
        }
    }

    /**
     * Popula o modal de visualização.
     */
    populateViewModal(data) {
        let html = '<div class="row">';

        for (const key in data) {
            const value = data[key];
            const displayValue = this.formatDisplayValue(key, value);

            html += `
                <div class="col-md-6 mb-3">
                    <strong>${this.formatFieldName(key)}:</strong><br>
                    <span class="text-muted">${displayValue}</span>
                </div>
            `;
        }

        html += '</div>';
        $('#viewContent').html(html);
    }

    /**
     * Formata nome do campo para exibição.
     */
    formatFieldName(fieldName) {
        // Remove prefixos comuns (cd, dc, dt, etc)
        let formatted = fieldName.replace(/^(cd|dc|dt|nr|vl|qt|id)/i, '');

        // Adiciona espaços antes de maiúsculas
        formatted = formatted.replace(/([A-Z])/g, ' $1').trim();

        // Capitaliza primeira letra
        return formatted.charAt(0).toUpperCase() + formatted.slice(1);
    }

    /**
     * Formata valor para exibição.
     */
    formatDisplayValue(key, value) {
        if (value === null || value === undefined) return '-';

        // Booleanos
        if (typeof value === 'boolean') {
            return value
                ? '<span class="badge bg-success">Sim</span>'
                : '<span class="badge bg-danger">Não</span>';
        }

        // Datas
        if (key.toLowerCase().includes('data') || key.toLowerCase().includes('date')) {
            try {
                return new Date(value).toLocaleDateString('pt-BR');
            } catch {
                return value;
            }
        }

        return value;
    }

    /**
     * Habilita/desabilita campos de chave primária.
     */
    enablePrimaryKeyFields(enable) {
        // Implementar na classe filha se necessário
        // Por padrão, desabilita o campo ID principal
        $(`#${this.config.idField}`).prop('readonly', !enable);
    }

    /**
     * Constrói dados do formulário.
     */
    buildFormData($form) {
        const formData = {};

        $form.serializeArray().forEach(function (item) {
            if (item.name !== '__RequestVerificationToken') {
                // Trata checkboxes
                const $field = $form.find(`[name="${item.name}"]`);
                if ($field.is(':checkbox')) {
                    formData[item.name] = $field.is(':checked');
                } else {
                    formData[item.name] = item.value;
                }
            }
        });

        // Adiciona checkboxes não marcados (não aparecem no serializeArray)
        $form.find('input[type="checkbox"]').each(function () {
            const name = $(this).attr('name');
            if (name && !formData.hasOwnProperty(name)) {
                formData[name] = false;
            }
        });

        return formData;
    }

    /**
     * Submete o formulário.
     */
    submitForm($form) {
        const self = this;

        // Valida formulário
        if (!$form.valid()) {
            return;
        }

        const isEdit = self.currentMode === 'edit';
        const id = self.currentId;
        const action = isEdit ? self.config.actions.edit : self.config.actions.create;
        const url = isEdit
            ? `/${self.config.controllerName}/${action}/${id}`
            : `/${self.config.controllerName}/${action}`;

        // Coleta os dados do formulário
        let formData = self.buildFormData($form);

        // Callback antes de submeter
        if (self.config.beforeSubmit) {
            formData = self.config.beforeSubmit(formData, isEdit) || formData;
        }

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            headers: {
                'RequestVerificationToken': $form.find('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function () {
                $form.find('button[type="submit"]').prop('disabled', true);
                self.showLoading();
            },
            success: function (response) {
                self.hideLoading();
                $form.find('button[type="submit"]').prop('disabled', false);

                if (response.success) {
                    self.showSuccess(response.message || `${self.config.entityName} salvo com sucesso!`);
                    $('#modalCrud').modal('hide');
                    self.refresh();

                    // Callback após submeter
                    if (self.config.afterSubmit) {
                        self.config.afterSubmit(response.data, isEdit);
                    }
                } else {
                    if (response.errors) {
                        self.displayValidationErrors(response.errors);
                    } else {
                        self.showError(response.message || 'Erro ao salvar registro.');
                    }
                }
            },
            error: function (xhr) {
                self.hideLoading();
                $form.find('button[type="submit"]').prop('disabled', false);

                if (xhr.status === 400 && xhr.responseJSON && xhr.responseJSON.errors) {
                    self.displayValidationErrors(xhr.responseJSON.errors);
                } else {
                    self.showError('Erro ao salvar registro.');
                }
            }
        });
    }

    /**
     * Exclui um único registro.
     */
    deleteSingle(id) {
        const self = this;

        this.confirmAction(
            `Tem certeza que deseja excluir este ${self.config.entityName.toLowerCase()}?`,
            'Esta ação não pode ser desfeita!',
            function () {
                $.ajax({
                    url: `/${self.config.controllerName}/${self.config.actions.delete}/${id}`,
                    type: 'POST',
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    beforeSend: function () {
                        self.showLoading();
                    },
                    success: function (response) {
                        self.hideLoading();

                        if (response.success) {
                            self.showSuccess(response.message || `${self.config.entityName} excluído com sucesso!`);
                            self.refresh();
                        } else {
                            self.showError(response.message || 'Erro ao excluir registro.');
                        }
                    },
                    error: function (xhr) {
                        self.hideLoading();
                        self.showError('Erro ao excluir registro.');
                    }
                });
            }
        );
    }

    /**
     * Exclui múltiplos registros selecionados.
     */
    deleteSelected() {
        const self = this;
        const selectedRows = this.table.rows({ selected: true }).data();

        if (selectedRows.length === 0) {
            this.showWarning('Nenhum registro selecionado.');
            return;
        }

        const ids = [];
        selectedRows.each(function (row) {
            ids.push(row[self.config.idField]);
        });

        this.confirmAction(
            `Tem certeza que deseja excluir ${ids.length} ${ids.length > 1 ? self.config.entityNamePlural.toLowerCase() : self.config.entityName.toLowerCase()}?`,
            'Esta ação não pode ser desfeita!',
            function () {
                $.ajax({
                    url: `/${self.config.controllerName}/${self.config.actions.deleteMultiple}`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(ids),
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    beforeSend: function () {
                        self.showLoading();
                    },
                    success: function (response) {
                        self.hideLoading();

                        if (response.success) {
                            self.showSuccess(response.message || `${ids.length} ${ids.length > 1 ? self.config.entityNamePlural.toLowerCase() : self.config.entityName.toLowerCase()} excluídos com sucesso!`);
                            self.refresh();
                        } else {
                            self.showError(response.message || 'Erro ao excluir registros.');
                        }
                    },
                    error: function (xhr) {
                        self.hideLoading();
                        self.showError('Erro ao excluir registros.');
                    }
                });
            }
        );
    }

    /**
     * Atualiza a tabela.
     */
    refresh() {
        this.table.ajax.reload(null, false);
        this.showInfo('Tabela atualizada.');
    }

    /**
     * Atualiza contador de registros selecionados.
     */
    updateSelectedCount() {
        const count = this.table.rows({ selected: true }).count();
        $('#selectedCount').text(count);

        if (count > 0) {
            $('#btnDeleteSelected').prop('disabled', false);
        } else {
            $('#btnDeleteSelected').prop('disabled', true);
        }
    }

    /**
     * Exibe erros de validação no formulário.
     */
    displayValidationErrors(errors) {
        this.resetValidation();

        $.each(errors, function (field, messages) {
            const $field = $(`[name="${field}"]`);
            $field.addClass('is-invalid');

            const errorHtml = `<div class="invalid-feedback d-block">${Array.isArray(messages) ? messages.join('<br>') : messages}</div>`;
            $field.closest('.mb-3').append(errorHtml);
        });
    }

    /**
     * Reseta validação do formulário.
     */
    resetValidation() {
        $('.is-invalid').removeClass('is-invalid');
        $('.is-valid').removeClass('is-valid');
        $('.invalid-feedback').remove();
        $('.valid-feedback').remove();
    }

    /**
     * Reseta formulário.
     */
    resetForm() {
        $('#formCrud')[0].reset();
        this.resetValidation();
        this.currentMode = null;
        this.currentId = null;
    }

    /**
     * Configuração de tooltips Bootstrap.
     */
    setupTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    /**
     * Confirmação de ação com SweetAlert2.
     */
    confirmAction(title, text, callback) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#e74a3b',
            cancelButtonColor: '#858796',
            confirmButtonText: 'Sim, confirmar!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                callback();
            }
        });
    }

    /**
     * Notificações Toast.
     */
    showSuccess(message) {
        this.showToast(message, 'success');
    }

    showError(message) {
        this.showToast(message, 'error');
    }

    showWarning(message) {
        this.showToast(message, 'warning');
    }

    showInfo(message) {
        this.showToast(message, 'info');
    }

    showToast(message, type) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: type,
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }

    /**
     * Loading overlay.
     */
    showLoading() {
        if ($('#crudLoading').length === 0) {
            const loadingHtml = `
                <div id="crudLoading" class="crud-loading active">
                    <div class="crud-loading-spinner"></div>
                </div>
            `;
            $('body').append(loadingHtml);
        } else {
            $('#crudLoading').addClass('active');
        }
    }

    hideLoading() {
        $('#crudLoading').removeClass('active');
    }

    /**
     * Tradução DataTables para português.
     */
    getPortugueseLanguage() {
        return {
            "sEmptyTable": "Nenhum registro encontrado",
            "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
            "sInfoFiltered": "(filtrado de _MAX_ registros no total)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Mostrar _MENU_ registros por página",
            "sLoadingRecords": "Carregando...",
            "sProcessing": "Processando...",
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar:",
            "oPaginate": {
                "sNext": "Próximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeiro",
                "sLast": "Último"
            },
            "oAria": {
                "sSortAscending": ": Ordenar colunas de forma ascendente",
                "sSortDescending": ": Ordenar colunas de forma descendente"
            },
            "select": {
                "rows": {
                    "_": "Selecionado %d linhas",
                    "0": "Nenhuma linha selecionada",
                    "1": "Selecionado 1 linha"
                }
            },
            "buttons": {
                "copy": "Copiar",
                "copyTitle": "Copiado para área de transferência",
                "copySuccess": {
                    "_": "%d linhas copiadas",
                    "1": "1 linha copiada"
                },
                "excel": "Excel",
                "pdf": "PDF",
                "print": "Imprimir",
                "csv": "CSV"
            }
        };
    }
}

// Exporta para uso global
window.CrudBase = CrudBase;
