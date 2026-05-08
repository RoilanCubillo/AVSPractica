/**
 * Tarea de cambio de activación de productos
 */

var StatusT131 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    supplierNotFound: false,
    errorUtility: false,
    errorInvDiscount: false,
    errorCusDiscount: false,
    errorCost: false,
    errorCode: false,
    errorDuplicated: false
}, ListsTask131 = {
    Suppliers: []
}, DataTask131 = {
    fields: [],
    columns: [
        { title: '', visible: false, }, // 0
        {
            title: 'Cod. de Producto', render: function (data, type, row, meta) {
                if (data === '') {
                    StatusT131.errorCode = true;
                    row[22] = row[22].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[22] = row[22].replaceAll(`<span class="badge bg-danger">CÓDIGO PRODUCTO NO INGRESADO</span>,`, '');
                    row[22] += `<span class="badge bg-danger">CÓDIGO PRODUCTO NO INGRESADO</span>,`;
                }
                return data;
            }
        }, // 1
        { title: 'Descripción del Producto<i class="fas fa-lock"></i>' }, // 2
        { title: 'Cod. Proveedor Primario<i class="fas fa-lock"></i>', visible: false, }, // 3
        { title: 'Nombre Proveedor Primario<i class="fas fa-lock"></i>', visible: false, }, // 4
        { title: 'Cod. Proveedor Nuevo', visible: false, }, // 5
        { title: 'Proveedor Nuevo', }, // 6
        { title: 'Costo Bruto Actual<i class="fas fa-lock"></i>', visible: false, render: function (data, type, row, meta) { return data; } }, // 7
        {
            title: 'Costo Bruto Nuevo', render: function (data, type, row, meta) {
                if (+data < 0) {
                    StatusT131.errorCost = true;
                    row[22] = row[22].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[22] = row[22].replaceAll(`<span class="badge bg-danger">COSTO NETO INVÁLIDO</span>,`, '');
                    row[22] += `<span class="badge bg-danger">COSTO NETO INVÁLIDO</span>,`;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        }, // 8
        { title: 'Utilidad Actual<i class="fas fa-lock"></i>', visible: false, }, // 9
        {
            title: 'Utilidad Nueva', render: function (data, type, row, meta) {
                if (+data < 0.01 && +data > 800) {
                    StatusT131.errorUtility = true;
                    row[22] = row[22].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[22] = row[22].replaceAll(`<span class="badge bg-danger">UTILIDAD INVÁLIDA</span>,`, '');
                    row[22] += `<span class="badge bg-danger">UTILIDAD INVÁLIDA</span>,`;
                }
                return `${data}%`;
            }
        }, // 10
        { title: 'Impuesto Específico<i class="fas fa-lock"></i>', visible: false, }, // 11
        {
            title: 'Desc. Factura(%)', render: function (data, type, row, meta) {
                if (+data < 0 && +data >= 100) {
                    row[22] = row[22].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[22] = row[22].replaceAll(`<span class="badge bg-danger">DESCUENTO DE FACTURA INVÁLIDO</span>,`, '');
                    row[22] += `<span class="badge bg-danger">DESCUENTO DE FACTURA INVÁLIDO</span>,`;
                    StatusT131.errorInvDiscount = true;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        }, // 12
        {
            title: 'Desc. Cliente(%)', render: function (data, type, row, meta) {
                if (+data < 0 && +data >= 100) {
                    StatusT131.errorCusDiscount = true;
                    row[22] = row[22].replaceAll('<span class="badge bg-yellow">NO VERIFICADO</span>', '');
                    row[22] = row[22].replaceAll(`<span class="badge bg-danger">DESCUENTO DE CLIENTE INVÁLIDO</span>,`, '');
                    row[22] += `<span class="badge bg-danger">DESCUENTO DE CLIENTE INVÁLIDO</span>,`;
                }
                return type == 'display' ? numberToMoney(data) : data;
            }
        }, // 13
        { title: 'IVA<i class="fas fa-lock"></i>', visible: false, }, // 14
        { title: 'Costo Compra Final' }, // 15
        { title: 'Precio Final (Utilidad)', visible: false, }, // 16
        { title: 'Precio Final (IVA)', visible: false, }, // 17
        { title: 'Utilidad Real', visible: false, }, // 18
        { title: 'Precio Real' }, //  19
        { title: 'Cód. Tiendas', visible: false }, //  20
        { title: 'Tiendas' }, //  21
        { title: 'Estado' } //  22
    ],
    columnsFile: [
        'Cód. Producto',
        'Cód. Proveedor',
        'Costo Bruto',
        'Utilidad(%)',
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)'
    ]
};

function T131SetItem(item) {
    dataItem = DataTask131.fields.find(y => y[1].trim().toUpperCase() === item.ItemLookupCode.trim().toUpperCase());
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
    dataItem[20] = item.StoresSelected;
    dataItem[21] = item.StoresNameSelected;
}

async function T131OnDoneGetInfo(data) {
    if (data.Status) {
        var dataMap = data.Result[0].Result.map(x => { return x.ItemLookupCode.toUpperCase(); });

        await DataTask131.fields.forEach(i => {
            if (dataMap.includes(i[1].toUpperCase())) i[22] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';
            else {
                StatusT131.noFind = true;
                i[22] = '<span class="badge bg-danger">PRODUCTO NO VERIFICADO</span>';
            }
        });

        await data.Result[0].Result.forEach(function (x) { T131SetItem(x); });
    } else {
        StatusT131.noFind = true;
        error(data.InternalMessage, data.Message);
    }
    T131EndGetInfo();
}

function T131GetInfo() {
    if (StatusT131.availableGetData) {
        try {
            StatusT131.gettingData = true;
            $("#txt_spn").html("Verificando productos..");
            $.post(Task131.urls.verify, { itemLookupCodes: DataTask131.fields.map(x => { return x[1] }) }).done(function (data) {
                T131OnDoneGetInfo(data);
            }).fail(function (e) {
                StatusT131.dataErrors = true;
                error(e, "Error intentando validar productos.");
                T131EndGetInfo();
            });
        } catch (e) {
            StatusT131.dataErrors = true;
            error(e, "Error intentando validar productos.");
            T131EndGetInfo();
        }
    } else T131EndGetInfo();
}

function T131EndGetInfo() {
    StatusT131.gettingData = false;
    StatusT131.isGetted = true;
    StatusT131.applyChanges = !StatusT131.dataErrors && !StatusT131.noFind && StatusT131.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask131.columns, DataTask131.fields);

    if (StatusT131.supplierNotFound)
        error("ERROR SUPPLIER NO DEFINED", 'No se puede continuar. Uno o varios productos no tienen un proveedor primario asignado');
    if (StatusT131.noFind)
        error("PRODUCTOS NO ENCONTRADOS", 'No se puede continuar. Uno o varios productos no pudieron ser verificados. Revise que los códigos coincidan con los productos del sistema.');
}

function T131GetItemsToStruct() {
    return DataTask131.fields.map(x => {
        var supplier = ListsTask131.Suppliers.find(s => s.Code === x[5]);
        return {
            ID: x[0],
            ItemLookupCode: x[1],
            SupplierID: supplier.ID,
            SupplierCode: x[5],
            GrossCost: x[8],
            Utility: x[10],
            InvoiceDiscount: x[12],
            CustomerDiscount: x[13]
        }
    })
}

async function T131ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            var func = function () {
                var list = T131GetItemsToStruct();
                $("#spn").show();
                $.post(Task131.urls.applyTask, { stores: taskConfig.stores, notes: taskValues, items: list }).done(function (data) {
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
                                console.error(r.InternalMessage);
                            }
                        })
                    } else error(data.InternalMessage, data.Message)
                    $("#spn").hide();
                    StatusT131.applingChanges = false;
                }).fail(function (xhr, ajaxOptions, thrownError) {
                    error(thrownError, 'Error intentando aplicar cambios.');
                    StatusT131.applingChanges = false;
                });
            };

            onActionApplyTask(func);
        } else StatusT131.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT131.applingChanges = false;
    }
}

