/******************************************************* CONFIGURACIÓN DE PLUGINS **********************************************/
function TableSimpleInit(vchClassName) {
    $('.' + vchClassName).each(function () {
        var myTable = $(this);
        var myTableName = myTable["0"].id;

        if (myTable.find("thead").length === 0) {
            myTable.append('<thead></thead>');
            myTable.find('thead').append(myTable.find("tr:has(th)"));
        }

        var division = this;
        while (division.tagName !== "DIV")
            division = division.parentNode;
        if (division.tagName === 'DIV') {
            var iDiv = document.createElement('div');
            iDiv.style.overflow = "scroll";
            iDiv.style.overflow = "auto";
            division.appendChild(iDiv);
            $(iDiv).append(myTable);
        }

        $(myTable).removeAttr("border");
        $(myTable).removeAttr("rules");
        $(myTable).css("width:", "100%");
        $('#' + myTableName + ' > thead > tr > th').css("background-color", "#fff");
        $('#' + myTableName + ' > thead > tr > th').css("color", "#6b6b6b");
        $('#' + myTableName + ' > thead > tr').css("color", "#6b6b6b");
    });
}

function DataTableInit(NExportacion, vchClassName, bitTools, bitPaginate) {
    //Añadir clases de estilo css
    $('.' + vchClassName).addClass("table table-bordered table-striped");

    //Iniciar plugin
    $('.' + vchClassName).each(function () {
        var mydataTable = $(this);
        var mydataTablaName = mydataTable["0"].id;

        

        //Elimina plugin existente
        var dtApply = mydataTable.DataTable();
        dtApply.destroy();

        //Habilitar Columnas con fechas
        var indexVal = 0;
        var aryJSONColTable = [];
        $('#' + mydataTablaName + ' thead .uk-date-column').each(function () {
            indexVal = $(this).index();
            aryJSONColTable.push({
                "sType": "uk_date",
                "aTargets": [indexVal]
            });
        });


        //Habilitar ordenamiento
        if (!bitPaginate) {
            aryJSONColTable.push({
                "bSortable": false,
                "aTargets": ["_all"]
            });
        }


        //Habilitar botones
        var notExport = $('#' + mydataTablaName).hasClass("dataTable-not-export");
        var buttonsExport;
        if (notExport) {
            buttonsExport = [];
        }
        else if ($('#' + mydataTablaName).hasClass("dataTable-only-excel")) {
            buttonsExport = [
                { extend: 'excel', title: 'Excel', text: 'Excel' }
            ];
        }
        else {
            buttonsExport = [
                { extend: 'excel', title: NExportacion},
                { extend: 'pdf', title: NExportacion },
                { extend: 'copy', title: NExportacion},
                { extend: 'print', title: NExportacion },
                { extend: 'colvis', title: NExportacion }
            ];
        }


        //Ordenar columna default
        var sortColumnDefault;
        var hasSortDesc = $('#' + mydataTablaName + ' thead .sort-desc-column');
        var hasSortAsc = $('#' + mydataTablaName + ' thead .sort-asc-column');

        if (hasSortDesc.length == 1) {
            sortColumnDefault = [[hasSortDesc.index(), "desc"]];
        }
        else if (hasSortAsc.length == 1) {
            sortColumnDefault = [[hasSortAsc.index(), "asc"]];
        }
        else {
            sortColumnDefault = [];
        }


        //Habilitar plugin
        dtApply = mydataTable.DataTable({
            "aaSorting": sortColumnDefault,
            "bStateSave": true,
            "searching": bitTools,
            "paginate": bitPaginate,
            "aoColumnDefs": aryJSONColTable,
            "language": dtable_i18n,

            dom: "<'row' <'col-12'l><'col-12 col-md-6'B><'col-12 col-md-6'f>>" +
                "<'row' <'col-12'tr>>" +
                "<'row' <'col-12 col-sm-6'i><'col-12 col-sm-6'p>>",

            buttons: buttonsExport,
            "destroy": true,
            "responsive": true,
            "autoWidth": false,
            "select": true,
            "processing": true,
            "serverSide": false,
        });

        // Funcionalidad, selección de fila
        try {
            $("#" + mydataTablaName + " tbody").on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    dtApply.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
        }
        catch (e) {
            console.log("Funcionalidad, selección de fila no disponible");
        }

        // Funcionalidad, agrupar filas
        try {
            var hasGroupRow = $('#' + mydataTablaName + ' thead .group-row-column');
            if (hasGroupRow.length == 1) {
                GroupRowsByColumn(hasGroupRow, mydataTablaName);

                $('#' + mydataTablaName).on('draw.dt', function () {
                    GroupRowsByColumn(hasGroupRow, mydataTablaName);
                });
            }
        }
        catch (e) {
            console.log("Funcionalidad, agrupar filas no disponible");
        }

        // Funcionalidad, filtro por columnas (text)
        try {
            var hasFilterText = $('#' + mydataTablaName + ' thead .filter-column').length;
            if (hasFilterText > 0) {
                dtApply.columns('.filter-column').every(function () {
                    var that = this;

                    //Remover input existentes
                    $("input", this.header()).remove();

                    // Crear input por cada columna
                    $('<input type="text" class="form-control input-sm" style="width: 100%; height: 25px; font-weight: 300 !important;font-size: 13px !important;" />')
                        .appendTo(this.header())
                        .on('keyup change', function () {
                            if (that.search() !== this.value) {
                                that.search(this.value).draw();
                                if (this.value != "")
                                    $(this).css("border-color", "#1ab394");
                                else
                                    $(this).css("border-color", "#e5e6e7");
                            }
                        })
                        .on('click', function () {
                            return false;
                        });
                });
            }
        }
        catch (e) {
            console.log("Funcionalidad, filtro por columnas (text) no disponible");
        }


        // Funcionalidad, filtro por columnas (select)
        try {
            var hasFilterSelect = $('#' + mydataTablaName + ' thead .filter-column-select').length;
            if (hasFilterSelect > 0) {
                dtApply.columns('.filter-column-select').every(function () {
                    var that = this;

                    //Remover select existentes
                    $("select", this.header()).remove();

                    // Crear select por cada columna
                    var select = $('<br /><select class="form-control input-sm" style="width: 100%;" />')
                        .append($('<option value="">- Todos -</option>'))
                        .appendTo(this.header())
                        .on('change', function () {
                            var search_input = $(this).val();
                            var character_specials = "\^${}[]().+?|-&";
                            for (j = 0; j < character_specials.length; j++) {
                                search_input = search_input.replace(character_specials.charAt(j), "\\" + character_specials.charAt(j));
                            }

                            var regExSearch = (search_input == '' ? '' : '^' + search_input + '$');

                            if (regExSearch != "")
                                $(this).css("border-color", "#1ab394");
                            else
                                $(this).css("border-color", "#e5e6e7");

                            that.search(regExSearch, true, false).draw();
                        })
                        .on('click', function () {
                            return false;
                        });;

                    // Obtener los datos unicos de la columna
                    this.cache('search').sort().unique()
                        .each(function (d) {
                            select.append($('<option value="' + d + '">' + d + '</option>'));
                        });
                });
            }
        }
        catch (e) {
            console.log("Funcionalidad, filtro por columnas (select) no disponible");
        }


        // Funcionalidad, crear botón limpiador para filtro por columnas
        try {
            if (hasFilterText > 0 || hasFilterSelect > 0) {

                var clean_filter = '<div style="margin-left:5px;float: right;"><div class="dt-buttons btn-group">\
                                    <a class="btn btn-outline btn-danger" style="box-shadow: none;padding: 6px 8px;font-size: 12px;" tabindex="0" id="filter_' + mydataTablaName + '">\
                                        <span>Limpiar</span>\
                                    </a>\
                                    </div></div>';

                $("#" + mydataTablaName + "_wrapper").prepend(clean_filter);

                $("#filter_" + mydataTablaName).on('click', function (e) {
                    dtApply.search("").draw();

                    dtApply.columns(".filter-column-select, .filter-column").every(function () {
                        $("input", this.header()).css('border-color', '#e5e6e7').val("");
                        $("select", this.header()).css('border-color', '#e5e6e7').val("");
                        var that = this;
                        that.search("").draw();
                    });
                });
            }
        }
        catch (e) {
            console.log("Funcionalidad, crear botón limpiador para filtro por columnas no disponible");
        }


        // Funcionalidad, ocultar columnas detalle
        try {
            var hasDetail = $('#' + mydataTablaName + ' thead .detail-hide-column').length;
            if (hasDetail > 0) {
                HideColumnTable(false, mydataTablaName);
                var divSwitch = '<div class="input-group m-b" style="width: 70px;float: right; margin-left:5px;"> \
                                <span class="input-group-addon" style= "padding-right:0!important; padding-top: 4px !important;padding-bottom: 3px !important;border-top-left-radius: 3px;border-bottom-left-radius: 3px;" > \
                                    <input type= "checkbox" class="switch-green" onchange= "HideColumnTable_OnClick(this, ' + mydataTablaName + ')" /> \
                                </span > \
                                <span class="input-group-addon" style="padding-left:5!important; padding-top: 4px !important;padding-bottom: 3px !important;border-top-right-radius: 3px;border-bottom-right-radius: 3px;"> \
                                    Detalle \
                                </span> \
                            </div > ';
                $("#" + mydataTablaName + "_wrapper").prepend(divSwitch);
            }
        }
        catch (e) {
            console.log("Funcionalidad, ocultar columnas detalle no disponible");
        }


        // Funcionalidad, suma de una columna
        try {
            var hasSummation = $('#' + mydataTablaName + ' thead .sum-column').length;
            if (hasSummation > 0) {
                SummationColumn(mydataTablaName);

                $('#' + mydataTablaName).on('draw.dt', function () {
                    SummationColumn(mydataTablaName);
                });
            }
        }
        catch (e) {
            console.log("Funcionalidad, suma de una columna no disponible");
        }
    });

    // UK Date Sorting DataTable
    jQuery.fn.dataTableExt.oSort['uk_date-asc'] = function (a, b) {
        var ukDatea = a.split('/');
        var ukDateb = b.split('/');
        var x, y;
        if (ukDatea[2].indexOf(' ') != -1) {
            var ukDateA_time = ukDatea[2].split(' ');
            ukDatea[2] = ukDateA_time[0];
            var time_A = ukDateA_time[1].split(':');
            //x = (ukDatea[2] + ukDatea[1] + ukDatea[0] + time_A[0] + time_A[1]) * 1;

            var ukDateB_time = ukDateb[2].split(' ');
            ukDateb[2] = ukDateB_time[0];
            var time_B = ukDateB_time[1].split(':');
            //y = (ukDateb[2] + ukDateb[1] + ukDateb[0] + time_B[0] + time_B[1]) * 1;
            x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
        }
        else {
            x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
        }

        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    };

    // UK Date Sorting DataTable
    jQuery.fn.dataTableExt.oSort['uk_date-desc'] = function (a, b) {
        var ukDatea = a.split('/');
        var ukDateb = b.split('/');
        var x, y;
        if (ukDatea[2].indexOf(' ') != -1) {
            var ukDateA_time = ukDatea[2].split(' ');
            ukDatea[2] = ukDateA_time[0];
            var time_A = ukDateA_time[1].split(':');
            //x = (ukDatea[2] + ukDatea[1] + ukDatea[0] + time_A[0] + time_A[1]) * 1;

            var ukDateB_time = ukDateb[2].split(' ');
            ukDateb[2] = ukDateB_time[0];
            var time_B = ukDateB_time[1].split(':');
            //y = (ukDateb[2] + ukDateb[1] + ukDateb[0] + time_B[0] + time_B[1]) * 1;
            x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
        }
        else {
            x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
        }

        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    };
}

