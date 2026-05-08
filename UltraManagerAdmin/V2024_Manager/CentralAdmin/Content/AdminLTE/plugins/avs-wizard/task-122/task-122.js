/**
 * Tarea de Cambio de Precios: Promociones Compre X lleve Y
 */

var StatusT122 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    errorPercentage: false,
    errorSupplier: false,
}, DataTask122 = {
    fields: [],
    columns: [
        { title: '', visible: false, },
        { title: 'Cód. Producto' },
        { title: 'Descripción' },
        { title: 'Cód. Proveedor' },
        { title: 'Proveedor' },
        { title: 'Promoción de Descuento Actual' },
        { title: 'Nueva Promoción de Descuento' },
        {
            title: 'Descuento en Factura(%)',
            render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT122.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            }
        },
        {
            title: 'Descuento para Cliente(%)',
            render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT122.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            }
        },
        { title: 'Descuento Total' },
        { title: 'Fecha Inicio de Promoción' },
        { title: 'Fecha Fin de Promoción' },
        { title: 'Estado' },
    ],
    columnsFile: [
        'Codigo de Producto',
        'Codigo de Proveedor',
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)'
    ],
    infoTask: { // Info de la tarea, descuento a aplicar y fechas en las que inicia y culmina dicha promo
        QuantityDiscount: { ID: 0, Description: '' },
        StartDate: null,
        EndDate: null
    }
};

function T122InitStatus() {
    StatusT122.applyChanges = false;
    StatusT122.availableGetData = true;
    StatusT122.gettingData = false;
    StatusT122.dataErrors = false;
    StatusT122.isGetted = false;
    StatusT122.dataEmpty = false;
    StatusT122.noFind = false;
    StatusT122.errorPercentage = false;
    StatusT122.errorSupplier = false;
}

function T122CsvToTaskTable() {
    DataTask122.fields = csvData.map(x => {
        var supplier = taskList.suppliers.find(s => s.Code === x[1]);
        if (supplier)
            return ['', x[0], '', x[1], supplier.Name, '-', DataTask122.infoTask.QuantityDiscount.Description,
                +x[2], +x[3], ((+x[2]) + (+x[3])), DataTask122.infoTask.StartDate, DataTask122.infoTask.EndDate,
                '<span class="badge bg-yellow">NO CARGADO</span>'];
        else {
            if (x[1] !== '') {
                StatusT122.errorSupplier = true;
                return ['', x[0], '', `<span class="badge bg-danger">(${x[1]}) INVÁLIDO</span>`,
                    `<span class="badge bg-danger">INVÁLIDO</span>`, '-',
                    DataTask122.infoTask.QuantityDiscount.Description, +x[2], +x[3], ((+x[2]) + (+x[3])),
                    DataTask122.infoTask.StartDate, DataTask122.infoTask.EndDate,
                    '<span class="badge bg-danger">ERROR PROVEEDOR</span>'];
            } else
                return ['', x[0], '', '', '', '-', DataTask122.infoTask.QuantityDiscount.Description,
                    +x[2], +x[3], ((+x[2]) + (+x[3])), DataTask122.infoTask.StartDate, DataTask122.infoTask.EndDate,
                    '<span class="badge bg-yellow">NO CARGADO</span>'];
        }
    });
}

async function T122ValidateDataTask() {
    if (StatusT122.errorPercentage)
        error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');

    if (StatusT122.errorSupplier)
        error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
}

function T122SetItem(index, item) {
    DataTask122.fields[index][0] = item.ID;
    DataTask122.fields[index][2] = item.Description;
    DataTask122.fields[index][5] = item.QuantityDiscountName;
    DataTask122.fields[index][12] = '<span class="badge bg-primary">OBTENIDO</span>';
    if (item.SupplierID === 0 && DataTask122.fields[index][3] === '') {
        StatusT122.supplierNotFound = true;
        DataTask122.fields[index][3] = `<span class="badge bg-danger">${item.SupplierCode}</span>`;
        DataTask122.fields[index][4] = `<span class="badge bg-danger">${item.SupplierName}</span>`;
        DataTask122.fields[index][12] = '<span class="badge bg-danger">SIN PROVEEDOR</span>';
    } else if (DataTask122.fields[index][3] === '') {
        DataTask122.fields[index][3] = item.SupplierCode;
        DataTask122.fields[index][4] = item.SupplierName;
    }
}

