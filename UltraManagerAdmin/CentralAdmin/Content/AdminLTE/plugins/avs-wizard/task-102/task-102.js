/**
 * Tarea de cambio de costos de productos
 */

var DataTask102 = {
    fields: [],
    columns: [
        { title: 'Código de producto' },
        { title: 'Descripción' },
        { title: 'Costo Actual' },
        { title: 'Costo Nuevo' },
        { title: '' },
        { title: '', visible: false, }
    ],
    columnsFile: ['Código de producto', 'Costo Nuevo']
};

var StatusT102 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false
}

function T102GetInfo(index) {
    if (index < DataTask102.fields.length) {
        if (StatusT102.availableGetData) {
            try {
                StatusT102.gettingData = true;
                if (DataTask102.fields[index][3] === numberToMoney(0) || DataTask102.fields[index][3] === numberToMoney('-')) StatusT102.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask102.fields[index][0] + ". " + (index + 1) + " de " + DataTask102.fields.length);
                $.get(Task102.urls.getItem + "?itemLookupCode=" + DataTask102.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask102.fields[index][1] = data.Result.Description;
                        DataTask102.fields[index][2] = numberToMoney(data.Result.Cost);
                        DataTask102.fields[index][5] = data.Result.ID;
                        DataTask102.fields[index][4] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask102.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask102.fields[index][4] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT102.noFind = true;
                        toastr.error('Producto ' + DataTask102.fields[index][0] + ' no encontrado.');
                    }
                    T102GetInfo(index + 1);
                }).fail(function () {
                    DataTask102.fields[index][4] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T102GetInfo(index + 1);
                    StatusT102.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask102.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask102.fields[index][4] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T102GetInfo(index + 1);
                StatusT102.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask102.fields[index][0] + '.');
            }
        } else T102EndGetInfo();
    } else T102EndGetInfo();
}

function T102EndGetInfo() {
    StatusT102.gettingData = false;
    StatusT102.isGetted = true;
    StatusT102.applyChanges = !StatusT102.dataErrors && !StatusT102.noFind && StatusT102.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask102.columns, DataTask102.fields);
}

function T102GetItemsToStruct() {
    return DataTask102.fields.map(x => {
        return {
            ID: x[5],
            ItemLookupCode: x[0],
            Description: x[1],
            MontoAnterior: moneyToNumber(x[2]),
            NuevoMonto: moneyToNumber(x[3])
        }
    })
}

function T102ApplyNewChanges() {
    try {
        var list = T102GetItemsToStruct();
        $("#spn").show();
        $.post(Task102.urls.applyTask, { stores: taskConfig.stores, items: list }).done(function (data) {
            if (data.Status) {
                data.Result.forEach(function (r) {
                    if (r.Status) {
                        $(document).Toasts('create', {
                            class: 'bg-success',
                            title: 'Creación de hoja de trabajo',
                            subtitle: '',
                            body: r.Message
                        });
                    }
                    else toastr.error(r.Message);
                })
            } else error(data.InternalMessage, data.Message)
            $("#spn").hide();
            StatusT102.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT102.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT102.applingChanges = false;
    }
}

var Task102 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task102'
    },

    startTask: function () {
        try {
            StatusT102.applyChanges = false;
            StatusT102.availableGetData = true;
            StatusT102.gettingData = false;
            StatusT102.dataErrors = false;
            StatusT102.isGetted = false;
            StatusT102.dataEmpty = false;
            StatusT102.noFind = false;
            DataTask102.fields = csvData.map(x => { return [x[0], '', numberToMoney('-'), numberToMoney(+(x[1])), '<span class="badge bg-yellow">NO CARGADO</span>', ''] });
            initTable(DataTask102.columns, DataTask102.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT102.applyChanges && !StatusT102.applingChanges) {
            StatusT102.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then((result) => {
                if (result.isConfirmed) {
                    if (StatusT102.dataEmpty) {
                        Swal.fire({
                            title: "¿Continuar?", html: "Algunos productos se guardarán con Costo 0...", icon: 'warning', showCancelButton: true,
                        }).then((result) => {
                            if (result.isConfirmed) T102ApplyNewChanges();
                            else StatusT102.applingChanges = false;
                        });
                    } else T102ApplyNewChanges();
                } else StatusT102.applingChanges = false;
            });
        } else {
            if (StatusT102.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT102.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT102.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT102.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT102.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT102.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
        }
    },

    onClickBtnGetInfo: function () {
        $("#spn").show()
        StatusT102.availableGetData = true;
        StatusT102.noFind = false;
        StatusT102.dataEmpty = false;
        T102GetInfo(0);
    },
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T102', DataTask102.columnsFile, Task102.startTask, Task102.onClickBtnApplyChanges, Task102.onClickBtnGetInfo);