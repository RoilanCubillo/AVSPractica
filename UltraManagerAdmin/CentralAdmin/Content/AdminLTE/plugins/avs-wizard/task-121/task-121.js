/**
 * Tarea de Cambio de Precios: Dinámicas
 */

var StatusT121 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    errorDate: false,
    errorOrderDate: false,
    errorPercentage: false,
    errorSupplier: false,
}, DataTask121 = {
    fields: [],
    columns: [
        { title: '', visible: false, },
        { title: 'Cód. Producto' },
        { title: 'Descripción' },
        { title: 'Cód. Proveedor' },
        { title: 'Proveedor' },
        { title: 'Precio Actual', render: function (data, type, row, meta) {
                return type == 'display' ? numberToMoney(data) : data;
            } },
        { title: 'Precio Nuevo Oferta', render: function (data, type, row, meta) {
                return type == 'display' ? numberToMoney(data) : data;
            } },
        { title: 'Cantidad Oferta' },
        { title: 'Descuento en Factura(%)', render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT121.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            } },
        { title: 'Descuento para Cliente(%)', render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT121.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            } },
        { title: 'Fecha Inicio', render: function (data, type, row, meta) {
                if (isNaN(moment(data, "DD/MM/YYYY").toDate())) {
                    StatusT121.errorDate = true;
                    return '<span class="badge bg-danger">' + data + ' ERRONEA</span>'
                } else return data;
            } },
        { title: 'Fecha Fin', render: function (data, type, row, meta) {
                if (isNaN(moment(data, "DD/MM/YYYY").toDate())) {
                    StatusT121.errorDate = true;
                    return '<span class="badge bg-danger">' + data + ' ERRONEA</span>'
                } else return data;
            } },
        { title: 'Estado' },
    ],
    columnsFile: [
        'Codigo de Producto',
        'Codigo de Proveedor',
        'Precio Oferta',
        'Cantidad Oferta',
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)',
        'Fecha Inicio (dd/MM/yyyy)',
        'Fecha Fin (dd/MM/yyyy)'
    ]
    };

function T121InitStatus() {
    StatusT121.applyChanges = false;
    StatusT121.availableGetData = true;
    StatusT121.gettingData = false;
    StatusT121.dataErrors = false;
    StatusT121.isGetted = false;
    StatusT121.dataEmpty = false;
    StatusT121.noFind = false;
    StatusT121.supplierNotFound = false;
    StatusT121.errorDate = false;
    StatusT121.errorOrderDate = false;
    StatusT121.errorPercentage = false;
    StatusT121.errorSupplier = false;
}

function T121CsvToTaskTable() {
    DataTask121.fields = csvData.map(x => {
        var supplier = taskList.suppliers.find(s => s.Code === x[1]);
        if (supplier)
            return ['', x[0], '', x[1], supplier.Name, '-', +x[2], +x[3], +x[4], +x[5], x[6], x[7], '<span class="badge bg-yellow">NO CARGADO</span>']
        else {
            if (x[1] !== '') {
                StatusT121.errorSupplier = true;
                return ['', x[0], '', `<span class="badge bg-danger">(${x[1]}) INVÁLIDO</span>`,
                    `<span class="badge bg-danger">INVÁLIDO</span>`, '', +x[2], +x[3], +x[4], +x[5], x[6], x[7], '<span class="badge bg-danger">ERROR PROVEEDOR</span>'];
            } else 
                return ['', x[0], '', '', '', '', +x[2], +x[3], +x[4], +x[5], x[6], x[7], '<span class="badge bg-yellow">NO CARGADO</span>'];
        }
    });
}

