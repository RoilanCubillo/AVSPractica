/**
 * Tarea de cambio de costos - proveedor
 */

var StatusT120 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    supplierNotFound: false,
    errorDate: false,
    errorUtility: false,
    errorInvDiscount: false,
    errorCusDiscount: false,
    errorCost: false,
    errorCode: false,
    errorDuplicated: false
}, ListsTask120 = {
    Suppliers: []
}, DataTask120 = {
    fields: [],
    columns: [
        { title: '', visible: false, },         //  0
        {
            title: 'Cod. de Producto', render: function (data, type, row, meta) {
                if (data === '') {
                    StatusT120.errorCode = true;
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">CÓDIGO PRODUCTO NO INGRESADO</span>,`, '');
                    row[21] += `<span class="badge bg-danger">CÓDIGO PRODUCTO NO INGRESADO</span>,`;
                }
                return data;
            }
        },          //  1
        { title: 'Descripción del Producto<i class="fas fa-lock"></i>' },  //  2
        { title: 'Cod. Proveedor Primario<i class="fas fa-lock"></i>', },  //  3
        { title: 'Nombre Proveedor Primario<i class="fas fa-lock"></i>', },//  4
        { title: 'Cod. Proveedor Nuevo', },     //  5
        { title: 'Proveedor Nuevo', },          //  6
        {
            title: 'Costo Neto Actual<i class="fas fa-lock"></i>', render: function (data, type, row, meta) {
                return data;
            }
        },              //  7
        {
            title: 'Costo Neto Nuevo', render: function (data, type, row, meta) {
                if (+data < 0) {
                    StatusT120.errorCost = true;
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">COSTO NETO INVÁLIDO</span>,`, '');
                    row[21] += `<span class="badge bg-danger">COSTO NETO INVÁLIDO</span>,`;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        },
        { title: 'Utilidad Actual<i class="fas fa-lock"></i>' },           //  9
        {
            title: 'Utilidad Nueva', render: function (data, type, row, meta) {
                if (+data < 0.01 && +data > 800) {
                    StatusT120.errorUtility = true;
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">UTILIDAD INVÁLIDA</span>,`, '');
                    row[21] += `<span class="badge bg-danger">UTILIDAD INVÁLIDA</span>,`;
                }
                return `${data}%`;
            }
        },            //  9
        { title: 'Impuesto Específico' },
        {
            title: 'Desc. en Factura(%)', render: function (data, type, row, meta) {
                if (+data < 0 && +data >= 100) {
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">DESCUENTO DE FACTURA INVÁLIDO</span>,`, '');
                    row[21] += `<span class="badge bg-danger">DESCUENTO DE FACTURA INVÁLIDO</span>,`;
                    StatusT120.errorInvDiscount = true;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        },       //  10
        {
            title: 'Desc. para Cliente(%)', render: function (data, type, row, meta) {
                if (+data < 0 && +data >= 100) {
                    StatusT120.errorCusDiscount = true;
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">DESCUENTO DE CLIENTE INVÁLIDO</span>,`, '');
                    row[21] += `<span class="badge bg-danger">DESCUENTO DE CLIENTE INVÁLIDO</span>,`;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        },     //  11
        { title: 'IVA<i class="fas fa-lock"></i>' },     //  11
        { title: 'Costo Compra Final' },
        { title: 'Precio Final (Utilidad)' },
        { title: 'Precio Final (IVA)' },
        { title: 'Utilidad Real' },
        { title: 'Precio Real' },
        {
            title: 'Fecha Inicio', render: function (data, type, row, meta) {
                if (isNaN(moment(data, "DD/MM/YYYY").toDate())) {
                    StatusT120.errorDate = true;
                    row[21] = row[21].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[21] = row[21].replaceAll(`<span class="badge bg-danger">FECHA INICIO INVÁLIDA</span>,`, '');
                    row[21] += `<span class="badge bg-danger">FECHA INICIO INVÁLIDA</span>,`;
                    return '<span class="badge bg-danger">' + data + ' ERRONEA</span>'
                } else return data;
            }
        },                                      //  13
        { title: 'Estado' }                     //  14
    ],
    columnsFile: [
        'Codigo de Producto',
        'Codigo de Proveedor',
        'Costo', 'Utilidad(%)',
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)',
        'Fecha de Inicio(dd/MM/yyyy)'
    ]
};

function T120SetItem(item) {
    dataItem = DataTask120.fields.find(y => y[1].trim().toUpperCase() === item.ItemLookupCode.trim().toUpperCase());
    var _cost = +dataItem[8];
    var _utilty = +dataItem[10];
    var _invDiscount = +dataItem[12];
    var _cusDiscount = +dataItem[13];
    var _msrp = +item.MSRP;
    var _tax = +item.TaxPercentage;
    // PARA CÁLCULOS DE COSTOS Y PRECIOS //
    var final_cost = (_cost - (_cost * (_invDiscount / 100)) + _msrp);
    var final_price = (_cost - (_cost * (_cusDiscount / 100)) + _msrp);
    var final_price_utility = (final_price * (1 + (_utilty / 100)));
    var mount_iva = final_price_utility * (_tax / 100);
    var final_price_iva = (final_price_utility + mount_iva);
    var real_price = Math.round(final_price_iva / 5) * 5;
    var real_utility = (((real_price - mount_iva) / final_price) - 1) * 100;
    // APLICAR CÁLCULOS Y DATOS DEL PRODUCTO ACTUALMENTE //
    dataItem[0] = item.ID;
    dataItem[1] = item.ItemLookupCode;
    dataItem[2] = item.Description;
    dataItem[3] = item.SupplierCode;
    dataItem[4] = item.SupplierName;
    dataItem[7] = numberToMoney(item.ReplacementCost);
    dataItem[9] = `${item.Utility}%`;
    dataItem[11] = numberToMoney(_msrp);
    dataItem[14] = `${item.TaxPercentage}%`;
    dataItem[15] = numberToMoney(final_cost);
    dataItem[16] = numberToMoney(final_price_utility);
    dataItem[17] = numberToMoney(final_price_iva);
    dataItem[18] = `${real_utility}%`;
    dataItem[19] = numberToMoney(real_price);
}

async function T120OnDoneGetInfo(data) {
    if (data.Status) {
        var dataMap = data.Result[0].Result.map(x => { return x.ItemLookupCode.toUpperCase(); });

        await DataTask120.fields.forEach(i => {
            if (dataMap.includes(i[1].toUpperCase())) i[21] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';
            else {
                StatusT120.noFind = true;
                i[21] = '<span class="badge bg-danger">PRODUCTO NO VERIFICADO</span>';
            }
        });

        await data.Result[0].Result.forEach(function (x) { T120SetItem(x); });
    } else {
        StatusT120.noFind = true;
        error(data.InternalMessage, data.Message);
    }
    T120EndGetInfo();
}

function T120GetInfo() {
    if (StatusT120.availableGetData) {
        try {
            StatusT120.gettingData = true;
            $("#txt_spn").html("Verificando productos..");
            $.post(Task120.urls.verify, { itemLookupCodes: DataTask120.fields.map(x => { return x[1] }) }).done(function (data) {
                T120OnDoneGetInfo(data);
            }).fail(function (e) {
                StatusT120.dataErrors = true;
                error(e, "Error intentando validar productos.");
                T120EndGetInfo();
            });
        } catch (e) {
            StatusT120.dataErrors = true;
            error(e, "Error intentando validar productos.");
            T120EndGetInfo();
        }
    } else T120EndGetInfo();
}

function T120EndGetInfo() {
    StatusT120.gettingData = false;
    StatusT120.isGetted = true;
    StatusT120.applyChanges = !StatusT120.dataErrors && !StatusT120.noFind && StatusT120.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask120.columns, DataTask120.fields);

    if (StatusT120.supplierNotFound)
        error("ERROR SUPPLIER NO DEFINED", 'No se puede continuar. Uno o varios productos no tienen un proveedor primario asignado');
    if (StatusT120.noFind)
        error("PRODUCTOS NO ENCONTRADOS", 'No se puede continuar. Uno o varios productos no pudieron ser verificados. Revise que los códigos coincidan con los productos del sistema.');
}

function T120GetItemsToStruct() {
    return DataTask120.fields.map(x => {
        return {
            ID: x[0],
            ItemLookupCode: x[1],
            SupplierCode: x[5],
            Cost: x[8],
            Utility: x[10],
            InvoiceDiscount: x[12],
            CustomerDiscount: x[13],
            StartDate: x[20],
        }
    })
}

async function T120ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            var func = function () {
                var list = T120GetItemsToStruct();
                $("#spn").show();
                $.post(Task120.urls.applyTask, { stores: taskConfig.stores, notes: taskValues, items: list }).done(function (data) {
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
                            else {
                                $(document).Toasts('create', {
                                    class: 'bg-danger',
                                    title: 'Creación de hoja de trabajo',
                                    subtitle: '',
                                    body: r.Message
                                });
                            }
                        })
                    } else error(data.InternalMessage, data.Message)
                    $("#spn").hide();
                    StatusT120.applingChanges = false;
                }).fail(function (xhr, ajaxOptions, thrownError) {
                    error(thrownError, 'Error intentando aplicar cambios.');
                    StatusT120.applingChanges = false;
                });
            }

            onActionApplyTask(func);
        } else StatusT120.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT120.applingChanges = false;
    }
}

