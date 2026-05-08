/**
 * Tarea de cambio de propiedades de productos
 */

var DataTask201 = {
    fields: [],
    columns: [],
    columnsFile: [],
    AllProps: [],
    PropsSelected: [],
    CodsDuplicated: []
};

var StatusT201 = {
    isVerified: false,
    isVerifying: false,
    isApplying: false,
    haveDuplicated: false,
    havePropsEmpty: false
}

function T201VerifyAllowApply() {
    return StatusT201.isVerified && !StatusT201.isApplying && !StatusT201.isVerifying;
}

function T201ShowVerifyApplyErrors(isVerify = false) {
    if (!StatusT201.isVerified && !isVerify) showErrorToasts('', 'No se puede continuar, los productos aún no han sido verificados.');
    if (StatusT201.isVerifying) showErrorToasts('Proceso en ejecución', `Ya existe un proceso de verificación de productos ejecutándose, espere a que finalice.`);
    if (StatusT201.haveDuplicated) showErrorToasts('Líneas duplicadas', `Varias líneas contienen el mismo código de producto. [${DataTask201.CodsDuplicated.join(', ')}]`);
    if (StatusT201.isApplying) showErrorToasts('Proceso en ejecución', `Ya existe un proceso de aplicación de cambios ejecutándose, espere a que finalice.`);
}

function T201ShowInitAlerts() {
    if (StatusT201.haveDuplicated) showErrorToasts('Líneas duplicadas', `Varias líneas contienen el mismo código de producto. [${DataTask201.CodsDuplicated.join(', ')}]`);
    if (StatusT201.havePropsEmpty) showToasts('warning', 'Precausión', 'Líneas con propiedades vacías', 'El sistema registrará las propiedades con valores vacíos en caso de aplicar los cambios.');
}

function T201InitStatus() {
    StatusT201.isVerified = false;
    StatusT201.isVerifying = false;
    StatusT201.isApplying = false;
    StatusT201.haveDuplicated = false;
    StatusT201.havePropsEmpty = false;
}

function T201InitDataTaskColumns() {
    DataTask201.columns = [
        { title: '', visible: false, },
        { title: 'Código de producto' },
        { title: 'Descripción Corta', visible: false },
        { title: 'Descripción Extendida' },
        {
            title: 'Propiedades Actuales', render: function (data, type, row, meta) {
                if (type === 'display' && data !== '') {
                    var props = data ? data.split(', ') : [];
                    var rows = '';

                    for (var i = 0; i < props.length; i++)
                        rows += `<span><strong>${props[i].split(':')[0]}:</strong> ${props[i].split(':')[1]}</span>&nbsp;`;

                    return rows;
                } else return data;
            }
        },
    ]
}

function T201GenerateColumnsFile(event) {
    T201InitDataTaskColumns();
    DataTask201.columnsFile = ['Código de producto'];
    var cont = 0;
    for (var i = 0; i < DataTask201.PropsSelected.length; i++) {
        var obj = DataTask201.AllProps.find(x => x.Name === DataTask201.PropsSelected[i]);

        if (obj) {
            DataTask201.columnsFile.push(DataTask201.PropsSelected[i]);
            DataTask201.columns.push({ title: DataTask201.PropsSelected[i] });
            cont++;
        }
    }

    DataTask201.columns.push({ title: 'Estado' });

    if (cont !== DataTask201.PropsSelected.length) {
        DataTask201.columnsFile = ['Código de producto'];
        event.cancel = true;
        error('SELECCION DE PROPIEDADES NO COINCIDE CON LA INFO DEL SISTEMA', 'Error con la información seleccionada.');
    } else taskConfig.columnsFile['T201'] = DataTask201.columnsFile;
}

function T201GetPropertyType(val) {
    switch (val) {
        case 1: return "System.DateTime";
        case 2: return "System.Decimal";
        case 3: return "System.Decimal";
        case 4: return "System.Boolean";
        case 5: return "System.Collections.ArrayList";
        default: return "System.String";
    }
}

function T201GetCsvData() {
    DataTask201.fields = csvData.map(x => {
        var obj = [0, x[0], '', '', ''], count = 0;

        for (var i = 1; i <= DataTask201.PropsSelected.length; i++) {
            if (x[i] === '') StatusT201.havePropsEmpty = true;
            obj.push(x[i]);
        }

        for (var i = 0; i < csvData.length; i++) if (csvData[i][0] === x[0]) count++;

        if (count > 1) {
            if (DataTask201.CodsDuplicated.find(k => k === x[0]) === undefined) DataTask201.CodsDuplicated.push(x[0]);

            obj.push('<span class="badge bg-yellow">LÍNEA DUPLICADA</span>');
            StatusT201.haveDuplicated = true;
        } else obj.push('<span class="badge bg-yellow">SIN VERIFICAR</span>');

        return obj;
    });
}

function T201GetList() {
    return DataTask201.fields.map(x => {
        var obj = { ID: x[0], ItemLookupCode: x[1], Properties: '' };

        for (var i = 0; i < DataTask201.PropsSelected.length; i++) {
            var prop = DataTask201.AllProps.find(p => p.Name === DataTask201.PropsSelected[i]);
            obj.Properties += `[{Property Name="${prop.Name}" Type="${T201GetPropertyType(prop.Type)}"}]${x[5+i]}[{/Property}]`;
        }

        return obj;
    });
}

