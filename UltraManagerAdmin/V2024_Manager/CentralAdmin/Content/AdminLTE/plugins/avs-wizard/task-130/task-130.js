/**
 * Tarea insertado de productos
 * */

var StatusT130 = {
    applyChanges: false,
    applingChanges: false,
    dataErrors: false,
    gettingData: false,
    availableGetData: true,
    isGetted: false,
    dataEmpty: false,
    noFind: false,
    errorPercentage: false,
    errorSupplier: false,
    supplierNotFound: false,
    errorTax: false,
    errorSegmentation: false,
    itemExists: false,
    duplicatedError: false,
    manufacturerError: false,
    brandError: false,
    purchaseError: false,
    uomError: false,
    costError: false,
    priceError: false,
    impEspecificoError: false,
    utilityError: false,

    codeTruncateError: false,
    descriptionTruncateError: false,
    extendedDescriptionTruncateError: false,
    cabysTruncateError: false,

    contentTruncateError: false,
    habladoresTruncateError: false,
    subDescriptionsTruncateError: false,

}, DataTask130 = {
    fields: [],
    columns: [
        { title: 'Cód. Producto' },
        { title: 'Descripción Corta' },
        { title: 'Descripción Larga' },
        { title: 'Cabys' },
        {
            title: 'Costo Neto', render: function (data, type, row, meta) {
                if ((+data) < 0) {
                    StatusT130.costError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${numberToMoney(data)}) COSTO INVÁLIDO</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${numberToMoney(data)}) COSTO INVÁLIDO</span>,`;
                    return `<span class="badge bg-danger">(${numberToMoney(data)}) COSTO INVÁLIDO</span>`;
                } else return type === 'display' ? numberToMoney(data) : data;
            }
        },
        {
            title: 'Impuesto Específico', render: function (data, type, row, meta) {
                if ((+data) < 0) {
                    StatusT130.impEspecificoError = true;
                    return `<span class="badge bg-danger">(${numberToMoney(data)}) IMPUESTO ESPECÍFICO INVÁLIDO</span>`;
                } else return type === 'display' ? numberToMoney(data) : data;
            }
        },
        {
            title: 'Utilidad(%)', render: function (data, type, row, meta) {
                if ((+data) > 800 || (+data) < 0.01) {
                    StatusT130.utilityError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data}) UTILIDAD INVÁLIDA </span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data}) UTILIDAD INVÁLIDA </span>,`
                    return '<span class="badge bg-danger">(' + data + ') UTILIDAD INVÁLIDA </span>';
                } else return type === 'display' ? (data + '%') : data;
            }
        },
        {
            title: 'Descuento en Factura(%)', render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT130.errorPercentage = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data}) DESC. FACTURA INVÁLIDO </span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data}) DESC. FACTURA INVÁLIDO </span>,`
                    return '<span class="badge bg-danger">(' + data + ') DESC. FACTURA INVÁLIDO </span>';
                } else return type === 'display' ? (data + '%') : data;
            }
        },
        {
            title: 'Descuento para Cliente(%)', render: function (data, type, row, meta) {
                if ((+data) >= 100 || (+data) < 0) {
                    StatusT130.errorPercentage = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data}) DESC. CLIENTE INVÁLIDO </span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data}) DESC. CLIENTE INVÁLIDO </span>,`;
                    return '<span class="badge bg-danger">(' + data + ') DESC. CLIENTE INVÁLIDO </span>';
                } else return type === 'display' ? (data + '%') : data;
            }
        },
        { title: 'Impuesto al Artículo(%)' }, { title: 'Impuesto al Artículo', visible: false },
        {
            title: 'Costo Final<i class="fas fa-lock"></i>', render: function (data, type, row, meta) {
                if ((+data) <= 0) {
                    StatusT130.costError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. COSTO INVÁLIDO</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. COSTO INVÁLIDO</span>,`;
                    return `<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. COSTO INVÁLIDO</span>`;
                } else return type === 'display' ? numberToMoney(data) : data;
            }
        },
        {
            title: 'Precio Final (SIN IVA)<i class="fas fa-lock"></i>', render: function (data, type, row, meta) {
                if ((+data) <= 0) {
                    StatusT130.priceError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. PRECIO INVÁLIDO</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. PRECIO INVÁLIDO</span>,`;
                    return `<span class="badge bg-danger">(${numberToMoney(data)}) RESULT. PRECIO INVÁLIDO</span>`;
                } else return type === 'display' ? numberToMoney(data) : data;
            }
        },
        { title: 'Precio Final (CON IVA)<i class="fas fa-lock"></i>' },
        { title: 'Utilidad Real<i class="fas fa-lock"></i>' },
        { title: 'Precio Redondeado<i class="fas fa-lock"></i>' },
        {
            title: 'UOM', render: function (data, type, row, meta) {
                if (data !== '') {
                    var uom = taskList.uoms.find(x => x.Code === data);
                    if (uom) return uom.Name;
                    else {
                        StatusT130.uomError = true;
                        row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                        row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data})NO EXISTE</span>`, '');
                        row[40] += `<span class="badge bg-danger">(${data})NO EXISTE</span>`;
                        return `<span class="badge bg-danger">(${data})NO EXISTE</span>`;
                    }
                } else return data;
            }
        },
        { title: 'Cod. Familia' }, { title: 'Familia' },
        { title: 'Cod. Departamento' }, { title: 'Departamento' },
        { title: 'Cod. Categoría' }, { title: 'Categoría' },
        {
            title: 'Cod. Subcategoría', render: function (data, type, row, meta) {
                return type === 'display' ? (data === '' ? '<span class="badge bg-warning">-NO DEFINIDO-</span>' : (data === null ? '<span class="badge bg-danger">-NO EXISTE-</span>' : data)) : data;
            },
        },
        {
            title: 'SubCategoría', render: function (data, type, row, meta) {
                return type === 'display' ? (data === '' ? '<span class="badge bg-warning">-NO DEFINIDO-</span>' : (data === null ? '<span class="badge bg-danger">-NO EXISTE-</span>' : data)) : data;
            },
        },
        {
            title: 'Cod. Segmento', render: function (data, type, row, meta) {
                return type === 'display' ? (data === '' ? '<span class="badge bg-warning">-NO DEFINIDO-</span>' : (data === null ? '<span class="badge bg-danger">-NO EXISTE-</span>' : data)) : data;
            },
        },
        {
            title: 'Segmento', render: function (data, type, row, meta) {
                return type === 'display' ? (data === '' ? '<span class="badge bg-warning">-NO DEFINIDO-</span>' : (data === null ? '<span class="badge bg-danger">-NO EXISTE-</span>' : data)) : data;
            },
        },
        { title: 'Cód. Proveedor' }, { title: 'Proveedor' },
        {
            title: 'Cod. Fabricante', render: function (data, type, row, meta) {
                var manufacturer = taskList.manufacturers.find(x => x.ID === +data);
                if ((manufacturer === null || manufacturer === undefined) && data) {
                    StatusT130.manufacturerError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data})NO EXISTE</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data})NO EXISTE</span>,`;
                    return `<span class="badge bg-danger">(${data})NO EXISTE</span>`;
                } else return data;
            }
        },
        {
            title: 'Fabricante', render: function (data, type, row, meta) {
                var manufacturer = taskList.manufacturers.find(x => x.ID === +data);
                if ((manufacturer === null || manufacturer === undefined) && data) {
                    StatusT130.manufacturerError = true;
                    return `<span class="badge bg-danger">NO EXISTE</span>`;
                } else if (manufacturer) return manufacturer.Description;
                else return data;
            }
        },
        {
            title: 'Cod. Marca', render: function (data, type, row, meta) {
                var brand = taskList.brands.find(x => x.ID === +data);
                if ((brand === null || brand === undefined) && data) {
                    StatusT130.brandError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data})NO EXISTE</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data})NO EXISTE</span>,`;
                    return `<span class="badge bg-danger">(${data})NO EXISTE</span>`;
                } else return data;
            }
        },
        {
            title: 'Marca', render: function (data, type, row, meta) {
                var brand = taskList.brands.find(x => x.ID === +data);
                if ((brand === null || brand === undefined) && data) {
                    StatusT130.brandError = true;
                    return `<span class="badge bg-danger">NO EXISTE</span>`;
                } else if (brand) return brand.Description;
                else return data;
            }
        },
        {
            title: 'Cod. Casa Comercial', render: function (data, type, row, meta) {
                var purchaser = taskList.purchasers.find(x => x.Code === data);
                if ((purchaser === null || purchaser === undefined) && data) {
                    StatusT130.purchaseError = true;
                    row[40] = row[40].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
                    row[40] = row[40].replaceAll(`<span class="badge bg-danger">(${data})NO EXISTE</span>,`, '');
                    row[40] += `<span class="badge bg-danger">(${data})NO EXISTE</span>,`;
                    return `<span class="badge bg-danger">(${data})NO EXISTE</span>`;
                } else return data;
            }
        },
        {
            title: 'Casa Comercial', render: function (data, type, row, meta) {
                var purchaser = taskList.purchasers.find(x => x.Code === data);
                if ((purchaser === null || purchaser === undefined) && data) {
                    StatusT130.purchaseError = true;
                    return `<span class="badge bg-danger">NO EXISTE</span>`;
                } else if (purchaser) return purchaser.Name;
                else return data;
            }
        },
        { title: 'Contenido' },
        { title: 'Habladores' },
        { title: 'Subdescripción 6' },
        { title: 'Subdescripción 7' },
        { title: 'Subdescripción 8' },
        { title: 'Subdescripción 9' },
        { title: 'Subdescripción 10' },
        { title: 'Estado' },
        {
            title: '', visible: false, render: function (data, type, row, meta) {
                if ((+data) <= 0) StatusT130.errorTax = true;
                return data;
            },
        }
    ],
    columnsFile: [
        'Cod. Producto', //
        'Descripción Corta', //
        'Descripción Larga', //
        'Cabys', //
        'Costo Neto', //
        'Impuesto Específico',
        'Utilidad', //
        'Descuento en Factura(%)',
        'Descuento para Cliente(%)',
        '% Impuesto al Artículo', //
        'Cod. Unidad de Medida',
        'Cod. Familia', //
        'Cod. Departamento', //
        'Cod. Categoría', //
        'Cod. Subcategoría',
        'Cod. Segmento',
        'Cod. Proveedor', //
        'Cod. Fabricante',
        'Cod. Marca',
        'Cod. Casa Comercial',
        'Contenido',
        'Habladores',
        'Subdescripción 6',
        'Subdescripción 7',
        'Subdescripción 8',
        'Subdescripción 9',
        'Subdescripción 10',
    ],
};

/** INICIALIZA LOS ESTADOS */
function T130InitStatus() {
    StatusT130.applyChanges = false;
    StatusT130.availableGetData = true;
    StatusT130.gettingData = false;
    StatusT130.dataErrors = false;
    StatusT130.isGetted = false;
    StatusT130.dataEmpty = false;
    StatusT130.noFind = false;
    StatusT130.errorPercentage = false;
    StatusT130.errorSupplier = false;
    StatusT130.errorTax = false;
    StatusT130.itemExists = false;
    StatusT130.errorSegmentation = false;
    StatusT130.manufacturerError = false;
    StatusT130.brandError = false;
    StatusT130.purchaseError = false;
    StatusT130.uomError = false;
    StatusT130.costError = false;
    StatusT130.priceError = false;
    StatusT130.impEspecificoError = false;
    StatusT130.utilityError = false;

    StatusT130.codeTruncateError = false;
    StatusT130.descriptionTruncateError = false;
    StatusT130.extendedDescriptionTruncateError = false;
    StatusT130.cabysTruncateError = false;
    StatusT130.contentTruncateError = false;
    StatusT130.habladoresTruncateError = false;
    StatusT130.subDescriptionsTruncateError = false;
    StatusT130.supplierNotFound = false;
    StatusT130.duplicatedError = false;
}

/**
 * VALIDA LA CATEGORIZACIÓN DE CADA UNO DE LOS PRODUCTOS
 * @param {any} fam FAMILIA
 * @param {any} dep DEPARTAMENTO
 * @param {any} cat CATEGORÍA
 * @param {any} subCat SUBCATEGORÍA
 * @param {any} subCatCode CÓDIGO DE SUBCATEGORÍA
 * @param {any} seg SEGMENTO
 * @param {any} segCode CÓDIGO DEL SEGMENTO
 */
function T130ValidateCategorization(fam, dep, cat, subCat, subCatCode, seg, segCode) {
    var categError = '';
    // VERIFICAR CATEGORIZACIÓN DEL SEGMENTO //
    if (seg) {
        if (subCat) {
            if (seg.SubCategoryID !== subCat.ID) categError += `EL SEGMENTO (${seg.Description}) NO PERTENECE A LA SUBCATEGORÍA (${subCat.Description}), `;
        } else categError += 'SEGMENTO NO VERIFICADO POR SUBCATEGORÍA NO DEFINIDA, ';
    } else if (segCode) categError += 'SEGMENTO NO EXISTE. ';
    // VERIFICAR CATEGORIZACIÓN DE LA SUBCATEGORÍA //
    if (subCat) {
        if (cat) {
            if (subCat.CategoryID !== cat.ID) categError += `LA SUBCATEGORÍA (${subCat.Description}) NO PERTENECE A LA CATEGORÍA (${cat.Name}), `;
        } else categError += 'SUBCATEGORÍA NO VERIFICADA POR CATEGORÍA NO DEFINIDA, ';
    } else if (subCatCode) categError += 'SUBCATEGORÍA NO EXISTE. ';
    // VERIFICAR CATEGORIZACIÓN DE LA CATEGORÍA //
    if (cat) {
        if (dep) {
            if (cat.DepartmentID !== dep.ID) categError += `LA CATEGORÍA (${cat.Name}) NO PERTENECE AL DEPARTAMENTO (${dep.Name}), `;
        } else categError += 'CATEGORÍA NO VERIFICADA POR DEPARTAMENTO NO DEFINIDO, ';
    } else categError += 'CATEGORÍA NO EXISTE O NO DEFINIDA. ';
    // VERIFICAR CATEGORIZACIÓN DEL DEPARTAMENTO //
    if (dep) {
        if (fam) {
            if (dep.FamilyID !== fam.ID) categError += `EL DEPARTAMENTO (${dep.Name}) NO PERTENECE A LA FAMILIA (${fam.Name}), `;
        } else categError += 'DEPARTAMENTO NO VERIFICADO POR FAMILIA NO DEFINIDA, ';
    } else categError += 'DEPARTAMENTO NO EXISTE O NO DEFINIDO, ';
    // VERIFICAR CATEGORIZACIÓN DE LA FAMILIA //
    if (fam === null || fam === undefined) categError += 'FAMILIA NO EXISTE O NO DEFINIDA, ';

    return { status: categError === '', message: categError };
}

/** VALIDACIÓN PARA VER CÓDIGOS DUPLICADOS O CAMPOS REUQERIDOS VACÍOS */
async function T130ValidateDataFields() {
    await DataTask130.fields.forEach(async function (x) {
        var count = 0;
        // VERIFICAR CÓDIGOS DUPLICADOS //
        if (x[0] !== '') await DataTask130.fields.forEach(y => { if (x[0] === y[0]) count++; });
        // VERIFICAR ESPACIOS REQUERIDOS VACIOS //
        x[0] = T130VerifyValidateData(T130ValidateLength(x[0], true, 25, 'Cod. Proveedor'), 'codeTruncateError', 'dataEmpty', x).result;
        x[1] = T130VerifyValidateData(T130ValidateLength(x[1], true, 80, 'Descripción Corta'), 'descriptionTruncateError', 'dataEmpty', x).result;
        x[2] = T130VerifyValidateData(T130ValidateLength(x[2], true, 2500, 'Descripción Larga'), 'extendedDescriptionTruncateError', 'dataEmpty', x).result;
        x[3] = T130VerifyValidateData(T130ValidateLength(x[3], true, 30, 'Cabys'), 'cabysTruncateError', 'dataEmpty', x).result;

        x[35] = T130VerifyValidateData(T130ValidateLength(x[35], true, 30, 'Contenido'), 'contentTruncateError', 'dataEmpty', x).result;
        x[36] = T130VerifyValidateData(T130ValidateLength(x[36], true, 38, 'Habladores'), 'habladoresTruncateError', 'dataEmpty', x).result;
        x[37] = T130VerifyValidateData(T130ValidateLength(x[37], false, 30, 'Subdescripción 6'), 'subDescriptionsTruncateError', 'dataEmpty', x).result;
        x[38] = T130VerifyValidateData(T130ValidateLength(x[38], false, 30, 'Subdescripción 7'), 'subDescriptionsTruncateError', 'dataEmpty', x).result;
        x[39] = T130VerifyValidateData(T130ValidateLength(x[39], false, 30, 'Subdescripción 8'), 'subDescriptionsTruncateError', 'dataEmpty', x).result;
        x[40] = T130VerifyValidateData(T130ValidateLength(x[40], false, 30, 'Subdescripción 9'), 'subDescriptionsTruncateError', 'dataEmpty', x).result;
        x[41] = T130VerifyValidateData(T130ValidateLength(x[41], false, 30, 'Subdescripción 10'), 'subDescriptionsTruncateError', 'dataEmpty', x).result;
        // VERIFICAR RESULTADO DE CÓDIGOS DUPLICADOS //
        if (count > 1) StatusT130.duplicatedError = true;
        // SE SALE EN CASO DE ALGUNA ANOMALÍA //
        if (StatusT130.duplicatedError || StatusT130.dataEmpty) return;
    })
}

function T130ValidateLength(data, required, max, fieldName) {
    if (data) {
        if (data.length > max) return { status: false, type: 'maxError', data: data, max: max, fieldName: fieldName };
        else return { status: true, type: '', data: data, max: max, fieldName: fieldName };
    } else return required ?
        { status: false, type: 'dataError', data: data, max: max, fieldName: fieldName } :
        { status: true, type: '', data: data, max: max, fieldName: fieldName };
}

function T130VerifyValidateData(dataResult, statusTruncate, statusDataEmpty, row) {
    if (!dataResult.status && dataResult.type === 'maxError') {
        StatusT130[statusTruncate] = true;
        console.log(row[42]);
        row[42] = row[42].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>', '');
        row[42] += `<span class="badge bg-danger">${dataResult.fieldName} sobrepasa los ${dataResult.max} caracteres. </span>,`;
        return {
            dataResult: dataResult,
            result: `<span class="badge bg-danger">(${dataResult.data}) sobrepasa los ${dataResult.max} caracteres. </span>,`
        };
    } else if (!dataResult.status && dataResult.type === 'dataError') {
        StatusT130[statusDataEmpty] = true;
        row[42] = row[42].replaceAll('<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>,', '');
        row[42] += `<span class="badge bg-danger">${dataResult.fieldName} es requerido. </span>,`;
        return {
            dataResult: dataResult,
            result: `<span class="badge bg-danger">${dataResult.fieldName} es requerido. </span>,`
        }
    } else return { dataResult: dataResult, message: ``, result: dataResult.data }
}

/** HACE MAPEO DEL CSV A ARRAY PARA MOSTRAR EN EL TABLE DEL TASK */
function T130CsvToTaskTable() {
    DataTask130.fields = csvData.map(x => {
        // OBTENER CATEGORIZACIÓN //
        var family = taskList.families.find(f => f.Code === x[11]);
        var department = taskList.departments.find(d => d.Code === x[12]);
        var category = taskList.categories.find(c => c.Code === x[13]);
        var subcategory = taskList.subcategories.find(sc => sc.Code === x[14]);
        var segment = taskList.segments.find(s => s.Code === x[15]);
        // OBTENER INFO ADICIONAL //
        var supplier = taskList.suppliers.find(p => p.Code === x[16]);
        var tax = taskList.taxs.find(t => t.Percentage === (+x[9]));
        // PARA MENSAJES DE NO ENCONTRADOS //
        var noFound1 = '<span class="badge bg-danger">(', noFound2 = ')NO EXISTE</span>';
        var notDefined = '<span class="badge bg-warning">-NO DEFINIDO-</span>';
        var suppNotFound = `<span class="badge bg-danger">${x[16]} INVÁLIDO</span>`;
        var taxNotFound = `<span class="badge bg-danger">PORCENTAJE IMPUETO (${+x[9]}%) INVÁLIDO</span>`;
        // VALIDAR CATEGORIZACIÓN //
        var validate = T130ValidateCategorization(family, department, category, subcategory, x[14], segment, x[15]);
        // VERIFICAR VALIDACIONES //
        if (!validate.status) StatusT130.errorSegmentation = true;
        if (supplier === null || supplier === undefined) StatusT130.supplierNotFound = true;
        if (tax === null || tax === undefined) StatusT130.errorTax = true;
        // OBTENER COSTOS //
        var cost = +x[4], msrp = +x[5], utility = +x[6], inv_discount = +x[7], cus_discount = +x[8];
        // PARA COSTOS Y PRECIOS FINALES //
        var final_cost = (cost - (cost * (inv_discount / 100)) + msrp);
        var final_price_sin_utilidad = (cost - (cost * (cus_discount / 100)) + msrp);
        var final_price = ((cost - (cost * (cus_discount / 100)) + msrp) * (1 + (utility / 100)));
        var final_price_iva = tax ? numberToMoney(final_price * (1 + (tax.Percentage / 100))) : '<span class="badge bg-danger">IMPUESTO AL ARTÍCULO INVÁLIDO</span>';

        var final_price_rounded = Math.round((+moneyToNumber(final_price_iva)) / 5) * 5;
        var real_utility = tax ? (((final_price_rounded - (final_price * (tax.Percentage / 100))) / final_price_sin_utilidad) - 1) * 100 : 0;

        var estado = '';
        if (!validate.status) estado += `<span class="badge bg-danger">${validate.message}</span>, `;
        if (supplier === null || supplier === undefined) estado += `<span class="badge bg-danger">PROVEEDOR (${x[16]}) INVÁLIDO</span>, `;
        if (tax === null || tax === undefined) estado += `<span class="badge bg-danger">IMPUESTO (${x[9]}) INVÁLIDO</span>, `;

        // PASAR DEL CSV AL TABLE DE VERIFICACIÓN DE DATOS //
        return [
            x[0], x[1], x[2], x[3], cost, msrp, utility, inv_discount, cus_discount,
            tax ? `${x[9]}%` : taxNotFound, tax ? tax.Name : taxNotFound, final_cost, final_price, final_price_iva, `${real_utility}%`, numberToMoney(final_price_rounded), x[10],
            family ? family.Code : noFound1 + x[11] + noFound2, family ? family.Name : noFound1 + x[11] + noFound2,
            department ? department.Code : noFound1 + x[12] + noFound2, department ? department.Name : noFound1 + x[12] + noFound2,
            category ? category.Code : noFound1 + x[13] + noFound2, category ? category.Name : noFound1 + x[13] + noFound2,
            subcategory ? subcategory.Code : (x[14] ? null : ''), subcategory ? subcategory.Description : (x[14] ? null : ''),
            segment ? segment.Code : (x[15] ? null : ''), segment ? segment.Description : (x[15] ? null : ''),
            supplier ? supplier.Code : suppNotFound, supplier ? supplier.Name : suppNotFound,
            x[17], x[17], x[18], x[18], x[19], x[19], x[20], x[21], x[22], x[23], x[24], x[25], x[26],
            estado === '' ? `<span class="badge bg-primary">CARGADO (FALTA VERIFICAR)</span>` : estado,
            tax ? tax.ID : 0
        ];
    });
}

/** CONSULTA TODOS LOS POSIBLES ERRORES EN LOS DATOS SUMISTRADOS Y SE LOS MUESTRA AL USUARIO */
async function T130ValidateDataTask() {
    if (StatusT130.errorPercentage) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Porcentajes.', body: 'Uno o varios productos poseen uno o varios descuentos erroneos. Los descuentos deben ser inferiores al 100% y mayores o igual a 0%.' });
    if (StatusT130.errorSegmentation) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Categorización.', body: 'Uno o varios productos poseen una categorización erronea. Verifique la información.' });
    if (StatusT130.supplierNotFound) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Proveedores.', body: 'Uno o varios productos poseen un proveedor inválido. Verifique que los proveedores asignados existen en el sistema.' });
    if (StatusT130.errorTax) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Impuestos.', body: 'Uno o varios productos poseen un valor de impuesto erroneo. Verifique que el impuesto suministrado exista en el sistema.' });
    if (StatusT130.manufacturerError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Fabricantes.', body: 'Uno o varios productos poseen un Fabricante erroneo o no existe.' });
    if (StatusT130.brandError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Marca.', body: 'Uno o varios productos poseen una Marca erronea o no existe.' });
    if (StatusT130.purchaseError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Casa Comercial.', body: 'Uno o varios productos poseen una Casa Comercial erroneo o no existe.' });
    if (StatusT130.costError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Costos.', body: 'Uno o varios productos poseen una costo inválido. Verifique que sea mayor a 0' });
    if (StatusT130.priceError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Precios Finales.', body: 'Uno o varios productos poseen resultado de precio final inválido.' });
    if (StatusT130.uomError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Unidad de Medida.', body: 'A uno o a varios productos se les asignó una Unidad de Medida inválido.' });
    if (StatusT130.duplicatedError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Códigos de Producto.', body: 'Uno o varios productos poseen un mismo código.' });
    if (StatusT130.dataEmpty) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en datos de Producto.', body: 'A uno o a varios productos les falta información. Verifique ([Cod. Producto], [Descripción Corta], [Descripción Larga], [Cabys]).' });
    if (StatusT130.impEspecificoError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Impuesto Específico.', body: 'Uno o varios productos tienen un Impuesto Específico erroneo. Verifique que sea mayor a 0.' });
    if (StatusT130.utilityError) $(document).Toasts('create', { class: 'bg-danger', title: 'Error en Utilidad.', body: 'Uno o varios productos tienen una utilida inválida. Verifique que esté entre 0~100.' });
    if (StatusT130.codeTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Código excede el límite de caracteres.', body: 'Uno o varios productos tienen un código que excede los 25 caracteres.' });
    if (StatusT130.descriptionTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Descripción Corta excede el límite de caracteres.', body: 'Uno o varios productos tienen una Descripción Corta que excede los 80 caracteres.' });
    if (StatusT130.extendedDescriptionTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Descripción Larga excede el límite de caracteres.', body: 'Uno o varios productos tienen una Descripción Corta que excede los 80 caracteres.' });
    if (StatusT130.cabysTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Cabys excede el límite de caracteres.', body: 'Uno o varios productos tienen un Cabys que excede los 30 caracteres.' });
    if (StatusT130.contentTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Contenido excede el límite de caracteres.', body: 'Uno o varios productos tienen un Contenido que excede los 30 caracteres.' });
    if (StatusT130.habladoresTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Campo Habladores excede el límite de caracteres.', body: 'Uno o varios productos tienen el campo Habladores que excede los 38 caracteres.' });
    if (StatusT130.subDescriptionsTruncateError) $(document).Toasts('create', { class: 'bg-warning', title: 'Subdescripción excede el límite de caracteres.', body: 'Uno o varios productos tienen una o varias Subdescripciones que exceden los 30 caracteres.' });
}

/** HACE MAPEO DE UN LISTADO DE ESTRUCTURAS DE PRODUCTOS PARA EJECUTAR LA TAREA DEL LISTADO DE PRODUCTOS EN EL TABLE DEL TASK */
function T130GetItemsToStruct() {
    return DataTask130.fields.map(x => {
        return {
            ItemLookupCode: x[0],
            Description: x[1],
            DescriptionExtended: x[2],
            Subdescription3: x[3],
            Cost: x[4],
            MSRP: x[5],
            Utility: x[6],
            InvoiceDiscount: x[7],
            CustomerDiscount: x[8],
            UOM: x[16],
            FamilyCode: x[17],
            DepartmentCode: x[19],
            CategoryCode: x[21],
            SubCategoryCode: x[23],
            SegmentCode: x[25],
            SupplierCode: x[27],
            ManufacturerCode: x[29],
            BrandCode: x[31],
            PurchaserCode: x[33],
            Subdescription4: x[35],
            Subdescription5: x[36],
            Subdescription6: x[37],
            Subdescription7: x[38],
            Subdescription8: x[39],
            Subdescription9: x[40],
            Subdescription10: x[41],
            TaxID: x[43]
        }
    })
}

/**
 * EVENTO EJECUTADO AL FINALIZAR LA VALIDACIÓN DE CÓDIGOS DE PRODUCTOS EN EL SISTEMA. REVISA Y SI HAY ERRORES SE LOS MUESTRA AL USUARIO
 * @param {any} data RESULTADO DEL $.post
 */
async function T130OnDoneGetInfo(data) {
    if (data.Status) {
        for (var i = 0; i < DataTask130.fields.length; i++) DataTask130.fields[i][42] = '<span class="badge bg-success">PRODUCTO VERIFICADO</span>';

        if (data.Result.length === 0) $(document).Toasts('create', { class: 'bg-success', title: 'Verificación de Productos.', body: 'Todos los productos están verificados. Se puede continuar' });
        else {
            for (var x = 0; x < data.Result.length; x++) {
                if (data.Result[x].Result) {
                    StatusT130.itemExists = true;
                    toastr.error(`El producto ${data.Result[x].Message} ya se encuentra en el sistema.`);
                    for (var i = 0; i < DataTask130.fields.length; i++) {
                        if (DataTask130.fields[i][0] === data.Result[x].Message) {
                            DataTask130.fields[i][42] = '<span class="badge bg-danger">CODIGO YA EXISTE</span>';
                            break;
                        }
                    }
                } else {
                    StatusT130.dataErrors = true;
                    toastr.error(data.Result[x].Message);
                    for (var i = 0; i < DataTask130.fields.length; i++) {
                        if (DataTask130.fields[i][0] === data.Result[x].Message) {
                            DataTask130.fields[i][42] = '<span class="badge bg-danger">ERROR CONSULTAR</span>';
                            break;
                        }
                    }
                }
            }
        }
        T130EndGetInfo();
    } else error(data.InternalMessage, data.Message);
}

/** HACE POST CON EL LISTADO DE CÓDIGOS DE PRODUCTOS PARA REVISAR QUE TODO ESTÁ BIEN CON EL SISTEMA */
function T130GetInfo() {
    try {
        StatusT130.gettingData = true;
        $("#txt_spn").html("Validando códigos de productos...");
        $.post(Task130.urls.validateCodes, { itemLookupCodes: DataTask130.fields.map(x => { return x[0] }) }).done(function (data) {
            T130OnDoneGetInfo(data);
        }).fail(function () {
            StatusT130.dataErrors = true;
            error("ERROR CONSULTA", "Error intentando validar productos.");
            T130EndGetInfo();
        });
    } catch (e) {
        StatusT130.dataErrors = true;
        error("ERROR CONSULTA", "Error intentando validar productos.");
        T130EndGetInfo();
    }
}

/** EVENTO EJECUTADO CUANDO SE FINALIZA LA VERIFICACIÓN DE LA LECTURA DEL RESULTADO DEL MÉTODO T130OnDoneGetInfo Y ACTUALIZA UNOS ESTADOS */
function T130EndGetInfo() {
    StatusT130.gettingData = false;
    StatusT130.isGetted = true;
    StatusT130.applyChanges = !StatusT130.dataErrors && !StatusT130.noFind && StatusT130.availableGetData;
    $("#spn").hide();
    $("#txt_spn").html("Cargando...");
    initTable(DataTask130.columns, DataTask130.fields);
}

/**
 * EVENTO EJECUTADO AL FINALIZAR LA APLICACIÓN DE CAMBIOS
 * @param {any} data RESULTADO DEL $.post
 */
function T130OnDoneApplyChanges(data) {
    if (data.Status) {
        T130InitStatus();
        $(document).Toasts('create', { class: 'bg-success', title: 'Importación de productos', subtitle: '', body: "Productos ingresados al sistema de forma satisfactoria" });
        T130CreateWorksheets();
    } else {
        $(document).Toasts('create', { class: 'bg-danger', title: 'Importación de productos', subtitle: '', body: data.Message });
        console.log(data.InternalMessage);
        $("#spn").hide();
    }
    StatusT130.applingChanges = false;
}

/** ACCIONA EL APLICAR CAMBIOS EN EL SISTEMA */
async function T130ApplyNewChanges() {
    try {
        const { value: taskValues } = await getNotesTask();

        if (taskValues) {
            DataTask130.notes = taskValues;

            var func = function () {
                var list = T130GetItemsToStruct();

                $("#spn").show();
                $("txt_spn").val("Importando productos...");
                $(function () {
                    $.ajax({
                        url: Task130.urls.applyTask, type: 'post',
                        data: {
                            storesID: taskConfig.stores,
                            notes: taskValues,
                            items: JSON.stringify(list)
                        }, cache: false,
                        success: function (data) {
                            T130OnDoneApplyChanges(data);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            console.log(xhr.responseText);
                            console.log(ajaxOptions);
                            console.log(thrownError);
                            error(thrownError, 'Error intentando aplicar cambios.');
                            StatusT130.applingChanges = false;
                            $("#spn").hide();
                            $("txt_spn").val("Cargando...");
                        }
                    });
                });
            }

            onActionApplyTask(func);
        } else StatusT130.applingChanges = false;
    } catch (e) {
        error(e, 'Error intentando aplicar cambios.');
        StatusT130.applingChanges = false;
        $("#spn").hide();
        $("txt_spn").val("Cargando...");
    }
}

async function T130CreateWorksheets() {
    try {
        var items = DataTask130.fields.map(x => { return x[0]; });
        $("spn").show();
        $("txt_spn").html("Creando hojas de trabajo...");

        $(async function () {
            $.post(Task130.urls.worksheets, { storesID: taskConfig.stores, items: items, notes: DataTask130.notes }).done(function (data) {
                $("#spn").hide();
                $("txt_spn").html("Cargando...");
                if (data.Status) {
                    $(document).Toasts('create', { class: 'bg-success', title: 'Creación de hoja de trabajo', subtitle: '', body: 'Hojas de trabajo creadas para la sincronización de productos a los grupos de tiendas' });
                } else {
                    $(document).Toasts('create', { class: 'bg-danger', title: 'Creación de hoja de trabajo', subtitle: '', body: data.Message });
                    Swal.fire({
                        title: "Preciona OK para continuar",
                        html: "Por algún error interno no se pudieron crear las hojas de trabajo para la sincronización de productos a los grupos de tiendas. A continuación se reintentará crear las hojas de trabajo.",
                        icon: 'error', showCancelButton: true, allowOutsideClick: false, allowEscapeKey: false,
                    }).then((result) => {
                        if (result.isConfirmed) T130CreateWorksheets();
                    });
                }
            }).fail(function (e) {
                $("#spn").hide();
                $("txt_spn").html("Cargando...");
                Swal.fire({
                    title: "Preciona OK para continuar",
                    html: "Por algún error interno no se pudieron crear las hojas de trabajo para la sincronización de productos a los grupos de tiendas. A continuación se reintentará crear las hojas de trabajo.",
                    icon: 'error', showCancelButton: true, allowOutsideClick: false, allowEscapeKey: false,
                }).then((result) => {
                    if (result.isConfirmed) T130CreateWorksheets();
                });
            });
        });
    } catch (e) {
        $("#spn").hide();
        $("txt_spn").html("Cargando...");
        Swal.fire({
            title: "Preciona OK para continuar",
            html: "Error intentando crear las hojas de trabajo. A continuación se reintentará crear las hojas de trabajo.",
            icon: 'error', showCancelButton: true, allowOutsideClick: false, allowEscapeKey: false,
        }).then((result) => {
            if (result.isConfirmed) T130CreateWorksheets();
        });
    }
}

/** ALMACENA DATOS Y FUNCIONES DEL TASK. SIRVE PARA QUE LA VISTA SE CONECTE CON ESTA TAREA EN ESPECÍFICO. (TODO ES UN ESTANDAR) */
var Task130 = {
    /** URLS PROPIAS DE CADA TASK */
    urls: {
        applyTask: '../Wizard/Task130',
        initData: '../Wizard/GetInitDataTask130',
        validateCodes: '../Wizard/Task130ValidateItemCodes',
        worksheets: '../Wizard/WizardDownloadItems'
    },

    /** FUNCIÓN INITIALIZE DEL TASK */
    startTask: async function () {
        try {
            await T130InitStatus();
            await T130CsvToTaskTable();
            await T130ValidateDataFields();
            await initTable(DataTask130.columns, DataTask130.fields);
            await T130ValidateDataTask();
            stepper.next();
            $("#spn").hide();
        } catch (e) {
            error(e, "Error al inicializar la tarea. Contacte a soporte.");
            $("#spn").hide();
        }
    },

    /** EVENTO DE CUANDO SE LE DA CLICK AL BOTÓN DE APLICAR CAMBIOS (LA VISTA SE ENCARGA DE EJECUTARLO) */
    onClickBtnApplyChanges: function () {
        if (!StatusT130.errorPercentage && !StatusT130.supplierNotFound && !StatusT130.errorSegmentation && !StatusT130.errorTax
            && !StatusT130.manufacturerError && !StatusT130.brandError && !StatusT130.purchaseError && !StatusT130.uomError
            && !StatusT130.costError && !StatusT130.priceError && !StatusT130.duplicatedError && !StatusT130.dataEmpty
            && !StatusT130.impEspecificoError && !StatusT130.utilityError
            && !StatusT130.codeTruncateError && !StatusT130.descriptionTruncateError && !StatusT130.extendedDescriptionTruncateError && !StatusT130.cabysTruncateError
            && !StatusT130.contentTruncateError && !StatusT130.habladoresTruncateError && !StatusT130.subDescriptionsTruncateError
        ) {
            if (StatusT130.applyChanges && !StatusT130.applingChanges && !StatusT130.dataEmpty && !StatusT130.dataErrors && !StatusT130.itemExists) {
                StatusT130.applingChanges = true;
                Swal.fire({
                    title: "¿Continuar?", html: "Se crearán los productos ingresados...", icon: 'warning', showCancelButton: true,
                }).then((result) => {
                    if (result.isConfirmed) T130ApplyNewChanges();
                    else StatusT130.applingChanges = false;
                });
            } else {
                if (StatusT130.dataEmpty) toastr.error("No se puede continuar. Hay productos con estados inválidos.");
                if (StatusT130.applingChanges) toastr.error("No se puede continuar. Ya existe un proceso corriendo. Espere a que termine.");
                if (StatusT130.dataErrors) toastr.error("No se puede continuar. Hay datos con errores.");
                if (StatusT130.noFind) toastr.error("No se puede continuar. Hubieron productos sin encontrar.");
                if (!StatusT130.isGetted) toastr.error("No se puede continuar. Primero debe de verificar los datos.");
                if (StatusT130.gettingData) toastr.error("No se puede continuar. Primero el proceso de obtención de datos debe de finalizar.");
                if (!StatusT130.availableGetData) toastr.error("No se puede continuar. La obtención de datos fue cancelada. Vuelva a intentar obtener la información.");
                if (StatusT130.itemExists) toastr.error("No se puede continuar. Uno o varios códigos de productos ya existen en el sistema.");
            }
        } else T130ValidateDataTask();
    },

    /** EVENTO QUE SE EJECUTA CUANDO SE LE DA CLICK AL BOTÓN DE VERIFICAR LA INFO (LA VISTA SE ENCARGA DE EJECUATARLO POR SÍ SOLA) */
    onClickBtnGetInfo: function () {
        if (!StatusT130.errorPercentage && !StatusT130.supplierNotFound && !StatusT130.errorSegmentation && !StatusT130.errorTax
            && !StatusT130.manufacturerError && !StatusT130.brandError && !StatusT130.purchaseError && !StatusT130.uomError
            && !StatusT130.costError && !StatusT130.priceError && !StatusT130.duplicatedError && !StatusT130.dataEmpty
            && !StatusT130.impEspecificoError && !StatusT130.utilityError
            && !StatusT130.codeTruncateError && !StatusT130.descriptionTruncateError && !StatusT130.extendedDescriptionTruncateError && !StatusT130.cabysTruncateError
            && !StatusT130.contentTruncateError && !StatusT130.habladoresTruncateError && !StatusT130.subDescriptionsTruncateError
        ) {
            $("#spn").show()
            StatusT130.availableGetData = true;
            StatusT130.noFind = false;
            StatusT130.dataEmpty = false;
            StatusT130.itemExists = false;
            StatusT130.dataErrors = false;
            T130GetInfo();
        } else T130ValidateDataTask();
    },
}

/** FUNCIÓN DE OBTENER LA INFO REQUERIDA PARA CONTINUAR CON LA IMPORTACIÓN DE LOS PRODUCTOS */
function T130GetInitData() {
    try {
        $("#spn").show();
        $.get(Task130.urls.initData).done(function (data) {
            if (data.Status) {
                taskList.suppliers = data.Result.suppliers;
                taskList.families = data.Result.families;
                taskList.departments = data.Result.departments;
                taskList.categories = data.Result.categories;
                taskList.subcategories = data.Result.subCategories;
                taskList.segments = data.Result.segments;
                taskList.taxs = data.Result.taxs;
                taskList.uoms = data.Result.uoms;
                taskList.manufacturers = data.Result.manufacturers;
                taskList.brands = data.Result.brands;
                taskList.purchasers = data.Result.purchasers;
                Task130.startTask();
            } else {
                error(data.InternalMessage, data.Message);
                $("#spn").hide();
            }
        }).fail(function (e) {
            $("#spn").hide();
            error(e, "No se puede continuar. Error al intentar obtener información requerida para la ejecución de la tarea.")
        })
    } catch (e) {
        $("#spn").hide();
        error(e, 'No se puede continuar. Error al intentar obtener información requerida para la ejecución de la tarea.')
    }
}

/** AQUÍ SE CONFIGURA AL WIZARD LOS EVENTOS Y REQUERIMIENTOS PARA LA EJECUCIÓN DE ESTA TAREA */
/** SI ESTO NO SE DEFINE, LA TAREA NO SE EJECUTARÁ */
/** CABE DESTACAR QUE, EL LA VISTA DEL WIZAR SE DEBE INCORPORAR ESTE .js */
setDataTaskConfiguration(
    'T130', // NOMBRE DEL TASK (ESTANDAR T<COD. DE TAREA>) //
    DataTask130.columnsFile, // COLUMNAS QUE REQUERIRÁ EN EL CSV A LEER //
    T130GetInitData, // EVENTO INICIAL DEL TASK (SE EJECUTA CUANDO SE DA EN SIGUIENTE, DESPUÉS DE CARGAR EL CSV) //
    Task130.onClickBtnApplyChanges, // EVENTO PARA CUANDO SE DE CLICK EN BOTÓN APLICAR CAMBIOS //
    Task130.onClickBtnGetInfo // EVENTO PARA CUANDO SE DE CLICK EN BOTÓN VERIFICAR INFO //
);