function T120VerifyErrors() {
    if (StatusT120.supplierNotFound) error("ERROR SUPPLIER NO FIND", 'Uno o varios productos poseen un PROVEEDOR erroneo. Intente: actualizar e intentar de nuevo, modificar el documento cargado con información válida, o comuníquese con el administrador.');
    if (StatusT120.errorDate) error("ERROR DATE INVALID", 'Uno o varios productos poseen una fecha erronea. Recuerde que el formato permitido es DD/MM/AAAA.');
    if (StatusT120.errorCost) error("ERROR COST INVALID", 'Uno o varios productos poseen una costo erroneo. Verifique que sea mayor/igual a 0.');
    if (StatusT120.errorUtility) error("ERROR UTILITY INVALID", 'Uno o varios productos poseen una utilidad erroneo. Verifique que esté de 0~100.');
    if (StatusT120.errorInvDiscount) error("ERROR INVOICEDISCOUNT INVALID", 'Uno o varios productos poseen un descuento de factura erroneo. Verifique que esté de 0~99.');
    if (StatusT120.errorCusDiscount) error("ERROR CUSTOMERDISCOUNT INVALID", 'Uno o varios productos poseen un descuento de cliente erroneo. Verifique que esté de 0~99.');
    if (StatusT120.errorCode) error("ERROR ITEMLOOKUPCODE EMPTY", 'Uno o varias líneas no tienen asignado un código de producto.');
    if (StatusT120.errorDuplicated) error("ERROR ITEMLOOKUPCODE duplicado", 'Uno o varias líneas tienen asignado un mismo código de producto.');
}

