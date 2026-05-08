/**
 * Tarea de cambio de descripciones corta
 */

var DataTask107 = {
    fields: [],
    columns: [
        { title: 'Código de producto' },
        { title: 'Descripción corta Actual' },
        { title: 'Descripción larga Actual' },
        { title: 'Descripción Corta Nueva', },
        { title: 'Estado' },
        { title: '', visible: false, }
    ],
    columnsFile: ['Código de producto', 'Descripción Corta Nueva']
};

var StatusT107 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false
}

function T107GetInfo(index) {
    if (index < DataTask107.fields.length) {
        if (StatusT107.availableGetData) {
            try {
                StatusT107.gettingData = true;
                if (DataTask107.fields[index][3] === '') StatusT107.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask107.fields[index][0] + ". " + (index + 1) + " de " + DataTask107.fields.length);
                $.get(Task107.urls.getItem + "?itemLookupCode=" + DataTask107.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask107.fields[index][1] = data.Result.Description;
                        DataTask107.fields[index][2] = data.Result.ExtendedDescription;
                        DataTask107.fields[index][5] = data.Result.ID;
                        DataTask107.fields[index][4] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask107.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask107.fields[index][4] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT107.noFind = true;
                        toastr.error('Producto ' + DataTask107.fields[index][0] + ' no encontrado.');
                    }
                    T107GetInfo(index + 1);
                }).fail(function () {
                    DataTask107.fields[index][4] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T107GetInfo(index + 1);
                    StatusT107.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask107.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask107.fields[index][4] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T107GetInfo(index + 1);
                StatusT107.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask107.fields[index][0] + '.');
            }
        } else T107EndGetInfo();
    } else T107EndGetInfo();
}

function T107EndGetInfo() {
    StatusT107.gettingData = false;
    StatusT107.isGetted = true;
    StatusT107.applyChanges = !StatusT107.dataErrors && !StatusT107.noFind && StatusT107.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask107.columns, DataTask107.fields);
}

function T107GetItemsToStruct() {
    return DataTask107.fields.map(x => {
        return {
            ID: x[5],
            ItemLookupCode: x[0],
            Description: x[1],
            DesAnterior: x[1],
            NuevaDes: x[3]
        }
    })
}

function T107ApplyNewChanges(notes) {
    try {
        var list = T107GetItemsToStruct();
        $("#spn").show();
        $.post(Task107.urls.applyTask, { stores: taskConfig.stores, items: list, notes: notes }).done(function (data) {
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
            StatusT107.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT107.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT107.applingChanges = false;
    }
}

var Task107 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task107'
    },

    startTask: function () {
        try {
            StatusT107.applyChanges = false;
            StatusT107.availableGetData = true;
            StatusT107.gettingData = false;
            StatusT107.dataErrors = false;
            StatusT107.isGetted = false;
            StatusT107.dataEmpty = false;
            StatusT107.noFind = false;
            DataTask107.fields = csvData.map(x => { return [x[0], '', '', x[1], '<span class="badge bg-yellow">NO CARGADO</span>', ''] });
            initTable(DataTask107.columns, DataTask107.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT107.applyChanges && !StatusT107.applingChanges && !StatusT107.dataEmpty) {
            StatusT107.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const { value: text } = await getNotesTask();

                    if (text) T107ApplyNewChanges(text);
                    else StatusT104.applingChanges = false;
                } else StatusT107.applingChanges = false;
            });
        } else {
            if (StatusT107.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT107.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT107.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT107.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT107.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT107.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            if (!StatusT107.dataEmpty) toastr.error("No se puede continuar. Una o varias descripciones están vacías.");
        }
    },

    onClickBtnGetInfo: function () {
        $("#spn").show()
        StatusT107.availableGetData = true;
        StatusT107.noFind = false;
        StatusT107.dataEmpty = false;
        T107GetInfo(0);
    },
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T107', DataTask107.columnsFile, Task107.startTask, Task107.onClickBtnApplyChanges, Task107.onClickBtnGetInfo);