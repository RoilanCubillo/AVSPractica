/**
 * Tarea de cambio de impuestos de productos
 */

var StatusT104 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    taxNoFind: false
}, ListsTask104 = {
    ItemTaxs: [], Taxs: []
}, DataTask104 = {
    fields: [],
    columns: [
        { title: 'Código de producto' },
        { title: 'Descripción' },
        { title: 'Impuesto Actual', },
        { title: 'Valor Impuesto Actual (%)', },
        { title: 'Impuesto Nuevo', },
        { title: 'Valor Impuesto Nuevo (%)', },
        { title: 'Estado' },
        { title: '', visible: false, }
    ],
    columnsFile: ['Código de producto', 'Nuevo Impuesto (%)']
};

function T104GetInfo(index) {
    if (index < DataTask104.fields.length) {
        if (StatusT104.availableGetData) {
            try {
                StatusT104.gettingData = true;
                if (DataTask104.fields[index][5] === '') StatusT104.dataEmpty = true;
                $("#txt_spn").html("Obteniendo " + DataTask104.fields[index][0] + ". " + (index + 1) + " de " + DataTask104.fields.length);
                $.get(Task104.urls.getItem + "?itemLookupCode=" + DataTask104.fields[index][0]).done(function (data) {
                    if (data.Status) {
                        var itemTax = ListsTask104.ItemTaxs.find(x => x.ID === data.Result.TaxID);
                        var tax = itemTax ? ListsTask104.Taxs.find(x => x.ID === itemTax.TaxID1) : null;

                        DataTask104.fields[index][1] = data.Result.Description;
                        DataTask104.fields[index][2] = itemTax ? itemTax.Description : '-NO DEFINIDO-';
                        DataTask104.fields[index][3] = tax ? tax.Percentage : '-NO DEFINIDO-';
                        DataTask104.fields[index][7] = data.Result.ID;
                        DataTask104.fields[index][6] = '<span class="badge bg-primary">OBTENIDO</span>';
                        toastr.success('Producto ' + DataTask104.fields[index][0] + ' encontrado.');
                    } else {
                        DataTask104.fields[index][7] = '<span class="badge bg-danger">NO ESCONTRADO</span>';
                        StatusT104.noFind = true;
                        toastr.error('Producto ' + DataTask104.fields[index][0] + ' no encontrado.');
                    }
                    T104GetInfo(index + 1);
                }).fail(function () {
                    DataTask104.fields[index][7] = '<span class="badge bg-danger">ERROR OBTENER</span>';
                    T104GetInfo(index + 1);
                    StatusT104.dataErrors = true;
                    toastr.error('Error obteniendo ' + DataTask104.fields[index][0] + '.');
                });
            } catch (e) {
                DataTask104.fields[index][7] = '<span class="badge bg-danger">ERROR CONSULTA</span>';
                T104GetInfo(index + 1);
                StatusT104.dataErrors = true;
                toastr.error('Error intentando obtener ' + DataTask104.fields[index][0] + '.');
            }
        } else T104EndGetInfo();
    } else T104EndGetInfo();
}

function T104EndGetInfo() {
    StatusT104.gettingData = false;
    StatusT104.isGetted = true;
    StatusT104.applyChanges = !StatusT104.dataErrors && !StatusT104.noFind && StatusT104.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask104.columns, DataTask104.fields);
}

function T104GetItemsToStruct() {
    return DataTask104.fields.map(x => {
        return {
            ID: x[7],
            ItemLookupCode: x[0],
            Description: x[1],
            TaxPerAnterior: x[3],
            TaxPercentage: x[5]
        }
    })
}

function T104ApplyNewChanges(notes) {
    try {
        var list = T104GetItemsToStruct();
        $("#spn").show();
        $.post(Task104.urls.applyTask, { stores: taskConfig.stores, items: list, notes: notes }).done(function (data) {
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
            StatusT104.applingChanges = false;
        }).fail(function (xhr, ajaxOptions, thrownError) {
            error(thrownError, 'Error intentando aplicar cambios.');
            StatusT104.applingChanges = false;
        });
    } catch (e) {
        error(thrownError, 'Error intentando aplicar cambios.');
        StatusT104.applingChanges = false;
    }
}

var Task104 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        getTaxs: '../Wizard/GetTaxs',
        applyTask: '../Wizard/Task104'
    },

    startTask: function () {
        try {
            StatusT104.applyChanges = false;
            StatusT104.availableGetData = true;
            StatusT104.gettingData = false;
            StatusT104.dataErrors = false;
            StatusT104.isGetted = false;
            StatusT104.dataEmpty = false;
            StatusT104.noFind = false;
            StatusT104.taxNoFind = false;

            DataTask104.fields = csvData.map(x => {
                var tax = ListsTask104.Taxs.find(t => t.Percentage === (+x[1]));
                var itemTax = tax ? ListsTask104.ItemTaxs.find(t => t.TaxID1 === tax.ID) : null;
                if (tax && itemTax) return [x[0], '', '', '', itemTax.Description, tax.Percentage, '<span class="badge bg-yellow">NO CARGADO</span>', '']
                else {
                    StatusT104.taxNoFind = true;
                    return [x[0], '', '', '', '-NO DEFINIDO-', '-NO DEFINIDO-', '<span class="badge bg-danger">IMPUESTO NO EXISTE</span>', '']
                }
            });
            initTable(DataTask104.columns, DataTask104.fields);
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: async function () {
        if (!StatusT104.taxNoFind) {
            if (StatusT104.applyChanges && !StatusT104.applingChanges && !StatusT104.dataEmpty) {
                StatusT104.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then(async (result) => {
                    if (result.isConfirmed) {
                        const { value: text } = await getNotesTask();

                        if (text) T104ApplyNewChanges(text);
                        else StatusT104.applingChanges = false;
                    } else StatusT104.applingChanges = false;
                });
            } else {
                if (StatusT104.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT104.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT104.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT104.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT104.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT104.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT104.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else error("ERROR TAX NO FIND", 'Uno o varios productos poseen un porcentaje de impuesto erroneo. Modififique el documento cargado con información válida o comuníquese con el administrador.');
    },

    onClickBtnGetInfo: function () {
        if (!StatusT104.taxNoFind) {
            $("#spn").show()
            StatusT104.availableGetData = true;
            StatusT104.noFind = false;
            StatusT104.dataEmpty = false;
            T104GetInfo(0);
        } else error("ERROR TAX NO FIND", 'Uno o varios productos poseen un porcentaje de impuesto erroneo. Modififique el documento cargado con información válida o comuníquese con el administrador.');
    },
}

function T104GetTaxs() {
    try {
        $.get(Task104.urls.getTaxs).done(function (data) {
            if (data.Status) {
                ListsTask104.Taxs = data.Result.taxs;
                ListsTask104.ItemTaxs = data.Result.itemTaxs;
                Task104.startTask();
            } else {
                error(data.InternalMessage, data.Message);
                $("#spn").hide();
            }
        }).fail(function (e) {
            $("#spn").hide();
            error(e, '"No se puede continuar. Error al intentar obtener los impuestos del sistema. Contacte a Soporte."')
        })
    } catch (e) {
        $("#spn").hide();
        error(e, "No se puede continuar. Error al intentar obtener los impuestos del sistema. Contacte a Soporte.");
    }
}

setDataTaskConfiguration('T104', DataTask104.columnsFile, T104GetTaxs, Task104.onClickBtnApplyChanges, Task104.onClickBtnGetInfo);