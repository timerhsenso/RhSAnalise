/**
 * CRUD BASE - JavaScript Reutilizável
 * Arquivo: wwwroot/js/crud-base.js
 * 
 * Classe base para gerenciar operações CRUD com DataTables.
 */

class CrudBase {
    /**
     * Construtor da classe CrudBase.
     * @param {Object} config - Configurações do CRUD
     * @param {string} config.controllerName - Nome do controller
     * @param {string} config.tableSelector - Seletor da tabela (padrão: '#tableCrud')
     * @param {Array} config.columns - Definição das colunas do DataTables
     * @param {Object} config.permissions - Permissões do usuário {canCreate, canEdit, canDelete, canView}
     * @param {Object} config.actions - Nomes das actions customizadas
     * @param {Object} config.additionalConfig - Configurações adicionais do DataTables
     */
    constructor(config) {
        this.config = {
            controllerName: config.controllerName,
            tableSelector: config.tableSelector || '#tableCrud',
            columns: config.columns || [],
            permissions: config.permissions || {},
            actions: {
                list: config.actions?.list || 'List',
                create: config.actions?.create || 'Create',
                edit: config.actions?.edit || 'Edit',
                view: config.actions?.view || 'GetById',
                delete: config.actions?.delete || 'Delete',
                deleteMultiple: config.actions?.deleteMultiple || 'DeleteMultiple'
            },
            additionalConfig: config.additionalConfig || {}
        };

        this.table = null;
        this.init();
    }

    /**
     * Inicializa o DataTables e eventos.
     */
    init() {
        const self = this;

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
                    console.error('Erro ao carregar dados:', error);
                    self.showError('Erro ao carregar dados da tabela.');
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
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
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

        // Eventos dos botões
        this.setupEventHandlers();
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
        $(document).on('click', '.btn-view', function () {
            const id = $(this).data('id');
            self.openViewModal(id);
        });

        // Botão de editar
        $(document).on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            self.openEditModal(id);
        });

        // Botão de excluir
        $(document).on('click', '.btn-delete', function () {
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
    }

    /**
     * Abre modal de criação.
     */
    openCreateModal() {
        // Implementar na classe filha
        console.warn('openCreateModal() deve ser implementado na classe filha');
    }

    /**
     * Abre modal de visualização.
     */
    openViewModal(id) {
        // Implementar na classe filha
        console.warn('openViewModal() deve ser implementado na classe filha');
    }

    /**
     * Abre modal de edição.
     */
    openEditModal(id) {
        // Implementar na classe filha
        console.warn('openEditModal() deve ser implementado na classe filha');
    }

    /**
     * Submete o formulário.
     */
    submitForm($form) {
        const self = this;
        const isEdit = $form.find('input[name="Id"]').length > 0;
        const id = $form.find('input[name="Id"]').val();
        const action = isEdit ? self.config.actions.edit : self.config.actions.create;
        const url = isEdit ? `/${self.config.controllerName}/${action}/${id}` : `/${self.config.controllerName}/${action}`;

        // Coleta os dados do formulário
        const formData = {};
        $form.serializeArray().forEach(function (item) {
            formData[item.name] = item.value;
        });

        // Remove o token de validação dos dados
        delete formData.__RequestVerificationToken;

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
                    self.showSuccess(response.message || 'Operação realizada com sucesso!');
                    $('#modalCrud').modal('hide');
                    self.refresh();
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

                if (xhr.status === 400 && xhr.responseJSON) {
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
            'Tem certeza que deseja excluir este registro?',
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
                            self.showSuccess(response.message || 'Registro excluído com sucesso!');
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
            // Tenta pegar o ID de diferentes formas
            ids.push(row.id || row.Id || row.codigo || row.Codigo);
        });

        this.confirmAction(
            `Tem certeza que deseja excluir ${ids.length} registro(s)?`,
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
                            self.showSuccess(response.message || 'Registros excluídos com sucesso!');
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
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').remove();

        $.each(errors, function (field, messages) {
            const $field = $(`[name="${field}"]`);
            $field.addClass('is-invalid');

            const errorHtml = `<div class="invalid-feedback d-block">${messages.join('<br>')}</div>`;
            $field.after(errorHtml);
        });
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
            }
        };
    }
}

// Exporta para uso global
window.CrudBase = CrudBase;