function GroupRowsByColumn(hasGroupRow, vchtable) {
    try {
        var rowsTable = document.getElementById(vchtable).getElementsByTagName('tr');
        var dataColumn = rowsTable[0].getElementsByTagName('td')[hasGroupRow.index()].innerText;

        for (i = 0; i < rowsTable.length; i++) {
            $(rowsTable[i].getElementsByTagName('td')).css("border-top", "none");

            var cell = rowsTable[i].getElementsByTagName('td')[hasGroupRow.index()];
            if (cell != undefined && i > 0) {
                if (cell.innerText != dataColumn) {
                    dataColumn = cell.innerText;
                    $(rowsTable[i].getElementsByTagName('td')).css("border-top", "5px solid #8dacca");
                }
            }
        }
    }
    catch (e) {
        return;
    }
}

function HideColumnTable_OnClick(chkControl, vchTable) {
    try {
        var bitHide = $(chkControl).is(":checked");
        HideColumnTable(bitHide, vchTable.id);
    }
    catch (e) {
        return;
    }
}

function HideColumnTable(bitHide, vchtable) {
    try {
        $('#' + vchtable + ' thead .detail-hide-column').each(function () {
            var intIndex = $(this).index();

            var attrDisplay = bitHide ? '' : 'none';
            fila = document.getElementById(vchtable).getElementsByTagName('tr');
            for (i = 0; i < fila.length; i++) {
                var col1 = fila[i].getElementsByTagName('th')[intIndex];
                var col2 = fila[i].getElementsByTagName('td')[intIndex];
                if (col1 != undefined) {
                    $(col1).css('display', attrDisplay);
                }
                else if (col2 != undefined) {
                    $(col2).css('display', attrDisplay);
                    $(col2).css('background-color', "#ffeaea");
                    $(col2).css('color', "#353535");
                }
            }
        });
    }
    catch (e) {
        return;
    }
}