function T122OnDoneGetInfo(index, data) {
    if (data.Status) {
        T122SetItem(index, data.Result);
        toastr.success('Producto ' + DataTask122.fields[index][1] + ' encontrado.');
    } else {
        DataTask122.fields[index][12] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
        StatusT122.noFind = true;
        toastr.error('Producto ' + DataTask122.fields[index][1] + ' no encontrado.');
    }
    T122GetInfo(index + 1);
}

function T122GetInfo(index) {
    if (index < DataTask122.fields.length) {
        if (StatusT122.availableGetData) {
            try {
                StatusT122.gettingData = true;
                $("#txt_spn").html("Obteniendo " + DataTask122.fields[index][1] + ". " + (index + 1) + " de " + DataTask122.fields.length);
                $.get(urls.getItem + "?itemLookupCode=" + DataTask122.fields[index][1]).done(function (data) {
                    T122OnDoneGetInfo(index, data);
                }).fail(function () {
                    DataTask122.fields[index][12] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T122GetInfo(index + 1);
                    StatusT122.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask122.fields[index][1] + '.');
                });
            } catch (e) {
                DataTask122.fields[index][12] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T122GetInfo(index + 1);
                StatusT122.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask122.fields[index][1] + '.');
            }
        } else T122EndGetInfo();
    } else T122EndGetInfo();
}

function T122EndGetInfo() {
    StatusT122.gettingData = false;
    StatusT122.isGetted = true;
    StatusT122.applyChanges = !StatusT122.dataErrors && !StatusT122.noFind && StatusT122.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask122.columns, DataTask122.fields);
}

function T122GetItemsToStruct() {
    return DataTask122.fields.map(x => {
        return {
            ID: x[0],
            SupplierCode: x[3],
            ItemLookupCode: x[1],
            InvoiceDiscount: x[7],
            CustomerDiscount: x[8]
        }
    })
}

function T122OnDoneApplyChanges(data) {
    if (data.Status) {
        data.Result.forEach(function (r) {
            if (r.Status)
                $(document).Toasts('create', { class: 'bg-success', title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message });
            else
                $(document).Toasts('create', { class: 'bg-danger', title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message });
        })
    } else error(data.InternalMessage, data.Message);
    $("#spn").hide();
    StatusT122.applingChanges = false;
}

async function T122ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {

            var list = T122GetItemsToStruct();
            $("#spn").show();
            $.post(Task122.urls.applyTask, {
                stores: taskConfig.stores,
                notes: taskValues,
                quantityDiscountID: DataTask122.infoTask.QuantityDiscount.ID,
                startDate: DataTask122.infoTask.StartDate,
                endDate: DataTask122.infoTask.EndDate,
                items: list
            }).done(function (data) {
                T122OnDoneApplyChanges(data);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                error(thrownError, 'Error intentando aplicar cambios.');
                StatusT122.applingChanges = false;
                $("#spn").hide();
            });

        } else StatusT122.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT122.applingChanges = false;
        $("#spn").hide();
    }
}

