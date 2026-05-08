/**
 * Tarea de cambio de estados de productos
 */

var DataTask105 = {
    fields: [],
    columns: [
        { title: 'Código de producto' },
        { title: 'Descripción' },
        {
            title: 'Estado Actual del Producto',
            render: function (data, type, row, meta) {
                var valClass = data.toString() === '0' ? 'success' : data.toString() === '1' ? 'danger' : 'primary';
                var valStatus = data.toString() === '0' ? 'ACTIVO' : data.toString() === '1' ? 'INACTIVO' : 'NO DEFINIDO'
                return type == 'display' ? '<span class="badge bg-' + valClass + '">' + valStatus + '</span>' : data;
            },
        },
        {
            title: 'Estado Nuevo del Producto',
            render: function (data, type, row, meta) {
                var valClass = data.toString() === '0' ? 'success' : data.toString() === '1' ? 'danger' : 'primary';
                var valStatus = data.toString() === '0' ? 'ACTIVAR' : data.toString() === '1' ? 'INACTIVAR' : 'NO DEFINIDO'
                return type == 'display' ? '<span class="badge bg-' + valClass + '">' + valStatus + '</span>' : data;
            }
        },
        { title: 'Estado de Carga' },
        { title: '', visible: false, }
    ],
    columnsFile: ['Código de producto', 'Estado Nuevo (1=Inactivar Producto, 0=Activar producto)']
};

var StatusT105 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false
}

function T105GetInfo(index) {
    if (index < DataTask105.fields.length) {
        if (StatusT105.availableGetData) {
            try {
                StatusT105.gettingData = true;
                if (DataTask105.fields[index][3] !== 0 && DataTask105.fields[index][3] !== 1 && DataTask105.fields[index][3] !== '0' && DataTask105.fields[index][3] !== '1') StatusT105.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask105.fields[index][0] + ". " + (index + 1) + " de " + DataTask105.fields.length);
                $.get(Task105.urls.getItem + "?itemLookupCode=" + DataTask105.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask105.fields[index][1] = data.Result.Description;
                        DataTask105.fields[index][2] = data.Result.Inactive ? 1 : 0;
                        DataTask105.fields[index][5] = data.Result.ID;
                        DataTask105.fields[index][4] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask105.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask105.fields[index][4] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT105.noFind = true;
                        toastr.error('Producto ' + DataTask105.fields[index][0] + ' no encontrado.');
                    }
                    T105GetInfo(index + 1);
                }).fail(function () {
                    DataTask105.fields[index][4] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T105GetInfo(index + 1);
                    StatusT105.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask105.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask105.fields[index][4] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T105GetInfo(index + 1);
                StatusT105.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask105.fields[index][0] + '.');
            }
        } else T105EndGetInfo();
    } else T105EndGetInfo();
}

function T105EndGetInfo() {
    StatusT105.gettingData = false;
    StatusT105.isGetted = true;
    StatusT105.applyChanges = !StatusT105.dataErrors && !StatusT105.noFind && StatusT105.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask105.columns, DataTask105.fields);
}

function T105GetItemsToStruct() {
    return DataTask105.fields.map(x => {
        return {
            ID: x[5],
            ItemLookupCode: x[0],
            Description: x[1],
            AnteriorEstado: x[2],
            NuevoEstado: x[3]
        }
    })
}

function T105ApplyNewChanges() {
    try {
        var list = T105GetItemsToStruct();
        $("#spn").show();
        $.post(Task105.urls.applyTask, { stores: taskConfig.stores, items: list }).done(function (data) {
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
            StatusT105.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT105.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT105.applingChanges = false;
    }
}

var Task105 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task105'
    },

    startTask: function () {
        try {
            StatusT105.applyChanges = false;
            StatusT105.availableGetData = true;
            StatusT105.gettingData = false;
            StatusT105.dataErrors = false;
            StatusT105.isGetted = false;
            StatusT105.dataEmpty = false;
            StatusT105.noFind = false;
            DataTask105.fields = csvData.map(x => { return [x[0], '', '', x[1], '<span class="badge bg-yellow">NO CARGADO</span>', ''] });
            initTable(DataTask105.columns, DataTask105.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT105.applyChanges && !StatusT105.applingChanges && !StatusT105.dataEmpty) {
            StatusT105.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then((result) => {
                if (result.isConfirmed) T105ApplyNewChanges();
                else StatusT105.applingChanges = false;
            });
        } else {
            if (StatusT105.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
            if (StatusT105.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT105.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT105.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT105.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT105.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT105.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
        }
    },

    onClickBtnGetInfo: function () {
        $("#spn").show()
        StatusT105.availableGetData = true;
        StatusT105.noFind = false;
        StatusT105.dataEmpty = false;
        T105GetInfo(0);
    },
}

setDataTaskConfiguration('T105', DataTask105.columnsFile, Task105.startTask, Task105.onClickBtnApplyChanges, Task105.onClickBtnGetInfo);