function SummationColumn(vchtable) {
    try {
        $("#sumation_" + vchtable).remove();

        var regMoney = /^\$(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$/;
        var rowsTable = document.getElementById(vchtable).getElementsByTagName('tr');
        var columnsTotal = '';

        $('#' + vchtable + ' thead .sum-column').each(function () {
            var isMoney = false;
            var isSummation = false;
            var columnSummation = 0;
            var columnIndex = $(this).index();

            $.each(rowsTable, function (index, item) {
                var dataNumeric = 0.0;
                var cellData = item.getElementsByTagName('td')[columnIndex];
                if (cellData != undefined) {
                    isMoney = regMoney.test(cellData.innerText);

                    if (isMoney || $.isNumeric(cellData.innerText)) {
                        dataNumeric = cellData.innerText.replace(/[^0-9\.-]+/g, "");
                        columnSummation += parseFloat(dataNumeric);
                        isSummation = true;
                    }
                    else {
                        return false;
                    }
                }
            });

            if (isSummation && isMoney) {
                var money = $.getFormattedCurrency(columnSummation);
                var headTitle = document.getElementById(vchtable).getElementsByTagName('th')[columnIndex].innerText;
                columnsTotal += '<b>Sumatoria ' + headTitle + ': </b>' + money + '<br />';
            }
            else if (isSummation) {
                var headTitle = document.getElementById(vchtable).getElementsByTagName('th')[columnIndex].innerText;
                columnsTotal += '<b>Sumatoria ' + headTitle + ': </b>' + columnSummation + '<br />';
            }
        });

        if (columnsTotal != '') {
            var divSummation = '<div id="sumation_' + vchtable + '" style="background-color: #ffffff;padding: 15px;border: 1px solid #e7eaec; border-radius: 3px; width: auto; margin-left: 40%; margin-bottom: 10px;">' + columnsTotal + '</div>';
            $("#" + vchtable + "_wrapper").prepend(divSummation);
        }
    }
    catch (e) {
        return;
    }
}

function Select2Init(vchClassName) {
    $('.' + vchClassName).select2({
        width: '100%'
    });
}

function DateRangePickerInit(vchClassName) {
    $('.' + vchClassName).daterangepicker({
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Últimos 7 Días': [moment().subtract(6, 'days'), moment()],
            'Últimos 30 Días': [moment().subtract(29, 'days'), moment()],
            'Este Mes': [moment().startOf('month'), moment().endOf('month')],
            'Último Mes': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: {
            format: 'dd/mm/yyyy',
            cancelLabel: 'Limpiar',
            applyLabel: 'Aplicar',
            fromLabel: 'De',
            toLabel: 'a',
            customRangeLabel: 'Personalizado',
            todayRangeLabel: 'Hoy',
            daysOfWeek: [
                "Do",
                "Lu",
                "Ma",
                "Mi",
                "Ju",
                "Vi",
                "Sa"
            ],
            monthNames: [
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            ]
        }
    });

    $('.' + vchClassName).on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });
}