var Task122 = {
    urls: {
        applyTask: '../Wizard/Task122',
        getInitData: '../Wizard/GetInitDataTask122'
    },

    startTask: async function () {
        try {
            await T122InitStatus();

            await T122CsvToTaskTable();

            await initTable(DataTask122.columns, DataTask122.fields);

            await T122ValidateDataTask();

            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (!StatusT122.errorPercentage && !StatusT122.errorSupplier) {
            if (StatusT122.applyChanges && !StatusT122.applingChanges && !StatusT122.dataEmpty) {
                StatusT122.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T122ApplyNewChanges();
                    else StatusT122.applingChanges = false;
                });
            } else {
                if (StatusT122.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT122.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT122.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT122.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT122.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT122.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT122.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else {
            if (StatusT122.errorPercentage)
                error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');
            if (StatusT122.errorSupplier)
                error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
        }
    },

    onClickBtnGetInfo: function () {
        if (!StatusT122.errorPercentage && !StatusT122.errorSupplier) {
            $("#spn").show()
            StatusT122.availableGetData = true;
            StatusT122.noFind = false;
            StatusT122.dataEmpty = false;
            T122GetInfo(0);
        } else {
            if (StatusT122.errorPercentage)
                error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');
            if (StatusT122.errorSupplier)
                error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
        }
    },
}

async function T122AskConfigurationTask() {
    var opts = {};

    await taskList.quantityDiscounts.forEach(x => { opts[x.ID] = x.Description; });

    return Swal.fire({
        title: 'Configuración de Dinámica de Descuento',
        input: 'select',
        inputOptions: opts,
        inputLabel: 'Descuento:',
        inputPlaceholder: 'Seleccionar descuento...',
        html:
            `<div>
                <label>Fecha inicio de Dinámica:</label>
                <input type="date" id="swal-task-start-date" class="swal2-input"/>
            </div>
            <div>
                <label>Fecha final de Dinámica:</label>
                <input type="date" id="swal-task-end-date" class="swal2-input"/>
            </div>`,
        showCancelButton: true,
        allowOutsideClick: false,
        allowEscapeKey: false,
        preConfirm: () => {

            var disc = $('.swal2-select').val(), start = $('#swal-task-start-date').val(), end = $('#swal-task-end-date').val();
            console.log(disc, start, end);
            if (disc && start && end)
                if (isNaN(moment(start, "YYYY/MM/DD").toDate()) || isNaN(moment(end, "YYYY/MM/DD").toDate())) {
                    toastr.error("Debe ingresar fechas válidas.");
                    console.log("CONFIG TASK122 ERROR DATES NULLS");
                    return false;
                } else if (moment(start, "YYYY/MM/DD").toDate() > moment(end, "YYYY/MM/DD").toDate()) {
                    toastr.error("Fechas incorrectas. Fecha inicio no puede ser después de fecha final.");
                    console.log("CONFIG TASK122 ERROR ORDER DATES");
                    return false;
                } else
                    return {
                        discount: disc,
                        startDate: start,
                        endDate: end
                    }
            else {
                toastr.error("Debe ingresar las fechas y seleccionar un descuento para continuar.");
                console.log("CONFIG TASK122 ERROR");
                return false;
            }
        },
    });
}

async function T122GetConfigurationTask() {
    const { value: taskValues } = await T122AskConfigurationTask();

    if (taskValues) {
        DataTask122.infoTask.QuantityDiscount.ID = taskValues.discount;
        DataTask122.infoTask.QuantityDiscount.Description = taskList.quantityDiscounts.find(x => { return x.ID.toString() === taskValues.discount.toString() }).Description;
        DataTask122.infoTask.StartDate = taskValues.startDate;
        DataTask122.infoTask.EndDate = taskValues.endDate;
        Task122.startTask();
    }
    else $("#spn").hide();
}

function T122GetInitData() {
    if (taskList.suppliers.length <= 0 || taskList.quantityDiscounts.length <= 0) {
        try {
            $("#spn").show();
            $.get(Task122.urls.getInitData).done(function (data) {
                if (data.Status) {
                    taskList.suppliers = data.Result.suppliers;
                    taskList.quantityDiscounts = data.Result.quantityDiscounts;
                    T122GetConfigurationTask();
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
    } else T122GetConfigurationTask();
}

setDataTaskConfiguration('T122', DataTask122.columnsFile, T122GetInitData, Task122.onClickBtnApplyChanges, Task122.onClickBtnGetInfo);