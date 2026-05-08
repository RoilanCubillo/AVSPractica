/**
 * Tarea de cambio de Subdescripción Larga
 */

var DataTask108 = {
    fields: [],
    columns: [
        { title: 'Código de producto' },
        { title: 'Description' },
        { title: 'Descripción Larga Actual' },
        { title: 'Descripción Larga Nueva' },
        { title: 'Estado' },
        { title: '', visible: false, }
    ]
};

var StatusT108 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,

    codeEmpy: false,
    codeDuplicated: false
}

async function T108ValidateData() {
    await DataTask108.fields.forEach(x => {
        var count = 0;

        if (x[0] !== '') {
            for (var i = 0; i < DataTask108.fields.length; i++) {
                if (DataTask108.fields[i][0] === x[0]) count++;
            }
        } else {
            StatusT108.codeEmpy = true;
            x[4] = x[4].replaceAll('<span class="badge bg-yellow">NO CARGADO</span>', '');
            x[4] += `<span class="badge bg-danger">CÓDIGO DEL PRODUCTO NO SUMINISTRADO</span>,`;
        }

        if (x[3] === '') {
            StatusT108.dataEmpty = true;
            x[4] = x[4].replaceAll('<span class="badge bg-yellow">NO CARGADO</span>', '');
            x[4] += `<span class="badge bg-danger">DESCRIPCIÓN LARGA ES REQUERIDA</span>,`;
        }

        if (count > 1) {
            StatusT108.codeDuplicated = true;
            x[4] = x[4].replaceAll('<span class="badge bg-yellow">NO CARGADO</span>', '');
            x[4] += `<span class="badge bg-danger">PRODUCTO DUPLICADO</span>,`;
        }
    });
}

function T108VerifyValidate() {
    
    if (StatusT108.codeEmpy) $(document).Toasts('create', { class: 'bg-danger', title: 'Líneas sin código de producto', body: 'Una  o varias lineas no tienen asigando un código de producto.' });
    if (StatusT108.codeDuplicated) $(document).Toasts('create', { class: 'bg-danger', title: 'Líneas con códigos duplicados', body: 'Una o varias lineas tienen duplicado el producto.' });
    if (StatusT108.dataEmpty) $(document).Toasts('create', { class: 'bg-danger', title: 'Descripción Requerida', body: 'Una o varias lineas no tienen la Descripción Larga.' });
}

function T108GetInfo(index) {
    if (index < DataTask108.fields.length) {
        if (StatusT108.availableGetData) {
            try {
                StatusT108.gettingData = true;
                $("#txt_spn").html("Obteniendo " + DataTask108.fields[index][0] + ". " + (index + 1) + " de " + DataTask108.fields.length);
                $.get(Task108.urls.getItemDescription1 + "?itemLookupCode=" + DataTask108.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        DataTask108.fields[index][1] = data.Result.Description;
                        DataTask108.fields[index][2] = data.Result.ExtendedDescription;
                        DataTask108.fields[index][5] = data.Result.ID;
                        DataTask108.fields[index][4] = '<span class="badge bg-primary">VERIFICADO</span>';
                        toastr.success('Producto ' + DataTask108.fields[index][0] + ' verificado.');
                    } else {
                        DataTask108.fields[index][4] = '<span class="badge bg-danger">No Encontrado</span>';
                        StatusT108.noFind = true;
                        toastr.error('Producto ' + DataTask108.fields[index][0] + ' no encontrado encontrado en el sistema.');
                    }
                    T108GetInfo(index + 1);
                }).fail(function () {
                    DataTask108.fields[index][4] = '<span class="badge bg-danger">ERROR AL INTENTAR OBTENER EL PRODUCTO</span>';
                    T108GetInfo(index + 1);
                    StatusT108.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask108.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask108.fields[index][4] = '<span class="badge bg-danger">ERROR AL INTENTAR CONSULTA EL PRODUCTO</span>';
                T108GetInfo(index + 1);
                StatusT108.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask108.fields[index][0] + '.');
            }
        } else T108EndGetInfo();
    } else T108EndGetInfo();
}

function T108EndGetInfo() {
    StatusT108.gettingData = false;
    StatusT108.isGetted = true;
    StatusT108.applyChanges = !StatusT108.dataErrors && !StatusT108.noFind && StatusT108.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask108.columns, DataTask108.fields);
}

function T108GetItemsToStruct() {
    return DataTask108.fields.map(x => {
        return {
            ID: x[5], ItemLookupCode: x[0], Description: x[1],
            OldSubDescription1: x[2], SubDescription1: x[3]
        }
    })
}

function T108ApplyNewChanges(notes) {
    try {
        var list = T108GetItemsToStruct();
        $("#spn").show();
        $.post(Task108.urls.applyTask, { stores: taskConfig.stores, items: list, notes: notes }).done(function (data) {
            if (data.Status) {
                data.Result.forEach(x => {
                    $(document).Toasts('create', {
                        class: 'bg-success',
                        title: 'Creación de hoja de trabajo',
                        subtitle: '',
                        body: x.Message
                    });
                });
            } else error(data.InternalMessage, data.Message)
            $("#spn").hide();
            StatusT108.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT108.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT108.applingChanges = false;
    }
}

var Task108 = {
    urls: {
        getItemDescription1: '../Wizard/GetItemByItemLookupCode',
        applyTask: '../Wizard/Task108'
    },

    startTask: async function () {
        try {
            StatusT108.applyChanges = false;
            StatusT108.availableGetData = true;
            StatusT108.gettingData = false;
            StatusT108.dataErrors = false;
            StatusT108.isGetted = false;
            StatusT108.dataEmpty = false;
            StatusT108.noFind = false;
            StatusT108.codeEmpy = false;
            StatusT108.codeDuplicated = false;
            DataTask108.fields = csvData.map(x => { return [x[0], '', '', x[1], '<span class="badge bg-yellow">NO CARGADO</span>', ''] });
            await T108ValidateData();
            T108VerifyValidate();
            initTable(DataTask108.columns, DataTask108.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (StatusT108.applyChanges && !StatusT108.applingChanges && !StatusT108.codeEmpy && !StatusT108.codeDuplicated && !StatusT108.dataEmpty) {
            StatusT108.applingChanges = true;
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const { value: text } = await getNotesTask();

                    if (text) T108ApplyNewChanges(text);
                    else StatusT108.applingChanges = false;
                } else StatusT108.applingChanges = false;
            });
        } else {
            if (StatusT108.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
            if (StatusT108.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
            if (StatusT108.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
            if (!StatusT108.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
            if (StatusT108.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
            if (!StatusT108.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            T108VerifyValidate();
        }
    },

    onClickBtnGetInfo: function () {
        if (!StatusT108.codeEmpy && !StatusT108.codeDuplicated && !StatusT108.dataEmpty) {
            $("#spn").show()
            StatusT108.availableGetData = true;
            StatusT108.noFind = false;
            StatusT108.dataEmpty = false;
            T108GetInfo(0);
        } else {
            T108VerifyValidate();
        }
    },
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T108', ['Código de producto', 'Descripción Larga'], Task108.startTask, Task108.onClickBtnApplyChanges, Task108.onClickBtnGetInfo);