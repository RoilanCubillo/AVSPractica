/**
 * Tarea de Cambio de Precios: Precio Regular
 */

var StatusT123 = {
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
    errorUtility: false
}, DataTask123 = {
    fields: [],
    columns: [
        { title: '', visible: false, },
        { title: 'Cód. Producto' },
        { title: 'Descripción' },
        { title: 'Cód. Proveedor' },
        { title: 'Proveedor' },
        {
            title: 'Último Costo Registrado',
            render: function (data, type, row, meta) {
                return type == 'display' ? numberToMoney(data) : data;
            }
        },
        {
            title: 'Nuevo Costo',
            render: function (data, type, row, meta) {
                return type == 'display' ? numberToMoney(data) : data;
            }
        },
        { title: 'Nueva Utilidad(%)' },
        {
            title: 'Descuento en Factura(%)',
            render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT123.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            }
        },
        {
            title: 'Descuento para Cliente(%)',
            render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT123.errorPercentage = true;
                    return '<span class="badge bg-danger">(' + data + ') % ERRONEO </span>';
                } else return data;
            }
        },
        { title: 'Descuento Total' },
        { title: 'Estado' },
    ],
    columnsFile: [
        'Codigo de Producto',
        'Costo Neto',
        'Utilidad',
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)'
    ],
    infoTask: { // Info de la tarea, descuento a aplicar y fechas en las que inicia y culmina dicha promo
        TipificationType: { ID: 0, Description: '' },
        StartDate: null,
        EndDate: null
    }
};

function T123InitStatus() {
    StatusT123.applyChanges = false;
    StatusT123.availableGetData = true;
    StatusT123.gettingData = false;
    StatusT123.dataErrors = false;
    StatusT123.isGetted = false;
    StatusT123.dataEmpty = false;
    StatusT123.noFind = false;
    StatusT123.errorPercentage = false;
    StatusT123.errorSupplier = false;
    StatusT123.errorUtility = false;
}

function T123CsvToTaskTable() {
    DataTask123.fields = csvData.map(x => {
        if (+x[2] < 0.01 || +x[2] > 800) StatusT123.errorUtility = true;

        return ['', x[0], '', '', '', '-', x[1], x[2], +x[3], +x[4], ((+x[3]) + (+x[4])), '<span class="badge bg-yellow">NO CARGADO</span>'];
    });
}

async function T123ValidateDataTask() {
    if (StatusT123.errorPercentage)
        error("ERROR PERCENTAGE INVALID", 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100%.');
    if (StatusT123.errorUtility)
        error("ERROR PERCENTAGE UTILITY", 'Uno o varios productos poseen una utilidad erroneos. Debe ser de 0.01~800');
    if (StatusT123.errorSupplier)
        error("ERROR SUPPLIER", "Uno o varios productos tienen asignado un proveedor inválido.");
}

function T123SetItem(item) {
    var dataItem = DataTask123.fields.find(y => y[1].trim().toUpperCase() === item.ItemLookupCode.trim().toUpperCase());
    dataItem[0] = item.ID;
    dataItem[2] = item.Description;
    dataItem[3] = item.SupplierCode;
    dataItem[4] = item.SupplierName;
    dataItem[5] = item.SupplierCost;
    if (item.SupplierID === 0) {
        StatusT123.supplierNotFound = true;
        dataItem[3] = `<span class="badge bg-danger">${item.SupplierCode}</span>`;
        dataItem[4] = `<span class="badge bg-danger">${item.SupplierName}</span>`;
        dataItem[11] = '<span class="badge bg-danger">SIN PROVEEDOR PRIMARIO</span>';
    }
}

async function T123OnDoneGetInfo(data) {
    if (data.Status) {
        var dataMap = data.Result[0].Result.map(x => { return x.ItemLookupCode.toUpperCase(); });

        await DataTask123.fields.forEach(i => {
            if (dataMap.includes(i[1].toUpperCase())) i[11] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';
            else {
                StatusT123.noFind = true;
                i[11] = '<span class="badge bg-danger">PRODUCTO NO VERIFICADO</span>';
            }
        });

        await data.Result[0].Result.forEach(function (x) { T123SetItem(x); });
    } else {
        StatusT123.noFind = true;
        error("PRODUCTOS NO VERIFICADOS", "Productos no verificados");
    }
    T123EndGetInfo();
}

function T123GetInfo() {
    if (StatusT123.availableGetData) {
        try {
            StatusT123.gettingData = true;
            $("#txt_spn").html("Verificando productos..");
            $.post(Task123.urls.verify, { itemLookupCodes: DataTask123.fields.map(x => { return x[1] }) }).done(function (data) {
                T123OnDoneGetInfo(data);
            }).fail(function () {
                StatusT123.dataErrors = true;
                error("ERROR CONSULTA", "Error intentando validar productos.");
                T123EndGetInfo();
            });
        } catch (e) {
            StatusT123.dataErrors = true;
            error("ERROR CONSULTA", "Error intentando validar productos.");
            T123EndGetInfo();
        }
    } else T123EndGetInfo();
}

function T123EndGetInfo() {
    StatusT123.gettingData = false;
    StatusT123.isGetted = true;
    StatusT123.applyChanges = !StatusT123.dataErrors && !StatusT123.noFind && StatusT123.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask123.columns, DataTask123.fields);

    if (StatusT123.supplierNotFound)
        error("ERROR SUPPLIER NO DEFINED", 'No se puede continuar. Uno o varios productos no tienen un proveedor primario asignado');
    if (StatusT123.noFind)
        error("PRODUCTOS NO ENCONTRADOS", 'No se puede continuar. Uno o varios productos no pudieron ser verificados. Revise que los códigos coincidan con los productos del sistema.');
}