async function T201OnDoneGetInfo(data) {
    if (data.Status && data.Result === null) showErrorToasts('Verificando productos', data.Message);
    else {
        var flag = false;
        await DataTask201.fields.forEach(i => {
            var item = data.Result.find(x => x.ItemLookupCode === i[1]);

            if (item) {
                i[0] = item.ID;
                i[2] = item.Description;
                i[3] = item.ExtDescription;
                i[4] = item.Properties;
                i[5 + DataTask201.PropsSelected.length] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';
            } else {
                i[5 + DataTask201.PropsSelected.length] = '<span class="badge bg-danger">PRODUCTO NO VERIFICADO</span>';
                flag = true;
            }
        });

        StatusT201.isVerifying = false;

        if (!flag) {
            StatusT201.isVerified = true;
        } else {
            StatusT201.isVerified = false;
            showErrorToasts('Productos sin verificar', 'Uno o varios productos no lograron ser verificados, valide que los códigos sean correctos y que los productos sean accesibles a su rol.')
        }

        initTable(DataTask201.columns, DataTask201.fields);
        $("#spn").hide();
    }
}

function T201GetInfo() {
    try {
        $.post(Task201.urls.verifyItems, { items: DataTask201.fields.map(x => x[1]), propsSelected: DataTask201.PropsSelected.join(',') }).done(function (data) {
            T201OnDoneGetInfo(data);
        }).fail(function (e) {
            StatusT201.isVerifying = false;
            $("#spn").hide();
            error(e, "Error intentando validar productos.");
        });
    } catch (e) {
        StatusT201.isVerifying = false;
        $("#spn").hide();
        error(e, 'Error al intentar validar los productos.')
    }
}

async function T201ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            var func = function () {
                var itemList = T201GetList();
                $("#spn").show();

                $.post(Task201.urls.applyTask, { items: itemList, propsSelected: DataTask201.PropsSelected.toString(), stores: taskConfig.stores.toString(), notes: taskValues }).done(function (data) {
                    if (typeof data === 'string') {
                        error(data, 'Problemas al aplicar los cambios, verifique con el administrador de configurar correctamente su rol de seguridad.');
                    } else if (data.Status) {
                        showSuccesToasts('Cambios aplicados.', `${data.Message}<br/>Recuerde dirigirse a <a href="../HojasTrabajo/Inicio" target="_blank" style="color:blue;">Hojas de Trabajo</a> para aplicar los cambios en la(s) tienda(s) seleccionada(s).`)
                    } else {
                        showErrorToasts('', data.Message);
                        console.error(data.InternalMessage)
                    }
                    StatusT201.isApplying = false;
                    $("#spn").hide();
                }).fail(function (xhr, ajaxOptions, thrownError) {
                    error(thrownError, 'Error intentando aplicar cambios.');
                    console.error(xhr);
                    console.error(ajaxOptions);
                    StatusT201.isApplying = false;
                    $("#spn").hide();
                });
            }

            onActionApplyTask(func);
        } else {
            StatusT201.isApplying = false;
            $("#spn").hide();
        }
    } catch (e) {
        error(e, 'Ocurrió un error al intentar aplicar los cambios.');
        StatusT201.isApplying = false;
        $("#spn").hide();
    }
}

var Task201 = {
    urls: {
        verifyItems: '../Wizard/Task201Verificar',
        applyTask: '../Wizard/Task201'
    },

    startTask: function () {
        try {
            $("#spn").show();
            T201InitStatus();
            T201GetCsvData();
            initTable(DataTask201.columns, DataTask201.fields);
            stepper.next();
            $("#spn").hide();
            T201ShowInitAlerts();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    onClickBtnApplyChanges: function () {
        if (T201VerifyAllowApply()) {
            Swal.fire({
                title: "¿Continuar?", html: "Se aplicarán cambios a los productos seleccionados..", icon: 'warning', showCancelButton: true,
            }).then((result) => {
                if (result.isConfirmed) {
                    $("#spn").show();
                    StatusT201.isApplying = true;
                    T201ApplyNewChanges();
                } else {
                    StatusT201.isApplying = false;
                    $("#spn").hide();
                }
            });
        } else T201ShowVerifyApplyErrors();
    },

    onClickBtnGetInfo: function () {
        if (!StatusT201.haveDuplicated && !StatusT201.isVerifying && !StatusT201.isApplying) {
            $("#spn").show();
            StatusT201.isVerified = false;
            StatusT201.isVerifying = true;
            T201GetInfo();
        } else T201ShowVerifyApplyErrors(true);
    },

    onSubmitTask: async function (event) {
        DataTask201.AllProps = [];
        DataTask201.PropsSelected = [];

        $("#spn").show();

        const propsAPI = '../Wizard/Task201_GetProperties'
        const props = await fetch(propsAPI).then(response => response.json());

        const { value: propsSelected } = await Swal.fire({
            title: 'Seleccionar propiedades a editar',
            html: `<select class="select2 mb-2" id="alert-properties" data-placeholder="Seleccionar propiedades" style="width: 100%;" multiple></select>`,
            showCancelButton: true, focusConfirm: false, allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
            preConfirm: (value, e) => {
                if ($('#alert-properties').val().length > 0) return $('#alert-properties').val();
                else return Swal.showValidationMessage(`Se debe seleccionar una propiedad`);
            },
            onOpen: (e) => {
                var propList = props.map(x => { return { 'id': x.Name, 'text': x.Name } });
                var select = $('#alert-properties').select2({ tags: true, data: propList, allowClear: true, multiple: true });
                $(`#swal2-content`).addClass(`mb-2`)
            }
        });

        if (propsSelected) {
            DataTask201.AllProps = props;
            DataTask201.PropsSelected = propsSelected;
            await T201GenerateColumnsFile(event);
        } else event.cancel = true;

        $("#spn").hide();
    }
}

// Obligatorio para aplicar la tarea
setDataTaskConfiguration('T201', DataTask201.columnsFile, Task201.startTask, Task201.onClickBtnApplyChanges, Task201.onClickBtnGetInfo);
taskConfig.eventOnSubmitTask['T201'] = Task201.onSubmitTask;