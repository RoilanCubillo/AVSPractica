/**
 * Tarea de cambio de subdescripción 3 (Cabys) de productos
 */

var DataTask110 = {
    fields: [],
    columns: [{ title: 'Código de producto' }, { title: 'Cabys Actual' }, { title: 'Cabys Nueva' }, { title: 'Estado' }, { title: '', visible: false, }, { title: 'Description', visible: false, }]
};

var StatusT110 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false
}

function T110GetInfo(index) {
    if (index < DataTask110.fields.length) {
        if (StatusT110.availableGetData) {
            try {
                StatusT110.gettingData = true;
                if (DataTask110.fields[index][2] === '') StatusT110.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask110.fields[index][0] + ". " + (index + 1) + " de " + DataTask110.fields.length);
                $.get(Task110.urls.getItemDescription3 + "?itemLookupCode=" + DataTask110.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask110.fields[index][1] = data.Result.SubDescription3;
                        DataTask110.fields[index][4] = data.Result.ID;
                        DataTask110.fields[index][5] = data.Result.Description;
                        DataTask110.fields[index][3] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask110.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask110.fields[index][3] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT110.noFind = true;
                        toastr.error('Producto ' + DataTask110.fields[index][0] + ' no encontrado.');
                    }
                    T110GetInfo(index + 1);
                }).fail(function () {
                    DataTask110.fields[index][3] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T110GetInfo(index + 1);
                    StatusT110.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask110.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask110.fields[index][3] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T110GetInfo(index + 1);
                StatusT110.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask110.fields[index][0] + '.');
            }
        } else T110EndGetInfo();
    } else T110EndGetInfo();
}

function T110EndGetInfo() {
    StatusT110.gettingData = false;
    StatusT110.isGetted = true;
    StatusT110.applyChanges = !StatusT110.dataErrors && !StatusT110.noFind && StatusT110.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask110.columns, DataTask110.fields);
}

function T110GetItemsToStruct() {
    return DataTask110.fields.map(x => {
        return {
            ID: x[4], ItemLookupCode: x[0], Description: x[5],
            OldCabys: x[1], SubDescription3: x[2]
        }
    })
}

function T110ApplyNewChanges(notes) {
    try {
        var list = T110GetItemsToStruct();
        $("#spn").show();
        $.post(Task110.urls.applyTask, { stores: taskConfig.stores, items: list, notes: notes }).done(function (data) {
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
            StatusT110.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT110.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT110.applingChanges = false;
    }
}

var Task110 = {
    urls: {
        getItemDescription3: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task110'
    },

    startTask: function () {
        try {
            StatusT110.applyChanges = false;
            StatusT110.availableGetData = true;
            StatusT110.gettingData = false;
            StatusT110.dataErrors = false;
            StatusT110.isGetted = false;
            StatusT110.dataEmpty = false;
            StatusT110.noFind = false;
            DataTask110.fields = csvData.map(x => { return [x[0], '', x[1], '<span class="badge bg-yellow">NO CARGADO</span>', '', ''] });
            initTable(DataTask110.columns, DataTask110.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT110.applyChanges && !StatusT110.applingChanges && !StatusT110.dataEmpty) {
            StatusT110.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const { value: text } = await getNotesTask();

                    if (text) T110ApplyNewChanges(text);
                    else StatusT110.applingChanges = false;
                } else StatusT110.applingChanges = false;
            });
        } else {
            if (StatusT110.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT110.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT110.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT110.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT110.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT110.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            if (StatusT110.dataEmpty) toastr.error("No se puede continuar. Una o varias descripciones están vacías.");
        }
    },

    onClickBtnGetInfo: function () {
        $("#spn").show()
        StatusT110.availableGetData = true;
        StatusT110.noFind = false;
        StatusT110.dataEmpty = false;
        T110GetInfo(0);
    },
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T110', ['Código de producto', 'Cabys'], Task110.startTask, Task110.onClickBtnApplyChanges, Task110.onClickBtnGetInfo);