var Task120 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        getSuppliers: '../Wizard/GetSuppliers',
        applyTask: '../Wizard/Task120',
        verify: '../Wizard/Task120Verificar'
    },

    startTask: async function () {
        try {
            StatusT120.applyChanges = false;
            StatusT120.availableGetData = true;
            StatusT120.gettingData = false;
            StatusT120.dataErrors = false;
            StatusT120.isGetted = false;
            StatusT120.dataEmpty = false;
            StatusT120.noFind = false;
            StatusT120.supplierNotFound = false;
            StatusT120.errorDate = false;
            StatusT120.errorCost = false;
            StatusT120.errorUtility = false;
            StatusT120.errorInvDiscount = false;
            StatusT120.errorCusDiscount = false;
            StatusT120.errorCode = false;
            StatusT120.errorDuplicated = false;

            DataTask120.fields = csvData.map(x => {
                var count = 0;

                for (var i = 0; i < csvData.length; i++) {
                    if (x[0].toUpperCase() === csvData[i][0].toUpperCase()) {
                        count++;
                    }
                }

                var errorDup = '';

                if (count > 1) {
                    errorDup = `<span class"badge bg-danger">CÓDIGO (${x[0]}) DUPLICADO`;
                    StatusT120.errorDuplicated = true;
                }

                var supplier = ListsTask120.Suppliers.find(s => s.Code === x[1]);
                if (supplier)
                    return ['', x[0], '-', '-', '-', x[1], supplier.Name, '-', (+x[2]), '-', +x[3], '-', +x[4], +x[5], '-', '-', '-', '-', '-', '-', x[6], (count > 1 ? errorDup : '<span class="badge bg-yellow">NO VERIFICADO</span>')]
                else {
                    StatusT120.supplierNotFound = true;
                    return ['', x[0], '-', '-', '-', x[1], '-ERROR-', '-', (+x[2]), '-', +x[3], '-', +x[4], +x[5], '-', '-', '-', '-', '-', '-', x[6], `${errorDup}<span class="badge bg-danger">PROVEEDOR NO EXISTE</span>`]
                }
            });

            await initTable(DataTask120.columns, DataTask120.fields);

            T120VerifyErrors();

            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (
            !StatusT120.supplierNotFound && !StatusT120.errorDate && !StatusT120.errorCost && !StatusT120.errorUtility
            && !StatusT120.errorInvDiscount && !StatusT120.errorCusDiscount && !StatusT120.errorCode
            && !StatusT120.errorDuplicated
        ) {
            if (StatusT120.applyChanges && !StatusT120.applingChanges && !StatusT120.dataEmpty) {
                StatusT120.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T120ApplyNewChanges();
                    else StatusT120.applingChanges = false;
                });
            } else {
                if (StatusT120.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT120.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT120.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT120.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT120.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT120.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT120.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else {
            T120VerifyErrors();
        }
    },

    onClickBtnGetInfo: function () {
        if (
            !StatusT120.supplierNotFound && !StatusT120.errorDate && !StatusT120.errorCost && !StatusT120.errorUtility
            && !StatusT120.errorInvDiscount && !StatusT120.errorCusDiscount && !StatusT120.errorCode
            && !StatusT120.errorDuplicated
        ) {
            $("#spn").show()
            StatusT120.availableGetData = true;
            StatusT120.noFind = false;
            StatusT120.dataEmpty = false;
            T120GetInfo();
        } else {
            T120VerifyErrors();
        }
    },
}

function T120GetSuppliers() {
    try {
        $.get(Task120.urls.getSuppliers).done(function (data) {
            if (data.Status) {
                ListsTask120.Suppliers = data.Result.suppliers;
                Task120.startTask();
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
}

setDataTaskConfiguration('T120', DataTask120.columnsFile, T120GetSuppliers, Task120.onClickBtnApplyChanges, Task120.onClickBtnGetInfo);