function T131VerifyErrors() {
    if (StatusT131.supplierNotFound) error("ERROR SUPPLIER NO FIND", 'Uno o varios productos poseen un PROVEEDOR erroneo. Intente: actualizar e intentar de nuevo, modificar el documento cargado con información válida, o comuníquese con el administrador.');
    if (StatusT131.errorCost) error("ERROR COST INVALID", 'Uno o varios productos poseen una costo erroneo. Verifique que sea mayor/igual a 0.');
    if (StatusT131.errorUtility) error("ERROR UTILITY INVALID", 'Uno o varios productos poseen una utilidad erroneo. Verifique que esté de 0~100.');
    if (StatusT131.errorInvDiscount) error("ERROR INVOICEDISCOUNT INVALID", 'Uno o varios productos poseen un descuento de factura erroneo. Verifique que esté de 0~99.');
    if (StatusT131.errorCusDiscount) error("ERROR CUSTOMERDISCOUNT INVALID", 'Uno o varios productos poseen un descuento de cliente erroneo. Verifique que esté de 0~99.');
    if (StatusT131.errorCode) error("ERROR ITEMLOOKUPCODE EMPTY", 'Uno o varias líneas no tienen asignado un código de producto.');
    if (StatusT131.errorDuplicated) error("ERROR ITEMLOOKUPCODE duplicado", 'Uno o varias líneas tienen asignado un mismo código de producto.');
}