function DatePickerInit(vchClassName) {
    try {
        $.fn.datepicker.dates['es'] = {
            days: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
            daysShort: ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"],
            daysMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
            months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: "Hoy",
            monthsTitle: "Meses",
            clear: "Limpiar",
            weekStart: 1,
            format: "dd/mm/yyyy"
        };

        $('.' + vchClassName).datepicker({
            autoclose: true,
            language: 'es'
        });
    }
    catch (e) {
        console.log("DatePicker no disponible");
    }
}

function ClockPickerInit(vchClassName) {
    $('.' + vchClassName).clockpicker({
        donetext: 'Aplicar'
    });
}

function ColorPickerInit(vchClassName) {
    $('.' + vchClassName).colorpicker();
}

function SwitcherInit(vchClassName, vchColor) {
    try {
        $('.' + vchClassName).each(function () {
            var switchery = new Switchery(this, { size: 'small', color: vchColor });
        });

        //Coloca plugin a asp.net Checkbox
        $('.' + vchClassName + ' > input').each(function () {
            var switchery = new Switchery(this, { size: 'small', color: vchColor });
        });
    }
    catch (e) {
        console.log("Switchery no disponible");
    }
}

function CustomScrollbarInit(vchClassName) {
    try {
        $('.' + vchClassName).mCustomScrollbar({
            axis: "x",
            theme: "rounded-dark",
            scrollInertia: 10
        });
    }
    catch (e) {
        return;
    }
}

