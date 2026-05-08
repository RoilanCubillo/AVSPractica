/* Tablas */
function StartLoadTable(div_table, bit_tools) {
    $("#" + div_table).hide();
    $("#" + div_table).parent().prepend("<div class='spinner-input' id='spn_" + div_table + "'></div>");

    if ($("#" + div_table + " > table").attr("id") == undefined)
        $("#" + div_table + " > table").attr("id", "tbl_" + div_table);

    if (bit_tools) {
        $("#tbl_" + div_table).DataTable().destroy();
        $("#" + div_table + " > table").attr("class", "").attr("class", "tbl_" + div_table);
    }
    else {
        $("#" + div_table + " > table").attr("class", "").attr("class", "table table-striped table-bordered");
    }
}

function EndLoadTable(Titulo,div_table, bit_tools) {
    $("#spn_" + div_table).remove();
    $("#" + div_table).show();

    if (bit_tools)
        DataTableInit(Titulo,"tbl_" + div_table, true, true);
}

/* Select */
function StartLoadSelect(select_id) {
    try {
        var select = $("#" + select_id);

        select.find('option').remove().end().append('<option value="0">-- Seleccione una opción --</option>');
        select.next(".select2-container").hide();
        select.parent().append("<div class='spinner-input-sm' id='spn_" + select_id + "'></div>");
    } catch (e) {
        console.log("Error --> fn: StartLoadSelect, ctrl: " + select_id);
    }
}

function StartLoadSelect2(select_id) {
    try {
        var select = $("#" + select_id);

        //select.find('option').remove().end().append('<option value="0">-- Seleccione una opción --</option>');
        select.next(".select2-container").hide();
        select.parent().append("<div class='spinner-input-sm' id='spn_" + select_id + "'></div>");
    } catch (e) {
        console.log("Error --> fn: StartLoadSelect, ctrl: " + select_id);
    }
}

/* Formulario */
function StartLoadForm(div_id) {
    try {
        var div_form = $("#" + div_id);

        div_form.hide();
        div_form.parent().append("<div class='spinner-input' id='spn_" + div_id + "'></div>");
    } catch (e) {
        console.log("Error --> fn: StartLoadForm, ctrl: " + div_id);
    }
}

function EndLoadForm(div_id) {
    try {
        $("#spn_" + div_id).remove();
        $("#" + div_id).show();
    } catch (e) {
        console.log("Error --> fn: StartLoadForm, ctrl: " + div_id);
    }
}

function DisableForm(div_id) {
    try {
        $("#" + div_id + " button").attr("disabled", "disabled");
        $("#" + div_id + " input").attr("disabled", "disabled");
        $("#" + div_id + " textarea").attr("disabled", "disabled");
        $("#" + div_id + " select").attr("disabled", "disabled");
        $("#" + div_id + " select").next(".select2-container").attr("disabled", "disabled");
    } catch (e) {
        console.log("Error --> fn: DisableForm, ctrl: " + div_id);
    }
}

function EnableForm(div_id) {
    try {
        $("#" + div_id + " button").attr("disabled", false);
        $("#" + div_id + " input").attr("disabled", false);
        $("#" + div_id + " textarea").attr("disabled", false);
        $("#" + div_id + " select").attr("disabled", false);
        $("#" + div_id + " select").next(".select2-container").attr("enabled", "enabled");
    } catch (e) {
        console.log("Error --> fn: EnableForm, ctrl: " + div_id);
    }
}

function CleanForm(div_id) {
    try {
        $("#" + div_id + " input").val("");
        $("#" + div_id + " textarea").val("");
        $("#" + div_id + " select").val("0").trigger('change.select2');
    } catch (e) {
        console.log("Error --> fn: EnableForm, ctrl: " + div_id);
    }
}

function GetFormatDateHourJSON(dateJSON) {
    try {
        var date = new Date(parseInt(dateJSON.substr(6)));

        return date.getHours() + ":" + date.getMinutes();
    } catch (e) {
        return "";
    }
}

    /* Formato a los numeros */
function number_format(amount, decimals) {

    amount += ''; // por si pasan un numero en vez de un string
    amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

    decimals = decimals || 0; // por si la variable no fue fue pasada

    // si no es un numero o es igual a cero retorno el mismo cero
    if (isNaN(amount) || amount === 0)
        return parseFloat(0).toFixed(decimals);

    // si es mayor o menor que cero retorno el valor formateado como numero
    amount = '' + amount.toFixed(decimals);

    var amount_parts = amount.split('.'),
        regexp = /(\d+)(\d{3})/;

    while (regexp.test(amount_parts[0]))
        amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    return amount_parts.join('.');
}
    /* Mensajes */
    function MuestraMensaje(string_mensaje) {
        try {
            swal({
                title: "Correcto",
                text: string_mensaje,
                icon: "success"
            });
        } catch (e) {
            alert(string_mensaje);
        }
    }

    function MuestraErrorServidor(string_mensaje) {
        try {
            swal({
                title: "Error",
                text: string_mensaje,
                icon: "error"
            });
        } catch (e) {
            alert(string_mensaje);
        }
    }

    function MuestraErrorAplicacion(string_mensaje) {
        try {
            console.log("Error --> " + string_mensaje);
            swal({
                title: "Error",
                text: "Ocurrió un error en la aplicación.",
                icon: "error"
            });
        } catch (e) {
            alert(string_mensaje);
        }
    }