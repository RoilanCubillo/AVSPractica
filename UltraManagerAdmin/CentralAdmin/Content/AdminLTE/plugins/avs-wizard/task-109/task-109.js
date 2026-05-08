/**
 * Tarea de cambio de subdescripción 2 de productos
 */

var DataTask109 = {
    fields: [],
    columns: [{ title: 'Código de producto' }, { title: 'Descripción 2 Actual' }, { title: 'Descripción 2 Nueva' }, { title: 'Estado' }, { title: '', visible: false, }, { title: 'Description', visible: false, }]
};

var StatusT109 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false
}

function T109GetInfo(index) {
    if (index < DataTask109.fields.length) {
        if (StatusT109.availableGetData) {
            try {
                StatusT109.gettingData = true;
                if (DataTask109.fields[index][2] === '') StatusT109.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask109.fields[index][0] + ". " + (index + 1) + " de " + DataTask109.fields.length);
                $.get(Task109.urls.getItemDescription2 + "?itemLookupCode=" + DataTask109.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask109.fields[index][1] = data.Result.SubDescription2;
                        DataTask109.fields[index][4] = data.Result.ID;
                        DataTask109.fields[index][5] = data.Result.Description;
                        DataTask109.fields[index][3] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask109.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask109.fields[index][3] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT109.noFind = true;
                        toastr.error('Producto ' + DataTask109.fields[index][0] + ' no encontrado.');
                    }
                    T109GetInfo(index + 1);
                }).fail(function () {
                    DataTask109.fields[index][3] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T109GetInfo(index + 1);
                    StatusT109.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask109.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask109.fields[index][3] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T109GetInfo(index + 1);
                StatusT109.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask109.fields[index][0] + '.');
            }
        } else T109EndGetInfo();
    } else T109EndGetInfo();
}

function T109EndGetInfo() {
    StatusT109.gettingData = false;
    StatusT109.isGetted = true;
    StatusT109.applyChanges = !StatusT109.dataErrors && !StatusT109.noFind && StatusT109.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask109.columns, DataTask109.fields);
}

function T109GetItemsToStruct() {
    return DataTask109.fields.map(x => {
        return {
            ID: x[4], ItemLookupCode: x[0], Description: x[5],
            OldSubDescription2: x[1], SubDescription2: x[2]
        }
    })
}

function T109ApplyNewChanges() {
    try {
        var list = T109GetItemsToStruct();
        $("#spn").show();
        $.post(Task109.urls.applyTask, { stores: taskConfig.stores, items: list }).done(function (data) {
            if (data.Status) {
                data.Result.forEach(function (r) {
                    if (r.Status) {
                        $(document).Toasts('create', {
                            class: 'bg-success',
                            title: 'Creación de hoja de trabajo',
                            subtitle: '',
                            body: r.Message
                        });
                    } else toastr.error(r.Message);
                })
            } else error(data.InternalMessage, data.Message)
            $("#spn").hide();
            StatusT109.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT109.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT109.applingChanges = false;
    }
}

var Task109 = {
    urls: {
        getItemDescription2: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task109'
    },

    startTask: function () {
        try {
            StatusT109.applyChanges = false;
            StatusT109.availableGetData = true;
            StatusT109.gettingData = false;
            StatusT109.dataErrors = false;
            StatusT109.isGetted = false;
            StatusT109.dataEmpty = false;
            StatusT109.noFind = false;
            DataTask109.fields = csvData.map(x => { return [x[0], '', x[1], '<span class="badge bg-yellow">NO CARGADO</span>', '', ''] });
            initTable(DataTask109.columns, DataTask109.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT109.applyChanges && !StatusT109.applingChanges) {
            StatusT109.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then((result) => {
                if (result.isConfirmed) {
                    if (StatusT109.dataEmpty) {
                        Swal.fire({
                            title: "¿Continuar?", html: "Algunos productos se guardarán con SubDescripción2 vacía...", icon: 'warning', showCancelButton: true,
                        }).then((result) => {
                            if (result.isConfirmed) T109ApplyNewChanges();
                            else StatusT109.applingChanges = false;
                        });
                    } else T109ApplyNewChanges();
                } else StatusT109.applingChanges = false;
            });
        } else {
            if (StatusT109.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT109.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT109.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT109.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT109.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT109.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
        }
    },

    onClickBtnGetInfo: function () {
        $("#spn").show()
        StatusT109.availableGetData = true;
        StatusT109.noFind = false;
        StatusT109.dataEmpty = false;
        T109GetInfo(0);
    },
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T109', ['Código de producto', 'Descripción 2'], Task109.startTask, Task109.onClickBtnApplyChanges, Task109.onClickBtnGetInfo);