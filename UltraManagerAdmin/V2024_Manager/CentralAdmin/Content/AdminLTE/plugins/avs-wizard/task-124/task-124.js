/**
 * Tarea de Cambio de Precios: Margen de Utilidad
 */
var StatusT124 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,

    codeEmpty: false,
    duplicatedError: false,
    utilityError: false,

    gettedError: false,

    supplierError: false
}, DataTask124 = {
    fields: [],
    columns: [
        { title: 'Cód. Producto' },
        { title: 'Descripción<i class="fas fa-lock"></i>' },
        { title: 'Proveedor<i class="fas fa-lock"></i>' },
        {
            title: 'Utilidad Nueva', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-primary">${data}%</span>` : data;
            },
        },
        { title: 'Costo Neto<i class="fas fa-lock"></i>' },
        { title: 'Impuesto Específico<i class="fas fa-lock"></i>' },
        { title: 'Descuento al cliente<i class="fas fa-lock"></i>' },
        { title: 'Impuesto al artículo<i class="fas fa-lock"></i>' },
        { title: 'Precio Final (Utilidad)' },
        {
            title: 'Precio Final (IVA)', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-warning">${data}</span>` : data;
            },
        },
        {
            title: 'Utilidad Actual<i class="fas fa-lock"></i>', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-primary">${data}</span>` : data;
            },
        },
        { title: 'Precio Actual (Utilidad)<i class="fas fa-lock"></i>' },
        {
            title: 'Precio Actual (IVA)<i class="fas fa-lock"></i>', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-warning">${data}</span>` : data;
            },
        },
        {
            title: 'Utilidad Real', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-success">${data}</span>` : data;
            },
        },
        {
            title: 'Precio Final Real', render: function (data, type, row, meta) {
                return type === 'display' ? `<span class="badge bg-success">${data}</span>` : data;
            },
        },
        { title: 'Estado' },
        { title: '', visible: false },
    ],
    columnsFile: ['Cod. Producto', 'Utilidad'],
};

/** INICIALIZA LOS ESTADOS */
function T124InitStatus() {
    StatusT124.applyChanges = false;
    StatusT124.availableGetData = true;
    StatusT124.gettingData = false;
    StatusT124.dataErrors = false;
    StatusT124.isGetted = false;
    StatusT124.dataEmpty = false;
    StatusT124.noFind = false;

    StatusT124.codeEmpty = false;
    StatusT124.duplicatedError = false;
    StatusT124.utilityError = false;
    StatusT124.gettedError = false;
    StatusT124.supplierError = false;
}

async function T124CsvToTaskTable() {
    DataTask124.fields = csvData.map(x => {
        return [
            x[0], // Cod. Producto
            '-', // Descripción
            '-',  // Proveedor
            x[1], // Utilidad Nueva
            numberToMoney('-'), // Costo Neto
            numberToMoney('-'), // Específico
            '-%', // Desc. Cliente
            '-%', // IVA
            numberToMoney('-'), // Precio final utilidad
            numberToMoney('-'), // Precio final iva
            '-%', // Utilidad actual
            numberToMoney('-'), // Precio final actual utilidad
            numberToMoney('-'), // Precio final actual iva
            '-%', // Utilidad real
            numberToMoney('-'), // Precio final real
            '<span class="badge bg-primary">FALTA VERIFICAR</span>', // Estado
            0
        ];
    });
}

async function T124ValidateDataFields() {
    DataTask124.fields.forEach(async function (x) {
        var count = 0, utility = +x[3];

        if (x[0] !== '') {
            for (var i = 0; i < DataTask124.fields.length; i++) {
                if (DataTask124.fields[i][0] === x[0]) count++;
            }
        } else {
            StatusT124.codeEmpty = true;
            x[15] = x[15].replaceAll('<span class="badge bg-primary">FALTA VERIFICAR</span>', '');
            x[15] += `<span class="badge bg-danger">CÓDIGO DE PRODUCTO NO INGRESADO</span>`;
        }

        if (count > 1) {
            StatusT124.duplicatedError = true;
            x[15] = x[15].replaceAll('<span class="badge bg-primary">FALTA VERIFICAR</span>', '');
            x[15] += `<span class="badge bg-danger">PRODUCTO (${x[0]}) DUPLICADO</span>`;
        }

        if (utility <= 0 || utility > 800) {
            StatusT124.utilityError = true;
            x[15] = x[15].replaceAll('<span class="badge bg-primary">FALTA VERIFICAR</span>', '');
            x[15] += `<span class="badge bg-danger">(${x[2]}%) Utilidad Inválida</span>`;
        }
    });
}