async function T121ValidateDataTask() {
    if (StatusT121.errorDate)
        error("ERROR DATE INVALID", 'Uno o varios productos poseen una fecha erronea. Recuerde que el formato permitido es DD/MM/AAAA.');
    else {
        await DataTask121.fields.forEach(x => {
            if (moment(x[10], "DD/MM/YYYY").toDate() > moment(x[11], "DD/MM/YYYY").toDate()) {
                StatusT121.errorOrderDate = true;
                x[12] = '<span class="badge bg-danger">FECHAS NO CONCUERDAN</span>';
            }
        });

        if (StatusT121.errorOrderDate)
            error("ERROR ORDER DATE INVALID", 'Uno o varios productos poseen orden de fechas erroneo. La fecha en que finaliza debe ser posterior a la que inicia.');
    }

    if (StatusT121.errorPercentage)
        error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');

    if (StatusT121.errorSupplier)
        error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
}

function T121SetItem(index, item) {
    DataTask121.fields[index][0] = item.ID;
    DataTask121.fields[index][1] = item.ItemLookupCode;
    DataTask121.fields[index][2] = item.Description;
    DataTask121.fields[index][5] = item.Price;
    DataTask121.fields[index][12] = '<span class="badge bg-primary">OBTENIDO</span>';
    if (item.SupplierID === 0 && DataTask121.fields[index][3] === '') {
        StatusT121.supplierNotFound = true;
        DataTask121.fields[index][3] = `<span class="badge bg-danger">${item.SupplierCode}</span>`;
        DataTask121.fields[index][4] = `<span class="badge bg-danger">${item.SupplierName}</span>`;
        DataTask121.fields[index][12] = '<span class="badge bg-danger">SIN PROVEEDOR</span>';
    } else if (DataTask121.fields[index][3] === ''){
        DataTask121.fields[index][3] = item.SupplierCode;
        DataTask121.fields[index][4] = item.SupplierName;
    }
}

function T121OnDoneGetInfo(index, data) {
    if (data.Status) {
        T121SetItem(index, data.Result);
        toastr.success('Producto ' + DataTask121.fields[index][1] + ' encontrado.');
    } else {
        DataTask121.fields[index][12] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
        StatusT121.noFind = true;
        toastr.error('Producto ' + DataTask121.fields[index][1] + ' no encontrado.');
    }
    T121GetInfo(index + 1);
}

function T121GetInfo(index) {
    if (index < DataTask121.fields.length) {
        if (StatusT121.availableGetData) {
            try {
                StatusT121.gettingData = true;
                //if (DataTask121.fields[index][8] === '' || DataTask121.fields[index][8] === '0' || (+DataTask121.fields[index][8]) > 0) StatusT121.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask121.fields[index][1] + ". " + (index + 1) + " de " + DataTask121.fields.length);
                $.get(urls.getItem + "?itemLookupCode=" + DataTask121.fields[index][1]).done(function (data) {
                    T121OnDoneGetInfo(index, data);
                }).fail(function () {
                    DataTask121.fields[index][12] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T121GetInfo(index + 1);
                    StatusT121.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask121.fields[index][1] + '.');
                });
            } catch (e) {
                DataTask121.fields[index][12] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T121GetInfo(index + 1);
                StatusT121.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask121.fields[index][1] + '.');
            }
        } else T121EndGetInfo();
    } else T121EndGetInfo();
}

function T121EndGetInfo() {
    StatusT121.gettingData = false;
    StatusT121.isGetted = true;
    StatusT121.applyChanges = !StatusT121.dataErrors && !StatusT121.noFind && StatusT121.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask121.columns, DataTask121.fields);
}

function T121GetItemsToStruct() {
    return DataTask121.fields.map(x => {
        return {
            ID: x[0],
            SupplierCode: x[3],
            ItemLookupCode: x[1],
            InvoiceDiscount: x[8],
            CustomerDiscount: x[9],
            StartDate: x[10],
            EndDate: x[11],
            SalePrice: x[6],
            Quantity: x[7],
        }
    })
}

function T121OnDoneApplyChanges(data) {
    if (data.Status) {
        data.Result.forEach(function (r) {
            if (r.Status) {
                $(document).Toasts('create', {
                    class: 'bg-success',
                    title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message
                });
            }
            else {
                $(document).Toasts('create', {
                    class: 'bg-danger',
                    title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message
                });
            }
        })
    } else error(data.InternalMessage, data.Message);
    $("#spn").hide();
    StatusT121.applingChanges = false;
}