/******************************************************* CARGA DE PÁGINA *******************************************************/
function PluginsValidator() {
    try {
        // Console log
        var log_message = "";

        //DataTable
        if (!(typeof $.fn.DataTable === 'function' || typeof $.fn.dataTable === 'function'))
            log_message += "DataTable, ";
        else {
            //DataTableInit("dataTable-none", false, false);
            DataTableInit("dataTable-no-paginate", true, false);
            DataTableInit("dataTable-default", true, true);

            TableSimpleInit("table-none");
            TableSimpleInit("table-simple");
            $('.table-none').addClass("table table-striped table-condensed table-responsive table-bordered");
            $('.table-simple').addClass("table table-striped no-border-head");
        }

        //Select2
        if (typeof $.fn.select2 !== 'function')
            log_message += "Select2, ";
        else {
            Select2Init("ddlBusca");
            Select2Init("select2-default");
        }

        //DateRangerPicker
        if (typeof $.fn.daterangepicker !== 'function')
            log_message += "DateRangerPicker, ";
        else {
            DateRangePickerInit("dateRangerPicker-default");
        }

        //DatePicker
        if (typeof $.fn.datepicker !== 'function')
            log_message += "DatePicker, ";
        else {
            DatePickerInit("datepicker-default");
        }

        //ClockPicker
        if (typeof $.fn.clockpicker !== 'function')
            log_message += "ClockPicker, ";
        else {
            ClockPickerInit("clockpicker-default");
        }

        //ColorPicker
        if (typeof $.fn.colorpicker !== 'function')
            log_message += "ColorPicker, ";
        else {
            ColorPickerInit("colorPicker-default");
        }

        //Tooltip
        if (typeof $.fn.tooltip !== 'function')
            log_message += "Bootstrap JS, ";
        else {
            $('[data-toggle="tooltip"]').tooltip({ html: true });
        }

        //Switchery
        SwitcherInit("switch-green", "#1AB394");
        SwitcherInit("switch-red", "#ED5565");

        //CustomScrollbarInit
        if (typeof $.fn.mCustomScrollbar !== 'function')
            log_message += "mCustomScrollbar, ";
        else {
            CustomScrollbarInit("customScrollbar-default");
        }

        //Print console log
        console.log("Alert --> " + log_message + "no disponible(s).");
    }
    catch (e) {
        return;
    }
}