function T123GetItemsToStruct() {
    return DataTask123.fields.map(x => {
        return {
            ID: x[0],
            ItemLookupCode: x[1],
            InvoiceDiscount: x[8],
            CustomerDiscount: x[9],
            Cost: x[6],
            Utility: x[7]
        }
    })
}

function T123OnDoneApplyChanges(data) {
    if (data.Status) {
        data.Result.forEach(function (r) {
            if (r.Status) {
                $(document).Toasts('create', { class: 'bg-success', title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message });
            }
            else
                $(document).Toasts('create', { class: 'bg-danger', title: 'Creación de hoja de trabajo', subtitle: '', body: r.Message });
        })
    } else error(data.InternalMessage, data.Message);
    $("#spn").hide();
    StatusT123.applingChanges = false;
}

async function T123ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {

            var list = T123GetItemsToStruct();
            $("#spn").show();
            $.post(Task123.urls.applyTask, {
                stores: taskConfig.stores,
                notes: taskValues,
                items: list
            }).done(function (data) {
                T123OnDoneApplyChanges(data);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                error(thrownError, 'Error intentando aplicar cambios.');
                StatusT123.applingChanges = false;
            });
        } else StatusT123.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT123.applingChanges = false;
    }
}

var Task123 = {
    urls: {
        applyTask: '../Wizard/Task123',
        verify: '../Wizard/Task123Verificar'
    },

    startTask: async function () {
        try {
            await T123InitStatus();

            await T123CsvToTaskTable();

            await initTable(DataTask123.columns, DataTask123.fields);

            await T123ValidateDataTask();

            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (!StatusT123.errorPercentage && !StatusT123.supplierNotFound && !StatusT123.errorUtility) {
            if (StatusT123.applyChanges && !StatusT123.applingChanges && !StatusT123.dataEmpty) {
                StatusT123.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T123ApplyNewChanges();
                    else StatusT123.applingChanges = false;
                });
            } else {
                if (StatusT123.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT123.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT123.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT123.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT123.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT123.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT123.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else {
            T123ValidateDataTask();
        }
    },

    onClickBtnGetInfo: function () {
        if (!StatusT123.errorPercentage && !StatusT123.errorSupplier && !StatusT123.errorUtility) {
            $("#spn").show()
            StatusT123.availableGetData = true;
            StatusT123.noFind = false;
            StatusT123.dataEmpty = false;
            StatusT123.supplierNotFound = false;
            T123GetInfo();
        } else {
            T123ValidateDataTask();
        }
    },
}

setDataTaskConfiguration('T123', DataTask123.columnsFile, Task123.startTask, Task123.onClickBtnApplyChanges, Task123.onClickBtnGetInfo);