async function T121ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            var list = T121GetItemsToStruct();
            $("#spn").show();
            $.post(Task121.urls.applyTask, { stores: taskConfig.stores, notes: taskValues, items: list }).done(function (data) {
                T121OnDoneApplyChanges(data);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                error(thrownError, 'Error intentando aplicar cambios.');
                StatusT121.applingChanges = false;
            });
        } else StatusT121.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT121.applingChanges = false;
    }
}

var Task121 = {
    urls: { applyTask: '../Wizard/Task121' },

    startTask: async function () {
        try {
            await T121InitStatus();

            await T121CsvToTaskTable();

            await initTable(DataTask121.columns, DataTask121.fields);

            await T121ValidateDataTask();

            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (!StatusT121.supplierNotFound && !StatusT121.errorDate && !StatusT121.errorOrderDate && !StatusT121.errorPercentage && !StatusT121.errorSupplier) {
            if (StatusT121.applyChanges && !StatusT121.applingChanges && !StatusT121.dataEmpty) {
                StatusT121.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T121ApplyNewChanges();
                    else StatusT121.applingChanges = false;
                });
            } else {
                if (StatusT121.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT121.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT121.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT121.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT121.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT121.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT121.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else {
            if (StatusT121.supplierNotFound)
                error("ERROR SUPPLIER NO FIND", 'Uno o varios productos no tienen asignado un proveedor.');
            if (StatusT121.errorDate)
                error("ERROR DATE INVALID", 'Uno o varios productos poseen una fecha erronea. Recuerde que el formato permitido es DD/MM/AAAA.');
            if (StatusT121.errorOrderDate)
                error("ERROR ORDER DATE INVALID", 'Uno o varios productos poseen orden de fechas erroneo. La fecha en que finaliza debe ser posterior a la que inicia.');
            if (StatusT121.errorPercentage)
                error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');
            if (StatusT121.errorSupplier)
                error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
        }
    },

    onClickBtnGetInfo: function () {
        if (!StatusT121.errorDate && !StatusT121.errorOrderDate && !StatusT121.errorPercentage && !StatusT121.errorSupplier) {
            $("#spn").show()
            StatusT121.availableGetData = true;
            StatusT121.noFind = false;
            StatusT121.dataEmpty = false;
            StatusT121.supplierNotFound = false;
            T121GetInfo(0);
        } else {
            if (StatusT121.errorOrderDate)
                error("ERROR ORDER DATE INVALID", 'Uno o varios productos poseen orden de fechas erroneo. La fecha en que finaliza debe ser posterior a la que inicia.');
            if (StatusT121.errorDate)
                error("ERROR DATE INVALID", 'Uno o varios productos poseen una fecha erronea. Recuerde que el formato permitido es DD/MM/AAAA.');
            if (StatusT121.errorPercentage)
                error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');
            if (StatusT121.errorSupplier)
                error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
        }
    },
}

function T121GetSuppliers() {
    if (taskList.suppliers.length <= 0) {
        try {
            $("#spn").show();
            $.get(urls.getSuppliers).done(function (data) {
                if (data.Status) {
                    taskList.suppliers = data.Result.suppliers;
                    Task121.startTask();
                } else {
                    error(data.InternalMessage, data.Message);
                    $("#spn").hide();
                }
            }).fail(function (e) {
                $("#spn").hide();
                error(e, '"No se puede continuar. Error al intentar obtener los proveedores del sistema."')
            })
        } catch (e) {
            $("#spn").hide();
            error(e, '"No se puede continuar. Error al intentar obtener los proveedores del sistema."')
        }
    } else {
        Task121.startTask();
    }
}

setDataTaskConfiguration('T121', DataTask121.columnsFile, T121GetSuppliers, Task121.onClickBtnApplyChanges, Task121.onClickBtnGetInfo);