/** CONSULTA TODOS LOS POSIBLES ERRORES EN LOS DATOS SUMISTRADOS Y SE LOS MUESTRA AL USUARIO */
async function T124ValidateDataTask() {
    if (StatusT124.utilityError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Utilidad.', body: 'Uno o varios productos tienen una utilida inválida. Verifique que esté entre 0~800.' });
    if (StatusT124.codeEmpty) $(document).Toasts('create', { class: 'bg-danger', title: 'Faltan códigos.', body: 'Una o varias líneas se ingresaron sin código del producto.' });
    if (StatusT124.duplicatedError) $(document).Toasts('create', { class: 'bg-danger', title: 'Productos duplicados.', body: 'Una o varias líneas referencian a un mismo producto.' });
    if (StatusT124.gettedError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error de verificación de productos.', body: 'Uno o varias productos tuvieron problemas al ser verificados. Revise los estados.' });
    if (StatusT124.supplierError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en proveedor del producto.', body: 'Uno o varias productos no tienen un proveedor primario asignado.' });
}

/** HACE MAPEO DE UN LISTADO DE ESTRUCTURAS DE PRODUCTOS PARA EJECUTAR LA TAREA DEL LISTADO DE PRODUCTOS EN EL TABLE DEL TASK */
function T124GetItemsToStruct() {
    return DataTask124.fields.map(x => { return { ID: x[16], ItemLookupCode: x[0], Utility: x[3] === '' ? 0 : x[3] } })
}

function T124SetDataItem(result) {
    var item = result;

    var field = DataTask124.fields.find(x => x[0].trim().toUpperCase() === item.ItemLookupCode.trim().toUpperCase());


    try {
        var costo = item.ReplacementCost;
        var msrp = item.MSRP;
        var cusdiscount = item.CustomerDiscount;
        var iva = item.TaxPercentage;
        var utility = +field[3];

        var finalPrice = (costo - (costo * (cusdiscount / 100)) + msrp);
        var finalPriceUtility = (finalPrice * (1 + (utility / 100)));
        var mount_tax = finalPriceUtility * (iva / 100);
        var finalPriceIVA = (finalPriceUtility + mount_tax);

        var price_real = Math.round(finalPriceIVA / 5) * 5;
        var utility_real = (((price_real - mount_tax) / finalPrice) - 1) * 100;

        if (item.SupplierID > 0 || item.iD > 0) {
            var priceUtility = finalPrice * (1 + (item.Utility / 100));
            field[1] = item.Description;
            field[2] = `${item.SupplierCode}-${item.SupplierName}`;

            field[4] = numberToMoney(costo);
            field[5] = numberToMoney(msrp);
            field[6] = `${cusdiscount}%`;
            field[7] = `${iva}%`;
            field[8] = numberToMoney(finalPriceUtility);
            field[9] = numberToMoney(finalPriceIVA);
            field[10] = `${item.Utility}%`;
            field[11] = numberToMoney(priceUtility);
            field[12] = numberToMoney(priceUtility * (1 + (iva / 100)));
            field[13] = `${utility_real}%`;
            field[14] = numberToMoney(price_real);
            field[16] = item.ID;
        } else {
            StatusT124.supplierError = true;
            error("PROVEEDOR / PRODUCTO ", `El producto (${item.ItemLookupCode}) no existe o no tiene proveedor asignado.`);
            field[15] = `<span class="badge bg-danger">Producto sin Proveedor Primario</span>`;
        }

    } catch (e) {
        console.log(e);
    }


}

async function T124OnDoneGetInfo(data) {
    if (data.Status) {
        var dataMap = data.Result[0].Result.map(x => { return x.ItemLookupCode.toUpperCase(); });

        await DataTask124.fields.forEach(i => {
            if (dataMap.includes(i[0].toUpperCase())) i[15] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';
            else {
                StatusT124.noFind = true;
                i[15] = '<span class="badge bg-danger">PRODUCTO NO VERIFICADO</span>';
            }
        });

        await data.Result[0].Result.forEach(function (x) { T124SetDataItem(x); });
    } else error(data.InternalMessage, data.Message);
    T124EndGetInfo();
}

/** HACE POST CON EL LISTADO DE CÓDIGOS DE PRODUCTOS PARA REVISAR QUE TODO ESTÁ BIEN CON EL SISTEMA */
function T124GetInfo() {
    if (StatusT124.availableGetData) {
        try {
            StatusT124.gettingData = true;
            $("#txt_spn").html("Verificando productos...");
            $.post(Task124.urls.verify, { itemLookupCodes: DataTask124.fields.map(x => { return x[0] }) }).done(function (data) {
                T124OnDoneGetInfo(data);
            }).fail(function (e) {
                StatusT124.dataErrors = true;
                error("ERROR CONSULTA", "Error intentando validar productos.");
                T124EndGetInfo();
            });
        } catch (e) {
            StatusT124.dataErrors = true;
            error("ERROR CONSULTA", "Error intentando validar productos.");
            T124EndGetInfo();
        }
    } else T124EndGetInfo();
}

/** EVENTO EJECUTADO CUANDO SE FINALIZA LA VERIFICACIÓN DE LA LECTURA DEL RESULTADO DEL MÉTODO T124OnDoneGetInfo Y ACTUALIZA UNOS ESTADOS */
function T124EndGetInfo() {
    StatusT124.gettingData = false;
    StatusT124.isGetted = true;
    StatusT124.applyChanges = !StatusT124.dataErrors && !StatusT124.noFind && StatusT124.availableGetData && !StatusT124.gettedError;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask124.columns, DataTask124.fields);
    T124ValidateDataTask();

    if (StatusT124.noFind)
        error("PRODUCTOS NO ENCONTRADOS", 'No se puede continuar. Uno o varios productos no pudieron ser verificados. Revise que los códigos coincidan con los productos del sistema.');
}

function T124OnDoneApplyChanges(data) {
    if (data.Status) {
        $(document).Toasts('create', { class: 'bg-success', title: 'Creación de hoja de trabajo', subtitle: '', body: data.Message });
    } else error(data.InternalMessage, data.Message);
    $("#spn").hide();
    StatusT124.applingChanges = false;
}

/** ACCIONA EL APLICAR CAMBIOS EN EL SISTEMA */
async function T124ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            var list = T124GetItemsToStruct();
            $("#spn").show();

            $.ajax({
                url: Task124.urls.applyTask, type: 'post',
                data: {
                    stores: taskConfig.stores,
                    items: list,
                    notes: taskValues
                }, cache: false,
                success: function (data) {
                    T124OnDoneApplyChanges(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    error(thrownError, 'Error intentando aplicar cambios.');
                    StatusT124.applingChanges = false;
                    $("#spn").hide();
                }
            });
        } else StatusT124.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT124.applingChanges = false;
    }
}