var Task131 = {
    urls: {
        getItem: '../Wizard/GetItemByItemLookupCode',
        getSuppliers: '../Wizard/GetSuppliers',
        applyTask: '../Wizard/Task131',
        verify: '../Wizard/Task131Verificar'
    },

    startTask: async function () {
        try {
            StatusT131.applyChanges = false;
            StatusT131.availableGetData = true;
            StatusT131.gettingData = false;
            StatusT131.dataErrors = false;
            StatusT131.isGetted = false;
            StatusT131.dataEmpty = false;
            StatusT131.noFind = false;
            StatusT131.supplierNotFound = false;
            StatusT131.errorCost = false;
            StatusT131.errorUtility = false;
            StatusT131.errorInvDiscount = false;
            StatusT131.errorCusDiscount = false;
            StatusT131.errorCode = false;
            StatusT131.errorDuplicated = false;

            DataTask131.fields = csvData.map(x => {
                var count = 0;

                for (var i = 0; i < csvData.length; i++) {
                    if (x[0].toUpperCase() === csvData[i][0].toUpperCase()) {
                        count++;
                    }
                }

                var errorDup = '';

                if (count > 1) {
                    errorDup = `<span class"badge bg-danger">CÓDIGO (${x[0]}) DUPLICADO`;
                    StatusT131.errorDuplicated = true;
                }

                var supplier = ListsTask131.Suppliers.find(s => s.Code === x[1]);
                if (supplier)
                    return ['', x[0], '-', '-', '-', x[1], supplier.Name, '-', (+x[2]), '-', +x[3], '-', +x[4], +x[5], '-', '-', '-', '-', '-', '-', '-', '-', (count > 1 ? errorDup : '<span class="badge bg-yellow">NO VERIFICADO</span>')]
                else {
                    StatusT131.supplierNotFound = true;
                    return ['', x[0], '-', '-', '-', x[1], '-ERROR-', '-', (+x[2]), '-', +x[3], '-', +x[4], +x[5], '-', '-', '-', '-', '-', '-', '-', '-', `${errorDup}<span class="badge bg-danger">PROVEEDOR NO EXISTE</span>`]
                }
            });

            await initTable(DataTask131.columns, DataTask131.fields);

            T131VerifyErrors();

            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (
            !StatusT131.supplierNotFound && !StatusT131.errorCost && !StatusT131.errorUtility
            && !StatusT131.errorInvDiscount && !StatusT131.errorCusDiscount && !StatusT131.errorCode
            && !StatusT131.errorDuplicated
        ) {
            if (StatusT131.applyChanges && !StatusT131.applingChanges && !StatusT131.dataEmpty) {
                StatusT131.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T131ApplyNewChanges();
                    else StatusT131.applingChanges = false;
                });
            } else {
                if (StatusT131.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT131.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT131.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT131.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT131.isGetted) toastr.error("No se puede continuar. Primero debe de obtener la información actual de base de datos.");
                if (StatusT131.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT131.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
            }
        } else {
            T131VerifyErrors();
        }
    },

    onClickBtnGetInfo: function () {
        if (
            !StatusT131.supplierNotFound && !StatusT131.errorCost && !StatusT131.errorUtility
            && !StatusT131.errorInvDiscount && !StatusT131.errorCusDiscount && !StatusT131.errorCode
            && !StatusT131.errorDuplicated
        ) {
            $("#spn").show()
            StatusT131.availableGetData = true;
            StatusT131.noFind = false;
            StatusT131.dataEmpty = false;
            T131GetInfo();
        } else {
            T131VerifyErrors();
        }
    },
}

function T131GetSuppliers() {
    try {
        $.get(Task131.urls.getSuppliers).done(function (data) {
            if (data.Status) {
                ListsTask131.Suppliers = data.Result.suppliers;
                Task131.startTask();
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

setDataTaskConfiguration('T131', DataTask131.columnsFile, T131GetSuppliers, Task131.onClickBtnApplyChanges, Task131.onClickBtnGetInfo);