/** ALMACENA DATOS Y FUNCIONES DEL TASK. SIRVE PARA QUE LA VISTA SE CONECTE CON ESTA TAREA EN ESPECÍFICO. (TODO ES UN ESTANDAR) */
var Task124 = {
    /** URLS PROPIAS DEL TASK */
    urls: { applyTask: '../Wizard/Task124', verify: '../Wizard/Task124Verificar' },

    /** FUNCIÓN INITIALIZE DEL TASK */
    startTask: async function () {
        $(async function () {
            try {
                T124InitStatus();
                await T124CsvToTaskTable();
                await T124ValidateDataFields();
                await T124ValidateDataTask();
                await initTable(DataTask124.columns, DataTask124.fields);
                stepper.next();
                $("#spn").hide();
            } catch (e) {
                error(e, "Error al inicializar la tarea. Contacte a soporte.");
                $("#spn").hide();
            }
        });
    },

    /** EVENTO DE CUANDO SE LE DA CLICK AL BOTÓN DE APLICAR CAMBIOS (LA VISTA SE ENCARGA DE EJECUTARLO) */
    onClickBtnApplyChanges: function () {
        if (!StatusT124.utilityError && !StatusT124.codeEmpty && !StatusT124.duplicatedError && !StatusT124.gettedError && !StatusT124.supplierError) {
            if (StatusT124.applyChanges && !StatusT124.applingChanges && !StatusT124.dataEmpty && !StatusT124.dataErrors && !StatusT124.itemExists) {
                StatusT124.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se aplicarán crearán los productos ingresados...", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T124ApplyNewChanges();
                    else StatusT124.applingChanges = false;
                });
            } else {
                if (StatusT124.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT124.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT124.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT124.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT124.isGetted) toastr.error("No se puede continuar. Primero debe de verificar los datos.");
                if (StatusT124.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT124.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
                if (StatusT124.itemExists) toastr.error("No se puede continuar. Uno o varios códigos de productos ya existen en el sistema.");
            }
        } else T124ValidateDataTask();
    },

    /** EVENTO QUE SE EJECUTA CUANDO SE LE DA CLICK AL BOTÓN DE VERIFICAR LA INFO (LA VISTA SE ENCARGA DE EJECUATARLO POR SÍ SOLA) */
    onClickBtnGetInfo: function () {
        if (!StatusT124.utilityError && !StatusT124.codeEmpty && !StatusT124.duplicatedError) {
            $("#spn").show()
            StatusT124.availableGetData = true;
            StatusT124.noFind = false;
            StatusT124.dataEmpty = false;
            StatusT124.itemExists = false;
            StatusT124.dataErrors = false;
            StatusT124.gettedError = false;
            StatusT124.supplierError = false;
            T124GetInfo();
        } else T124ValidateDataTask();
    },
}

/** AQUÍ SE CONFIGURA AL WIZARD LOS EVENTOS Y REQUERIMIENTOS PARA LA EJECUCIÓN DE ESTA TAREA */
/** SI ESTO NO SE DEFINE, LA TAREA NO SE EJECUTARÁ */
/** CABE DESTACAR QUE, EL LA VISTA DEL WIZAR SE DEBE INCORPORAR ESTE .js */
setDataTaskConfiguration(
    'T124', // NOMBRE DEL TASK (ESTANDAR T<COD. DE TAREA>) //
    DataTask124.columnsFile, // COLUMNAS QUE REQUERIRÁ EN EL CSV A LEER //
    Task124.startTask, // EVENTO INICIAL DEL TASK (SE EJECUTA CUANDO SE DA EN SIGUIENTE, DESPUÉS DE CARGAR EL CSV) //
    Task124.onClickBtnApplyChanges, // EVENTO PARA CUANDO SE DE CLICK EN BOTÓN APLICAR CAMBIOS //
    Task124.onClickBtnGetInfo // EVENTO PARA CUANDO SE DE CLICK EN BOTÓN VERIFICAR INFO //
);