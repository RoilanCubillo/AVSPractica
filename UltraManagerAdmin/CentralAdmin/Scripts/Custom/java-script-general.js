//var elementPrototype = typeof HTMLElement !== "undefined" ? HTMLElement.prototype : Element.prototype;
//HTMLElement=typeof HTMLElement !== "undefined" ? HTMLElement : IHTMLElement;

function getAllElementsWithClass(esteSelf, Argumentos) {
    var matchingElements = [];
    try {
        matchingElements = Array.prototype.slice.call(esteSelf.getElementsByClassNames(Argumentos))
        return matchingElements;
    }
    catch (e) {
        try {
            var allElements = Array.prototype.slice.call(esteSelf.getElementsByTagName('*'));
            allElements.forEach(function (HTMLControlElement) {
                if (HTMLControlElement.className.indexOf(Argumentos) > 0) {
                    matchingElements.push(HTMLControlElement);
                }
            });
            return matchingElements;
        }
        catch (e) {
        }
    }
    return matchingElements;
} 

function getAllElementsWithAttribute(esteSelf, Argumentos) {
    var matchingElements = [];
    try {
        matchingElements = Array.prototype.slice.call(esteSelf.querySelectorAll("[" + Argumentos[0] + "]"))
        return matchingElements;
    }
    catch (e) {
        try {
            var allElements = Array.prototype.slice.call(esteSelf.getElementsByTagName('*'));
            allElements.forEach(function (HTMLControlElement) {
                if (HTMLControlElement.hasAttribute(Argumentos[0])) {
                    matchingElements.push(HTMLControlElement);
                }
            });
            return matchingElements;
        }
        catch (e) {
        }
    }
    return matchingElements;
}

function getAllElementsWithTagName(esteSelf, Argumentos) {
    var matchingElements = [];
    try {
        matchingElements = Array.prototype.slice.call(esteSelf.getElementsByTagName(Argumentos[0]))
        return matchingElements;
    }
    catch (e) {
        try {
            var allElements = Array.prototype.slice.call(esteSelf.getElementsByTagName('*'));
            allElements.forEach(function (HTMLControlElement) {
                if (HTMLControlElement.tagName === Argumentos[0]) {
                    matchingElements.push(HTMLControlElement);
                }
            });
            return matchingElements;
        }
        catch (e) {
            matchingElements = [];
        }
    }
    return matchingElements;
}

// Methods Injection By Fer XD
try {
    if (!String.prototype.contains) {
        String.prototype.contains = function () {
            //console.log("this: " + this);
            //console.log("arguments: " + arguments.length);
            //arguments.forEach(function(este){console.log(este)});
            //return this.indexOf(this, arguments) !== -1;
            return String.prototype.indexOf.apply(this, arguments) !== -1;
        };
    }

    HTMLDocument.prototype.getAllElementsWithAttribute = function () { return getAllElementsWithAttribute(this, arguments); };
    HTMLDocument.prototype.getAllElementsWithClass = function () { return getAllElementsWithClass(this, arguments); };
    HTMLDocument.prototype.getAllElementsWithTagName = function () { return getAllElementsWithTagName(this, arguments); };

    //Función para reemplazar comas en los números
    if (!String.prototype.replaceAll) {
        String.prototype.replaceAll = function () {
            return this.split(arguments[0]).join(arguments[1]);
        };
    }

    if (!HTMLElement.prototype.getAllElementsWithAttribute) {
        HTMLElement.prototype.getAllElementsWithAttribute = function () { return getAllElementsWithAttribute(this, arguments); };
    }
    if (!HTMLElement.prototype.getAllElementsWithTagName) {
        HTMLElement.prototype.getAllElementsWithTagName = function () { return getAllElementsWithTagName(this, arguments); };
    }
    if (!HTMLElement.prototype.getAllElementsWithClass) {
        HTMLElement.prototype.getAllElementsWithClass = function () { return getAllElementsWithClass(this, arguments); };
    }
}
catch (ex) {
    HTMLDivElement.prototype.getAllElementsWithAttribute = function () { return getAllElementsWithAttribute(this, arguments); };
    HTMLDivElement.prototype.getAllElementsWithTagName = function () { return getAllElementsWithTagName(this, arguments); };
    HTMLDivElement.prototype.getAllElementsWithClass = function () { return getAllElementsWithClass(this, arguments); };
    alert("¡¡¡¡¡Error grave!!!!!. Por favor, notifique al administrador. Det.: " + ex.message);
}


/* ---------------------------------------------------------------- */
//                      JavaScript Functions                    //
/* ---------------------------------------------------------------- */

function horizontalScrollingTables() {
    getAllElementsWithTagName('table').forEach(function (tablaActual) {
        horizontalScrollingTableObject(tablaActual);
    });
}
//Función para dar formato a las fechas en tipo dd/mm/yyyy
function Append0(stringDate) {
    try {
        var arrDate = stringDate.split("/");
        arrDate[0] = arrDate[0].length === 1 ? "0" + arrDate[0] : arrDate[0];
        arrDate[1] = arrDate[1].length === 1 ? "0" + arrDate[1] : arrDate[1];
        stringDate = arrDate.join("/");
    }
    catch (ex) {
        console.log("Error al intentar agregar 0");
    }
    return stringDate;
}
function formatJSONDate(jsonDate){
    var dateString = jsonDate.substr(6);
    var currentTime = new Date(parseInt(dateString ));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var date = day + "/" + month + "/" + year;
    return Append0(date);
}


function formatJSONDateUSFormat(jsonDate) {
    var dateString = jsonDate.substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var date = year + "/" + day + "/" + month;
    return Append0(date);
}

function parseDate(str, dateFormat) {
    if (str != "") {
        re = /(\d{1,2})\/(\d{1,2})\/(\d{4})/
        var arr = re.exec(str);
        var mDate;

        if (dateFormat) {
            if (dateFormat == "DD_MM_YYYY") {
                mDate = new Date(parseInt(arr[3]), parseInt(arr[2], 10) - 1, parseInt(arr[1], 10));
            }
            else if (dateFormat == "MM_DD_YYYY") {
                mDate = new Date(parseInt(arr[3]), parseInt(arr[1], 10) - 1, parseInt(arr[2], 10));
            }
        }
        else {
            mDate = new Date(parseInt(arr[3]), parseInt(arr[2], 10) - 1, parseInt(arr[1], 10));
        }
        return mDate;
    }

    
}

function horizontalScrollingTableObject(objetoTabla) {
    if (objetoTabla !== null) {
        if (objetoTabla.tagName.toLowerCase() === 'table') {
            var iDiv = document.createElement('div');
            iDiv.style.overflow = "scroll";
            iDiv.style.overflow = "auto";
            objetoTabla.parentNode.insertBefore(iDiv, objetoTabla);
            iDiv.appendChild(objetoTabla);
        }
    }
}

function chkSelectAll(sender) {
    if (sender.type !== 'checkbox') {
        console.log('Esta función sólo puede ser asignada a elementos CHECKBOX');
        return;
    }

    var chkSender = sender;
    var tbl = sender;
    var thOrtd = sender;

    while (tbl.tagName !== 'TABLE' && tbl.tagName !== null) {
        tbl = tbl.parentNode;
    }

    while (thOrtd.tagName !== 'TH' && thOrtd.tagName !== 'TD' && thOrtd.tagName !== null) {
        thOrtd = thOrtd.parentNode;
    }

    if (thOrtd === null || tbl === null) {
        console.log(sender + ' no está contenido en una tabla, o el formato de la tabla es invalido');
        return;
    }

    var indexCelda = thOrtd.cellIndex;

    if (indexCelda === null || indexCelda < 0) {
        console.log(sender + ' Error al buscar indice de celda');
        return;
    }


    for (var i = 0; i < tbl.rows.length; i++) {
        var celda = tbl.rows[i].cells[indexCelda];
        var chk = celda.getElementsByTagName('INPUT')[0];
        if (chk.type === 'checkbox')
            chk.checked = chkSender.checked;
    }
}

function UserException(message) {
    this.message = message;
    this.name = 'Excepcion de Usuario';
}

function tableNormalization(objetoTabla) {
    //Verifica si ya existe la etiqueta THEAD; si existe, no realiza ninguna acción que perjudique el HTML de la tabla
    if (objetoTabla.getElementsByTagName('thead').length > 0)
        return;

    for (var i = 0; i < objetoTabla.rows.length; i++) {
        if (objetoTabla.rows[i].getElementsByTagName('th').length > 0) {
            var theadTabla = document.createElement('thead');
            var filaTH = objetoTabla.rows[i];
            theadTabla.appendChild(filaTH);
            objetoTabla.insertBefore(theadTabla, objetoTabla.firstElementChild);
            break;
        }
    }

}

function getIndexIdPositionByColumnName(IdTabla, nombreColumnaId) {
    var Tabla = document.getElementById(IdTabla);
    var ninios = Tabla.getAllElementsWithTagName('th');
    nombreColumnaId = nombreColumnaId.toUpperCase();

    var encontradoFlag = false;
    var index = -1;

    ninios.forEach(function (elemento, indice, todoElArray) {
        if (elemento.textContent.toUpperCase().indexOf(nombreColumnaId) >= 0) {
            index = indice;
            return;
        }
    });
    return index;
}

function getCellValueByRowElement(sender, nombreColumnaId) {
    try {
        var Fila = sender;

        while (Fila.tagName !== "TR")
            Fila = Fila.parentNode;

        var Tabla = Fila;

        while (Tabla.tagName !== "TABLE")
            Tabla = Tabla.parentNode;

        var indexValueColumn = getIndexIdPositionByColumnName(Tabla.id, nombreColumnaId);
        if (indexValueColumn < 0)
            return;

        var columnas = Fila.getAllElementsWithTagName('TD'); //$(Fila).find('TD');

        if (columnas.length >= indexValueColumn) {
            var returnValue = columnas[indexValueColumn].textContent;
            return returnValue;
        }
    }
    catch (ex) {
        console.log("Error en JS General: " + ex.message);
    }

    return null;
}

function cleanControlsInDiv(IdDiv) {

    //var DivContenedor = $("div[id*='" + IdDiv + "']");
    var DivContenedor = document.getElementById(IdDiv);

    DivContenedor.getAllElementsWithTagName('input').forEach(function (CurrentElemento) {
        if (CurrentElemento.type === 'text')
            CurrentElemento.value = '';
    });

    DivContenedor.getAllElementsWithTagName('textarea').forEach(function (CurrentElemento) {
        CurrentElemento.value = '';
    });

    DivContenedor.getAllElementsWithTagName('iframe').forEach(function (CurrentElemento) {
        CurrentElemento.src = '';
    });

    DivContenedor.getAllElementsWithTagName('img').forEach(function (CurrentElemento) {
        CurrentElemento.src = '';
    });

    DivContenedor.getAllElementsWithTagName('select').forEach(function (CurrentElemento) {
        CurrentElemento.selectedIndex = 0;
    });
}

function comprobarForm(idName, mensajeError) {

    var ME = typeof mensajeError === 'undefined' ? '¡Algunos campos están vacíos y/o son incorrectos!' : mensajeError;

    var banderaSuccess = true;
    var entradas = document.getElementById(idName).getAllElementsWithAttribute("obligatorio"); //$("#" + idName).find('[obligatorio]');

    entradas.forEach(function (ElementoActual) {
        if (!validarInput(ElementoActual)) {
            banderaSuccess = false;
        }
    });

    if (!banderaSuccess) {
        try {
            toastNotification(ME, 'Por favor revise esta sección', 'warning');
            //toastr.success('Without any options', 'Simple notification!');
        }
        catch (ex) {
            try {
                crearDivFlotanteDestruible(ME, 5000);
            }
            catch (ex1) {
                alert('¡Algunos campos están vacíos y/ o son incorrectos!. Por favor revise el formulario');
                console.log("Error en comprobarForm(). Detalles: ", ex1.name, "::", ex1.message);
            }


        }
    }
    return banderaSuccess;
}

function comprobarFormWEvent(idName, evento, mensajeError) {

    var ME = typeof mensajeError === 'undefined' ? '¡Algunos campos están vacíos y/o son incorrectos!' : mensajeError;

    var banderaSuccess = true;
    var entradas = document.getElementById(idName).getAllElementsWithAttribute("obligatorio"); //$("#" + idName).find('[obligatorio]');

    entradas.forEach(function (ElementoActual) {
        if (!validarInput(ElementoActual))
            banderaSuccess = false;
    });

    if (!banderaSuccess) {
        try {
            evento.stopPropagation();
            evento.preventDefault();
            toastNotification(ME, 'Por favor revise esta sección', 'warning');
        }
        catch (ex) {
            try {
                crearDivFlotanteDestruible(ME, 5000);
            }
            catch (ex1) {
                alert('¡Algunos campos están vacíos y/ o son incorrectos!. Por favor revise el formulario');
                console.log("Error en comprobarForm(). Detalles: ", ex1.name, "::", ex1.message);
            }
        }
    }
    return banderaSuccess;
}

function comprobarFormConfirm(idName, cadenaMensaje) {
    var enviador = this;
    var banderaSuccess = true;


    var entradas = document.getElementById(idName).getAllElementsWithAttribute("obligatorio"); //$("#" + idName).find('[obligatorio]');
    entradas.forEach(function (ElementoActual) {
        if (!validarInput(ElementoActual))
            banderaSuccess = false;
    });

    if (!banderaSuccess) {
        try {
            toastNotification('¡Algunos campos están vacíos y/o son incorrectos!', 'Por favor revise el formulario', 'warning');
        }
        catch (e) {
            try {
                crearDivFlotanteDestruible(ME, 5000);
            }
            catch (ex1) {
                alert('¡Algunos campos están vacíos y/ o son incorrectos!. Por favor revise el formulario');
                console.log("Error en comprobarForm(). Detalles: ", ex1.name, "::", ex1.message);
            }
        }
        return false;
    }

    if (cadenaMensaje === undefined || cadenaMensaje === '') {
        return confirm('¿Realmente deseas continuar?');
    }
    else {
        return confirm(cadenaMensaje);
    }
}


function comprobarFormConfirWEvent(idName, evento, cadenaMensaje,MensajeError) {
    var enviador = this;
    var banderaSuccess = true;

    var ME = '¡Algunos campos están vacíos y/ o son incorrectos!. Por favor revise el formulario';
    ME = typeof MensajeError !== 'string' ? ME : MensajeError.trim().length > 0 ? MensajeError:ME;


    var entradas = document.getElementById(idName).getAllElementsWithAttribute("obligatorio"); //$("#" + idName).find('[obligatorio]');
    entradas.forEach(function (ElementoActual) {
        if (!validarInput(ElementoActual))
            banderaSuccess = false;
    });

    if (!banderaSuccess) {
        try {
            evento.stopPropagation();
            evento.preventDefault();
            toastNotification('¡Algunos campos están vacíos y/o son incorrectos!', 'Por favor revise el formulario', 'warning');
        }
        catch (e) {
            try {
                crearDivFlotanteDestruible(ME, 5000);
            }
            catch (ex1) {
                alert('¡Algunos campos están vacíos y/ o son incorrectos!. Por favor revise el formulario');
                console.log("Error en comprobarForm(). Detalles: ", ex1.name, "::", ex1.message);
            }
        }
        return false;
    }

    if (cadenaMensaje === undefined || cadenaMensaje === '') {
        if (!confirm('¿Realmente deseas continuar?')){
            evento.stopPropagation();
            evento.preventDefault();
            return false;
        }
    }
    else {
        if (!confirm(cadenaMensaje)){
            evento.stopPropagation();
            evento.preventDefault();
            return false;
        }
    }
    return true;
}


function toastNotification(mainTitle, Message, toastType) {
    if (typeof mainTitle !== 'string' || typeof Message !== 'string' || typeof toastType !== 'string') {
        //toastr.error('call function error', 'parameters have errors')
        throw new UserException('Error en los parametros de funcion toastNotification');
        return false;
    }

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "preventDuplicates": false,
        "positionClass": "toast-bottom-full-width",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "2500",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };


    if (toastType === 'success')
        toastr.sucess(Message, mainTitle);
    else if (toastType === 'warning')
        toastr.warning(Message, mainTitle);
    else if (toastType === 'error')
        toastr.error(Message, mainTitle);
    else if (toastType === 'info')
        toastr.info(Message, mainTitle);
    else
        toastr.info(Message, mainTitle);
}

function validarInput(objeto) {
    var bordeError = "1px dashed #ed5565";
    var bordeSuccess = "1px solid #d2d6de";
    var tipos = ['text', 'password', 'datetime-local', 'date', 'month', 'time', 'week', 'number', 'email', 'url', 'search', 'tel', 'color'];
    //var tipoInput = $(objeto).attr('tipo');
    var tipoInput = objeto.getAttribute('tipo');

    if (typeof tipoInput === 'string')
        tipoInput = tipoInput.toUpperCase();

    if (objeto.value) {
        objeto.value = objeto.value.trim();
        if (objeto.value.length === 0)
            return true;
    }

    if (objeto.type === 'checkbox') {
        if (objeto.checked) {
            objeto.style.border = bordeSuccess;
            return true;
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else if (objeto.type === "file") {
        if (objeto.value.length > 0) {
            return validarInputFile(objeto);
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else if (objeto.type === 'select-one') {
    //caso especial con control
        if (objeto.id == "ddlCodTarifaIVA") {
            objeto.style.border = bordeSuccess;
            $("#" + objeto.id).next(".select2-container").css("border", "0");
            return true;
        }

        if (objeto.selectedIndex > 0) {
            objeto.style.border = bordeSuccess;
            $("#" + objeto.id).next(".select2-container").css("border", "0");
            return true;
        }
        else {
            objeto.style.border = bordeError;
            $("#" + objeto.id).next(".select2-container").css("border", bordeError);
            return false;
        }
    }
    else if (objeto.tagName === 'TEXTAREA') {
        if (objeto.value.length > 0) {
            objeto.style.border = bordeSuccess;
            return true;
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else {
        for (var i = 0; i < tipos.length; i++) {
            if (objeto.type === tipos[i]) {
                if (objeto.value.length > 0) {

                    if (tipoInput === 'EMAIL') {
                        if (validarEmail(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ALFABETO') {
                        if (validarFecha(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ALFANUMERICO') {
                        if (validarFecha(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'FECHA') {
                        if (validarFecha(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'TIEMPO12') {
                        if (validarTiempo12(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'TIEMPO24') {
                        if (validarTiempo24(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'RFC') {
                        if (validarRFC(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'CURP') {
                        if (validarCURP(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ENTERO') {
                        if (validarEntero(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'FLOTANTE') {
                        if (validarFlotante(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            objeto.value = parseFloat(objeto.value);
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'CP') {
                        if (validarCodigoPostal(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            objeto.value = parseInt(objeto.value);
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }

                    objeto.style.border = bordeSuccess;
                    return true;
                }
                else {
                    objeto.style.border = bordeError;
                    //notificarErrorFormato(objeto);
                    return false;
                }
            }

        }
        return false;
    }
}

function validarTipo(objeto) {
    var bordeError = "2px dashed #ed5565";
    var bordeSuccess = "1px solid #5cb85c";
    var tipos = ['text', 'password', 'datetime-local', 'date', 'month', 'time', 'week', 'number', 'email', 'url', 'search', 'tel', 'color'];
    //var tipoInput = $(objeto).attr('tipo');
    var tipoInput = objeto.getAttribute('tipo');

    if (objeto.value) {
        objeto.value = objeto.value.trim();
        if (objeto.value.length === 0)
            return true;
    }

    if (typeof tipoInput === 'string')
        tipoInput = tipoInput.toUpperCase();

    if (objeto.type === 'checkbox') {
        if (objeto.checked) {
            objeto.style.border = bordeSuccess;
            return true;
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else if (objeto.type === "file") {
        if (objeto.value.length > 0) {
            return validarInputFile(objeto);
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else if (objeto.type === 'select-one') {
        if (objeto.selectedIndex > 0) {
            objeto.style.border = bordeSuccess;
            return true;
        }
        else {
            objeto.style.border = bordeError;
            return false;
        }
    }
    else {
        for (var i = 0; i < tipos.length; i++) {
            if (objeto.type === tipos[i]) {
                if (objeto.value.length > 0) {

                    if (tipoInput === 'EMAIL') {
                        if (validarEmail(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ALFABETO') {
                        if (validarAlfabeto(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ALFANUMERICO') {
                        if (validarAlfaNumerico(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'FECHA') {
                        if (validarFecha(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'TIEMPO12') {
                        if (validarTiempo12(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'TIEMPO24') {
                        if (validarTiempo24(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'RFC') {
                        if (validarRFC(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'CURP') {
                        if (validarCURP(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'ENTERO') {
                        if (validarEntero(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'FLOTANTE') {
                        if (validarFlotante(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            objeto.value = parseFloat(objeto.value);
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }
                    else if (tipoInput === 'CP') {
                        if (validarCodigoPostal(objeto.value)) {
                            objeto.style.border = bordeSuccess;
                            objeto.value = parseInt(objeto.value);
                            return true;
                        }
                        else {
                            objeto.style.border = bordeError;
                            return false;
                        }
                    }


                    objeto.style.border = bordeSuccess;
                    return true;
                }
                else {
                    objeto.style.border = bordeError;
                    notificarErrorFormato(objeto);
                    return false;
                }
            }

        }
        return false;
    }
}

function validarLongitud(objeto) {
    if (objeto.tagName !== 'INPUT' && objeto.tagName !== 'TEXTAREA')
        return true;
    if (objeto.tagName === 'INPUT') {
        if (objeto.type === 'checkbox')
            return true;
    }

    if (objeto.value) {
        objeto.value = objeto.value.trim();
        if (objeto.value.length === 0)
            return true;
    }

    var longitudInput = parseInt(objeto.getAttribute('longitud'));
    if (isNaN(longitudInput))
        return true;

    var ValidTipoDato = objeto.getAttribute('tipo') ? validarTipo(objeto) : true;

    var bordeError = "2px dashed #ed5565";
    var bordeSuccess = "1px solid #5cb85c";

    var IsLongValid = (objeto.value.trim().length === longitudInput) && ValidTipoDato;
    objeto.style.border = IsLongValid ? bordeSuccess : bordeError;
    return IsLongValid;
}

function validarEmail(stringEmail) {
    var emailRegex = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return emailRegex.test(stringEmail);
}

function validarFecha(sringFecha) {
    // First check for the pattern
    return /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/.test(sringFecha);
}

function validarTiempo24(stringTiempo24) {
    return (/^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$/.test(stringTiempo24));
}

function validarTiempo12(stringTiempo12) {
    return /^(1[0-2]|0?[1-9]):([0-5]?[0-9])/.test(stringTiempo12);
}

function validaRFC(stringRFC) {
    var _rfc_pattern_pm = "^(([A-Z�&]{3})([0-9]{2})([0][13578]|[1][02])(([0][1-9]|[12][\\d])|[3][01])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{3})([0-9]{2})([0][13456789]|[1][012])(([0][1-9]|[12][\\d])|[3][0])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{3})([02468][048]|[13579][26])[0][2]([0][1-9]|[12][\\d])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{3})([0-9]{2})[0][2]([0][1-9]|[1][0-9]|[2][0-8])([A-Z0-9]{3}))$";
    var _rfc_pattern_pf = "^(([A-Z�&]{4})([0-9]{2})([0][13578]|[1][02])(([0][1-9]|[12][\\d])|[3][01])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{4})([0-9]{2})([0][13456789]|[1][012])(([0][1-9]|[12][\\d])|[3][0])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{4})([02468][048]|[13579][26])[0][2]([0][1-9]|[12][\\d])([A-Z0-9]{3}))|" +
        "(([A-Z�&]{4})([0-9]{2})[0][2]([0][1-9]|[1][0-9]|[2][0-8])([A-Z0-9]{3}))$";

    if (stringRFC.toUpperCase().match(_rfc_pattern_pm) || stringRFC.toUpperCase().match(_rfc_pattern_pf)) {
        return true;
    } else {
        //alert("La estructura de la clave de RFC es incorrecta.");
        return false;
    }
}

function validarInputFile(objetoInput) {
    try {
        //var extensiones = $(objetoInput).attr('tipo');
        var extensiones = objetoInput.getAttribute('tipo');
        var localURLFile = objetoInput.value;

        if (extensiones === undefined)
            return true;
        else if (typeof extensiones === 'string') {

            if (extensiones === "")
                return true;

            var arregloExtensiones = extensiones.toLowerCase().split(",");

            if (arregloExtensiones.length > 0) {
                for (var i = 0; i < arregloExtensiones.length; i++) {

                    if (localURLFile.contains(arregloExtensiones[i]))
                        return true;
                }
                //return localURLFile.contains(localURLFile);
                return false;
            }
            return true;
        }
    }
    catch (excepcion) {
        return true;
    }
}

function validarEntero(stringEntero) {
    try {
        var numero = Number(stringEntero);
        return !isNaN(parseFloat(numero)) && isFinite(numero) && !stringEntero.contains('.');
    }
    catch (ex) {
        return false;
    }
}

function validarFlotante(stringEntero) {
    try {
        var numero = Number(stringEntero);
        return !isNaN(parseFloat(numero)) && isFinite(numero);
    }
    catch (ex) {
        return false;
    }
}

function validarCodigoPostal(stringEntero) {
    try {
        var numero = Number(stringEntero);
        return !isNaN(parseInt(numero)) && isFinite(numero) && !stringEntero.contains('.') && (stringEntero.length === 5);
    }
    catch (ex) {
        return false;
    }
}

function validarRFC(val) {
    var rfc = new RegExp(/^([A-Z&Ññ]{3}|[A-Z][AEIOUX][A-Z]{2})\d{2}((01|03|05|07|08|10|12)(0[1-9]|[12]\d|3[01])|02(0[1-9]|[12]\d)|(04|06|09|11)(0[1-9]|[12]\d|30))([A-Z0-9]{2}[0-9A])?$/i);
    var valid = rfc.test(val);
    return (valid) ? true : false;
}

function validarCURP(val) {
    var curp = new RegExp(/[A-Z]{1}[AEIOUX]{1}[A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[HM]{1}(AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z]{1}[0-9]{1}$/i);
    var valid = curp.test(val);
    return (valid) ? true : false;
}

function validarAlfabeto(stringAlfabeto) {
    return /^[A-Za-z]+$/.test(stringAlfabeto);
}

function validarAlfaNumerico(stringAlfabeto) {
    return /^[A-Za-z0-9]+$/.test(stringAlfabeto);
}

function soloEnteros(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode === 46 || charCode > 31) && (charCode < 48 || charCode > 57)) {
        evt.preventDefault();
        return false;
    }
    return true;
}

function soloFlotante(evt) {
    var charCode = evt.which || evt.keyCode;
    if (charCode === 46 && (charCode < 48 || charCode > 57)) {
        if ((evt.currentTarget.value.split(".")).length !== 1) {
            evt.preventDefault();
            return false;
        }
    }
    return true;
}

function soloTiempo12(evt) {
    evt.currentTarget.value = evt.currentTarget.value.trim();

    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode === 46 || charCode > 31) && (charCode < 48 || charCode > 57)
        && evt.currentTarget.value.contains('.')) {
        evt.preventDefault();
        return false;
    }
    return true;
}

function soloTiempo24() {

}


function clearArea(area) {
    try {
        var Contenedor;
        if (typeof area === 'string')
            Contenedor = document.getElementById(area);
        else if (typeof area === 'object')
            Contenedor = area;
        else if (typeof Contenedor === undefined)
            return;

        var elementos = ['input', 'select'];

        elementos.forEach(function (elementoActual, indice, todosElementos) {
            Contenedor.getAllElementsWithTagName(elementoActual).forEach(function (htmlElemento) {
                clearControl(htmlElemento);
            });
        });
    }
    catch (ex) {
        console.log("Error: " + ex.message)
    }

}


function clearControl(objeto) {//JS_f
    if (objeto === undefined || objeto === null)
        return;

    if (objeto.tagName === 'INPUT' && (objeto.type === 'checkbox' || objeto.type === 'radio')) {
        objeto.checked = false;
    }
    else if (objeto.tagName === 'INPUT') {
        objeto.value = '';
    }
    else if (objeto.tagName === 'SELECT') {
        objeto.selectedIndex = 0;
        try {
            if (isFunction($.fn.select2)) {
                if ($(objeto).attr("class").includes('select2'));

            }
        }
        catch (excepcion) {
            console.log('Select2 no está registrado: ' + excepcion.message);
        }

    }

    try {
        if (objeto.hasAttribute("obligatorio") || objeto.hasAttribute("tipo"))
            objeto.style.border = "";

        if (objeto.onchange !== null && typeof objeto.onchange === 'function')
            objeto.onchange();

    }
    catch (excepcion) {
        console.log('Error al accceder al atributo "obligatorio" o a la función CHANGE: ' + excepcion.message);
    }

}

function clearBorders() {
    document.getAllElementsWithAttribute('obligatorio').forEach(function (CurrentElemento) {
        CurrentElemento.style.border = "";
    });
    document.getAllElementsWithAttribute('tipo').forEach(function (CurrentElemento) {
        CurrentElemento.style.border = "";
    });

}

function clearBorder(objeto) {
    try {
        objeto.style.border = "";
    }
    catch (ex) {
        console.log('hubo un error al intentar limpiar un Borde en función \'ClearBorder\'. Error: ' + ex.message);
    }
}


function buscarElementosParaFiltrar(elementoBuscador) {
    try {

        if (elementoBuscador.tagName === "INPUT") {
            if (elementoBuscador.type !== "text")
                return false;
        }
        else
            return false;

        var nombreClaseBuscable = $(elementoBuscador).attr("buscar");

        if (typeof nombreClaseBuscable === undefined || typeof nombreClaseBuscable !== 'string')
            return false;
        if (nombreClaseBuscable.length <= 0)
            return false;

        var textoBuscar = elementoBuscador.value;
        var nombreClaseBuscable = $(elementoBuscador).attr("buscar");

        $("." + nombreClaseBuscable).each(function () {
            if (this.innerText.toLowerCase().contains(textoBuscar.toLowerCase()) || textoBuscar === "") {
                this.style.display = '';
                this.style.visibility = ''
            }
            else {
                this.style.display = 'none';
                this.style.visibility = 'hidden'
            }

        });

    }
    catch (excepcion) {
        console.log("Error en función 'buscarElementosParaFiltrar'. Error: " + excepcion.message);
    }

}

/* Relacionados con la creación dinámica de DIVs, TABLEs & OPTIONSselect */
function revisarExisteCSS(nombreCSS) {
    for (var i = 0; i < document.styleSheets.length; i++) {
        if (typeof document.styleSheets[i].href === 'string') {
            if (document.styleSheets[i].href.indexOf(nombreCSS) >= 0)
                return true;
        }
    }
    return false;
}

/* Funciones en las que se interactua con Objectos JSON */
function JSONtoHtmlTableCustomColumns(JSONElemento, arregloColumnas) {
    try {

        if (JSONElemento.length === 0)
            return null;

        var col = [];
        var colKeys = [];
        for (var i = 0; i < JSONElemento.length; i++) {
            for (var key in JSONElemento[i]) {
                var pos = arregloColumnas[0].indexOf(key);
                if (pos >= 0) {
                    Newkey = arregloColumnas[1][pos];
                    if (col.indexOf(Newkey) === -1) {
                        col.push(Newkey);
                        colKeys.push(key);
                    }
                }
                else
                    delete JSONElemento[i][key];
            }
        }

        // CREATE DYNAMIC TABLE.
        var table = document.createElement("table");

        // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE.
        var tr = table.insertRow(-1);                   // TABLE ROW.
        for (var i = 0; i < col.length; i++) {
            var th = document.createElement("th");      // TABLE HEADER.
            th.innerHTML = col[i];
            tr.appendChild(th);
        }

        // ADD JSON DATA TO THE TABLE AS ROWS.
        for (var i = 0; i < JSONElemento.length; i++) {
            tr = table.insertRow(-1);
            for (var j = 0; j < colKeys.length; j++) {
                //console.log(JSONElemento[i].keys);
                var tabCell = tr.insertCell(-1);
                tabCell.innerHTML = JSONElemento[i][colKeys[j]];
            }
        }
        return table;

    }
    catch (e) {
        return null;
    }
}

function JSONtoHtmlTable(JSONElemento) {
    try {
        if (JSONElemento.length === 0)
            return null;

        var col = [];

        for (var i = 0; i < JSONElemento.length; i++) {
            for (var key in JSONElemento[i]) {
                if (col.indexOf(key) === -1) {
                    col.push(key);
                }
            }
        }

        // CREATE DYNAMIC TABLE.
        var table = document.createElement("table");

        // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE.

        var tr = table.insertRow(-1);                   // TABLE ROW.

        for (var i = 0; i < col.length; i++) {
            var th = document.createElement("th");      // TABLE HEADER.
            th.innerHTML = col[i];
            tr.appendChild(th);
        }

        // ADD JSON DATA TO THE TABLE AS ROWS.
        for (var i = 0; i < JSONElemento.length; i++) {
            tr = table.insertRow(-1);
            for (var j = 0; j < col.length; j++) {
                var tabCell = tr.insertCell(-1);
                tabCell.innerHTML = JSONElemento[i][col[j]];
            }
        }
        return table;

    }
    catch (e) {
        return null;
    }

}

function limpiarSelect(selectid) {
    try {
        var select = document.getElementById(selectid);

        if (select === null)
            throw new UserException("No existe el elemento SELECT con id:" + selectid);

        var i;
        for (i = select.options.length - 1; i >= 0; i--) {
            select.remove(i);
        }
    }
    catch (ex) {
        console.log("Error en función 'limpiarSelect'. Error: " + ex.message);
    }
}

function eliminarSelectOptions(selectid) {
    try {
        var select = document.getElementById(selectid);

        if (select === null)
            throw new UserException("No existe el elemento SELECT con id:" + selectid);

        var i;
        for (i = select.options.length - 1; i >= 0; i--) {
            select.remove(i);
        }
    }
    catch (ex) {
        console.log("Error en función 'limpiarSelect'. Error: " + ex.message);
    }
}

function loadSelectOptionsFromJSON(selectid, JSONObject, stringValue, stringText) {
    try {
        var select = document.getElementById(selectid);

        if (select === null)
            throw new UserException("No existe el elemento SELECT con id:" + selectid);

        limpiarSelect(selectid);

        var keyValue = null;
        var keyText = null;

        for (var i = 0; i < JSONObject.length; i++) {
            for (var key in JSONObject[i]) {
                if (key === stringValue)
                    keyValue = key;
                else if (key === stringText)
                    keyText = key;
            }
        }

        if (keyValue === null || keyText === null)
            throw new UserException("No se hallaron los atributos ValueKey: " + stringValue + " o TextKey: " + stringText + "en el Objeto JSON");


        //Agrega la opciòn por defecto
        var option = document.createElement("option");
        option.text = "--- Seleccione ---";
        option.value = "0";
        select.appendChild(option);

        for (var i = 0; i < JSONObject.length; i++) {
            var option = document.createElement("option");
            option.text = JSONObject[i][keyText];//area.nombrearea;
            option.value = JSONObject[i][keyValue];//area.areaid;
            select.appendChild(option);
        }

    }
    catch (ex) {
        console.log("Error en función 'loadSelectOptionsFromJSON'. Error: " + ex.message);
    }
}
/****************************************** Start Dynami DIV Controls ******************************************/
// Crea un div Flotante de forma dinámica
function crearDivFlotante(elementoActivador) {
    try {
        var floatMessage = document.getElementById("MensajeFlotantoInformacionControlHTML" + elementoActivador.id);

        if (floatMessage === null) {
            var floatMessage = document.createElement("div");
            floatMessage.id = "MensajeFlotantoInformacionControlHTML" + elementoActivador.id;
            document.getElementsByTagName("body")[0].appendChild(floatMessage);
        }

        var ScrollY = window.scrollY === undefined ? window.pageYOffset : window.scrollY;
        var ScrollX = window.scrollX === undefined ? window.pageXOffset : window.scrollX;
        var rect = elementoActivador.getBoundingClientRect();
        var ancho = rect.left + ScrollX; //+elementoActivador.clientWidth ;
        var alto = rect.top + elementoActivador.clientHeight + ScrollY;

        if (revisarExisteCSS('bootstrap'))
            floatMessage.className = "alert bg-info";
        else
            floatMessage.style.backgroundColor = 'white';

        //Asigna el mismo ancho que el elemento Activador (Input en este caso)
        floatMessage.clientHeight = elementoActivador.clientHeight + 30;

        floatMessage.style.display = "none";
        floatMessage.style.position = "absolute";
        floatMessage.style.border = "black 1px solid";
        floatMessage.style.left = ancho + "px";
        floatMessage.style.top = alto + 15 + "px";
        floatMessage.style.zIndex = "100000";
        floatMessage.style.display = "";

        return floatMessage;
    }
    catch (ex) {
        console.log("Error en función 'crearDivFlotante'. Exc: " + ex.message);
    }
    return null;
}

function crearDivFlotanteDestruible(Mensaje, TimeOut, classeNombre) {
    try {
        var FloatDiv = document.createElement("div");
        FloatDiv.innerHTML = typeof Mensaje !== 'string' || Mensaje === '' || Mensaje === undefined || Mensaje === null ? "Hello World" : Mensaje;

        FloatDiv.style.maxWidth = "1000px";
        FloatDiv.style.maxHeight = "500px";
        FloatDiv.style.width = "max-content";
        FloatDiv.style.height = "max-content";
        FloatDiv.style.top = "0";
        FloatDiv.style.bottom = "0";
        FloatDiv.style.right = "0";
        FloatDiv.style.left = "0";
        FloatDiv.style.margin = "auto";
        //FloatDiv.style.zIndex = "90000";
        FloatDiv.style.textAlign = "center";

        if (typeof classeNombre !== 'string') {
            if (revisarExisteCSS('bootstrap'))
                FloatDiv.className = "alert alert-danger";
            else
                FloatDiv.style.backgroundColor = 'white';
        }
        else {
            FloatDiv.className = classeNombre;
        }


        TiempoOut = validarEntero(TimeOut.toString()) ? TimeOut : 5000;

        var OutterDiv = document.createElement("div");
        OutterDiv.style.display = "table";
        OutterDiv.style.position = "fixed";
        OutterDiv.style.height = "100%";
        OutterDiv.style.width = "100%";
        OutterDiv.style.zIndex = "90000";
        OutterDiv.style.top = "0";
        OutterDiv.style.bottom = "0";
        OutterDiv.style.right = "0";
        OutterDiv.style.left = "0";
        OutterDiv.style.margin = "auto";
        OutterDiv.onclick = function () {
            OutterDiv.parentNode.removeChild(OutterDiv);
        };


        var MiddleDiv = document.createElement("div");
        MiddleDiv.style.display = "table-cell";
        MiddleDiv.style.verticalAlign = "middle";


        setTimeout(function () {
            if (OutterDiv.parentNode !== null)
                OutterDiv.parentNode.removeChild(OutterDiv);
        }, TiempoOut);


        MiddleDiv.appendChild(FloatDiv);
        OutterDiv.appendChild(MiddleDiv);
        document.getElementsByTagName("body")[0].appendChild(OutterDiv);
        return OutterDiv;
    }
    catch (ex) {
        console.log("Error en función 'crearDivFlotanteDestruible'. Exc: " + ex.message);
    }
    return null;
}

function crearDivFlotanteDestruibleHTML(Mensaje, classe, TimeOut) {
    try {
        var FloatDiv = document.createElement("div");

        //floatMessage.id = "MensajeFlotantoInformacionControlHTML" + elementoActivador.id;
        document.getElementsByTagName("body")[0].appendChild(FloatDiv);
        FloatDiv.innerHTML = typeof Mensaje !== 'string' || Mensaje === '' || Mensaje === undefined || Mensaje === null ? "Hello World" : Mensaje;
        FloatDiv.onclick = function () {
            FloatDiv.parentNode.removeChild(FloatDiv);
        };


        //FloatDiv.style.minWidth = "300px";
        //FloatDiv.style.minHeight = "100px";
        FloatDiv.style.width = "max-content";
        FloatDiv.style.height = "max-content";
        FloatDiv.style.maxWidth = "1000px";
        FloatDiv.style.maxHeight = "500px";
        FloatDiv.style.opacity = "1";
        FloatDiv.style.position = "block";
        FloatDiv.style.top = 0;
        FloatDiv.style.bottom = 0;
        FloatDiv.style.right = 0;
        FloatDiv.style.left = 0;
        FloatDiv.style.zIndex = "10000";
        FloatDiv.style.margin = "auto";
        FloatDiv.style.textAlign = "center";

        if (typeof classe !== 'string') {
            if (revisarExisteCSS('bootstrap'))
                FloatDiv.className = "alert alert-info";
            else
                FloatDiv.style.backgroundColor = 'white';
        }
        else
            FloatDiv.className = classe;


        TiempoOut = typeof TimeOut === 'number' ? TimeOut : 3000;

        setTimeout(function () {
            if (FloatDiv.parentNode !== null)
                FloatDiv.parentNode.removeChild(FloatDiv);
        }, TiempoOut);

        return FloatDiv;
    }
    catch (ex) {
        console.log("Error en función 'crearDivFlotanteDestruible'. Exc: " + ex.message);
    }
    return null;
}

function crearProgressBar(elementoActivador, Mensaje) {
    try {
        var DivBackDrop = document.createElement("div");

        DivBackDrop.style.opacity = "0.8";
        DivBackDrop.style.position = "fixed";
        DivBackDrop.style.top = "0";
        DivBackDrop.style.right = "0";
        DivBackDrop.style.bottom = "0";
        DivBackDrop.style.left = "0";
        DivBackDrop.style.zIndex = "2000";
        DivBackDrop.style.backgroundColor = " #000";
        DivBackDrop.style.transition = "opacity .15s linear";

        var DivMsjPB = document.createElement("div");

        DivMsjPB.style.width = "1000px";
        DivMsjPB.style.height = "100px";
        DivMsjPB.style.opacity = "0.5";
        DivMsjPB.style.position = "fixed";
        DivMsjPB.style.top = "0";
        DivMsjPB.style.bottom = "0";
        DivMsjPB.style.right = "0";
        DivMsjPB.style.left = "0";
        DivMsjPB.style.zIndex = "10000";
        DivMsjPB.style.margin = "auto";
        DivMsjPB.innerText = Mensaje;
        DivMsjPB.style.textAlign = "center";

        document.getElementsByTagName("body")[0].appendChild(DivBackDrop);
        //document.getElementsByTagName("body")[0].appendChild(DivMsjPB);
        DivBackDrop.appendChild(DivMsjPB);


        return DivBackDrop;
    }
    catch (ex) {
        console.log("Error en función 'crearDivFlotante'. Exc: " + ex.message);
    }
    return null;
}


function crearProgressBarINSPINIA() {
    try {
        var classNombre = 'INSPINIA_ProgressBar' + location.pathname;
        var DivPB = document.createElement("div");
        DivPB.className = classNombre;
        DivPB.innerHTML = '<section class="progress-transaction"><div class="spinner-ucsj"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div><span>Procesando</span></div></section>';
        document.getElementsByTagName("body")[0].appendChild(DivPB);


        return DivPB;
    }
    catch (ex) {
        console.log("Error en función 'crearDivFlotante'. Exc: " + ex.message);
    }
    return null;
}

function destruirProgressBarINSPINIA() {
    try {
        var classNombre = 'INSPINIA_ProgressBar' + location.pathname;
        var ProgressBarInspiniaArray = Array.prototype.slice.call(document.getElementsByClassName(classNombre));

        ProgressBarInspiniaArray.forEach(function (currentElemnt) {
            currentElemnt.parentNode.removeChild(currentElemnt);
        });
    }
    catch (ex) {
        console.log("Error en función 'destruirProgressBarINSPINIA'. Exc: " + ex.message);
    }
}

function createBootStrapProgressBar(Mensaje) {
    try {
        var classNombre = 'BS_ProgressBar' + location.pathname;
        var ProgressBarMessage = typeof Mensaje === 'string' ? Mensaje : 'Procesando....';
        var DivBS_PB = document.createElement('div');
        DivBS_PB.className = classNombre
        DivBS_PB.innerHTML =
            '<div style="opacity:0.9;  display: table; position: fixed; width: 100%; height: 100%; top: 0; right: 0; bottom: 0; left: 0; z-index: 2000; background-color: #000; transition: opacity .15s linear;">' +
            '<div style="vertical-align: middle; display: table-cell;">' +
            '<div style="display: block; max-width: 1000px; height: 100px; margin: auto; text-align:center;" class="alert bg-light">' +
            '<h2 class="text-center text-black" style="background-color:white;">' + ProgressBarMessage + '</h2>' +
            '<div id="progressBarInModal" class="progress">' +
            '<div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0" style="width: 100%">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        document.getElementsByTagName("body")[0].appendChild(DivBS_PB);
    }
    catch (exc) {

    }

}

function destroyBootStrapProgressBar() {
    try {
        var classNombre = 'BS_ProgressBar' + location.pathname;
        var ProgressBarInspiniaArray = Array.prototype.slice.call(document.getElementsByClassName(classNombre));

        ProgressBarInspiniaArray.forEach(function (currentElemnt) {
            currentElemnt.parentNode.removeChild(currentElemnt);
        });
    }
    catch (ex) {
        console.log("Error en función 'destruirProgressBarINSPINIA'. Exc: " + ex.message);
    }
}
/****************************************** Ends Dynamic DIV Controls ******************************************/

/****************************************** PB_Injection ******************************************/
function PB_Injection() {
    var Elements_PB = document.querySelectorAll('[data-progress-bar]');
    var cantidad_PB_Elements = Elements_PB.length;
    if (cantidad_PB_Elements === 0)
        return;
    var StringPB = Elements_PB[cantidad_PB_Elements - 1].dataset.progressBar;

    if (typeof StringPB !== 'string')
        return;
    StringPB = StringPB.trim();
    if (StringPB.length === 0)
        return;
    var PB_FunctionCreate = null, PB_FunctionDestroy = null;

    switch (StringPB) {
        case 'BootStrapPB':
            PB_FunctionCreate = createBootStrapProgressBar;
            PB_FunctionDestroy = destroyBootStrapProgressBar;
            break;

        case 'InspiniaPB':
            PB_FunctionCreate = crearProgressBarINSPINIA;
            PB_FunctionDestroy = destruirProgressBarINSPINIA;
            break;
    }

    if (typeof PB_FunctionCreate !== 'function' || typeof PB_FunctionDestroy !== 'function')
        return;


    (function (open) {
        XMLHttpRequest.prototype.open = function () {
            xhrSender = this;
            var ConstrunctProgressBarFunction = PB_FunctionCreate;
            var DestructProgressBarFunction = PB_FunctionDestroy;
            this.addEventListener("readystatechange", function () {
                try {
                    switch (this.readyState) {
                        case 1:
                            setTimeout(function () {
                                if (xhrSender.readyState < 4)
                                    ConstrunctProgressBarFunction();
                            }, 50);

                            break;

                        case 4:
                            DestructProgressBarFunction();
                            break;
                    }
                }
                catch (ex) {
                    DestructProgressBarFunction();
                    console.log("entró a una excepción..");
                }
            }, false);
            open.apply(this, arguments);
        };
    })(XMLHttpRequest.prototype.open);
}
/****************************************** PB_Injection ******************************************/

/****************************************** Hover Elements ************************************/
function MouseOverHoverElement(event) {
    var elementListenerXD = event.currentTarget;
    var cHover = typeof elementListenerXD.dataset.colorHover === 'string' ? elementListenerXD.dataset.colorHover : "gray";
    if (typeof elementListenerXD.BGColorBackUp === 'undefined')
        elementListenerXD.BGColorBackUp = elementListenerXD.style.backgroundColor;
    elementListenerXD.style.backgroundColor = cHover;
}

function MouseLeaveHoverElement(event) {
    var elementListenerXD = event.currentTarget;
    elementListenerXD.style.backgroundColor = elementListenerXD.BGColorBackUp;
}

function HoverElements(objeto) {
    objeto = typeof objeto !== 'undefined' ? objeto : document;
    Array.prototype.slice.call(objeto.querySelectorAll('[data-color-hover]'))
        .forEach(function (currElement, index, array) {
            if (!HasEventListener("mouseover", currElement, 'MouseOverHoverElement'))
                currElement.addEventListener("mouseover", MouseOverHoverElement);
            if (!HasEventListener("mouseleave", currElement, 'MouseLeaveHoverElement'))
                currElement.addEventListener("mouseleave", MouseLeaveHoverElement);
        });
}
/****************************************** Hover Elements ************************************/

/* A partir de esta línea se utilizan funciones para crear mensajes flotante en cualquier control  */
/****************************************************************************************************/
function mostrarMensajeInformacion(MouseEvento) {
    var elementoActivador = MouseEvento.currentTarget;

    var floatMessage = document.getElementById("MensajeFlotantoInformacionControlHTML" + elementoActivador.id);
    if (floatMessage === null) {
        var floatMessage = document.createElement("div");
        floatMessage.innerHTML += elementoActivador.dataset.mensajeInfo;
        document.getElementsByTagName("body")[0].appendChild(floatMessage);
        floatMessage.id = "MensajeFlotantoInformacionControlHTML" + elementoActivador.id;
    }


    /*Inicia definicion de clase por atributo (si existe)*/
    var classMensaje = elementoActivador.dataset.mensajeClass;
    if (classMensaje !== null && typeof classMensaje === 'string')
        floatMessage.className = classMensaje;
    else {
        if (revisarExisteCSS('bootstrap'))
            floatMessage.className = "alert alert-info";
        else
            floatMessage.style.backgroundColor = 'white';
    }
    /*Termina definicion de clase por atributo (si existe)*/

    var ScrollY = window.scrollY === undefined ? window.pageYOffset : window.scrollY;
    var ScrollX = window.scrollX === undefined ? window.pageXOffset : window.scrollX;

    var rect = elementoActivador.getBoundingClientRect();
    //console.log(rect);
    floatMessage.style.position = "absolute";
    floatMessage.style.border = "black 1px solid";
    ///floatMessage.style.left = ancho + "px";
    //floatMessage.style.top = alto + "px";
    //floatMessage.style.zIndex="1100";
    floatMessage.style.display = "";

    var posMensaje = elementoActivador.dataset.mensajePosicion;
    posMensaje = typeof posMensaje !== 'string' ? 'BOTTOM' : posMensaje.trim().length === 0?'BOTTOM':posMensaje;
    if (posMensaje !== null && typeof posMensaje === 'string') {
        switch (posMensaje.toUpperCase()) {
            case 'TOP':
                var ancho = rect.left + ScrollX;
                var alto = rect.bottom + floatMessage.clientHeight + elementoActivador.height - 10 + ScrollY;
                floatMessage.style.left = ancho + "px";
                floatMessage.style.top = alto + "px";
                break;

            case 'BOTTOM':
                var ancho = rect.left + ScrollX;
                var alto = rect.top + elementoActivador.clientHeight + 10 + ScrollY;
                floatMessage.style.left = ancho + "px";
                floatMessage.style.top = alto + "px";
                break;
            case 'LEFT':
                var ancho = rect.left - 10 + ScrollX;
                var alto = rect.top + ScrollY;
                floatMessage.style.rigth = ancho + "px";
                floatMessage.style.top = alto + "px";
                //floatMessage.style.left = "0px";
                break;
            case 'RIGHT':
                var ancho = rect.left + elementoActivador.clientWidth + 10 + ScrollX;
                var alto = rect.top + ScrollY;
                floatMessage.style.left = ancho + "px";
                floatMessage.style.top = alto + "px";
                break;
        }
        //console.log("X: " + ancho + " Y: " + alto);
    }
    else {
        var ancho = rect.left + ScrollX;
        var alto = rect.top - elementoActivador.clientHeight - floatMessage.clientHeight + ScrollY;
    }

}

function ocultarMensajeInformacion(elemento) {
    var elementoActivador = elemento.currentTarget;
    var floatMessage = document.getElementById("MensajeFlotantoInformacionControlHTML" + elementoActivador.id);
    if (floatMessage === null)
        return;
    floatMessage.parentNode.removeChild(floatMessage);
    //floatMessage.innerText = "";
    //floatMessage.style.display = "none";
}

/* Terminan funciones estrictamente necesarias para crear un mensaje flotante  */
/****************************************************************************************************/

function HasEventListener(Evento, elementoHTML, funcionName) {
    try {
        funcionName = funcionName || "SinFuncionEspecifica";
        var retValue = false, eventListenerArr = getEventListeners(elementoHTML)[Evento];

        if (eventListenerArr instanceof Array)
            retValue = (funcionName === "SinFuncionEspecifica") ?
                true :
                eventListenerArr.some(function (elemento) { return (elemento.listener.name === funcionName) });
        return retValue;
    }
    catch (ex) {
        return false;
    }
}



function inicializarEventosElementosObligatorios(objeto) {
    objeto = typeof objeto !== 'undefined' ? objeto : document;
    /*Aplica los eventos que dispararan la comprobación de campos*/
    objeto.getAllElementsWithAttribute('obligatorio').forEach(function (elementoActual) {
        ['focus', 'blur', 'change'].forEach(function (nombreEvento, indiceNombreEvento, todosEventos) {
            if (!HasEventListener(nombreEvento, elementoActual, 'clearBorder') && nombreEvento === 'focus')
                elementoActual.addEventListener(nombreEvento, function () { clearBorder(elementoActual); }, false);
            if (HasEventListener(nombreEvento, elementoActual, 'validarInput') && nombreEvento === 'focus')
                return;
            elementoActual.addEventListener(nombreEvento, function () { validarInput(elementoActual); }, false);
        });
        //elementoActual.addEventListener('focus', function () {clearBorder(elementoActual); }, false);
        //elementoActual.addEventListener('blur', function () { validarInput(elementoActual); }, false);
        //elementoActual.addEventListener('change', function () { validarInput(elementoActual); }, false);
    });
}

function inicializarEventosElementosTipo(objeto) {
    /*Aplica los eventos que dispararan la comprobación de campos*/
    objeto = typeof objeto !== 'undefined' ? objeto : document;
    objeto.getAllElementsWithAttribute('tipo').forEach(function (elementoActual) {
        if (!HasEventListener('keypress', elementoActual, 'soloEnteros') && elementoActual.getAttribute('tipo').toUpperCase() === 'ENTERO')
            elementoActual.addEventListener('keypress', soloEnteros);
        else if (!HasEventListener('keypress', elementoActual, 'soloFlotante') && elementoActual.getAttribute('tipo').toUpperCase() === 'FLOTANTE')
            elementoActual.addEventListener('keypress', soloFlotante);

        elementoActual.addEventListener('focus', function () { elementoActual.style.border = ""; }, false);
        if (!HasEventListener('blur', elementoActual, 'validarTipo'))
            elementoActual.addEventListener('blur', function () { validarTipo(elementoActual); }, false);
        if (!HasEventListener('change', elementoActual, 'validarTipo'))
            elementoActual.addEventListener('change', function () { validarTipo(elementoActual); }, false);
    });
}

function inicializarEventosLongitud() {
    objeto = typeof objeto !== 'undefined' ? objeto : document;
    /*Aplica los eventos que dispararan la comprobación de campos*/
    objeto.getAllElementsWithAttribute('longitud').forEach(function (elementoActual) {
        elementoActual.addEventListener('focus', function () { elementoActual.style.border = ""; }, false);
        elementoActual.addEventListener('blur', function () { validarLongitud(elementoActual); }, false);
        elementoActual.addEventListener('change', function () { validarLongitud(elementoActual); }, false);
    });
}

function inicializarMensajesFlotantes(objeto) {
    objeto = typeof objeto !== 'undefined' ? objeto : document;
    var elementosTipo = objeto.getAllElementsWithAttribute("data-mensaje-info");
    elementosTipo.forEach(function (elementoActual, indice, TodosElementos) {
        elementoActual.onmouseover = mostrarMensajeInformacion;
        elementoActual.onmouseleave = ocultarMensajeInformacion;
    });
}


document.addEventListener("DOMContentLoaded", function (event) {
    PB_Injection();
    ValidatorListenersInjection();
});



function ValidatorListenersInjection(objeto) {
    objeto = typeof objeto !== 'undefined' ? objeto : document;

    inicializarEventosElementosObligatorios(objeto);//
    inicializarEventosElementosTipo(objeto);//
    inicializarEventosLongitud(objeto);//
    HoverElements(objeto);
    inicializarMensajesFlotantes(objeto);//

}

/* Funciones en las que se interactua con Objectos JSON */
/******************************************************* Ajax Objects *******************************************************/
function ajaxObjeto() {
    try {
        // Opera 8.0+, Firefox, Safari
        return new XMLHttpRequest();
    } catch (e) {
        // Internet Explorer Browsers
        try {
            return new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                return new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
                // Something went wrong
                alert("Your browser broke!");
                return false;
            }
        }
    }
}

function generarObjetoXMLHttpRequest() {
    this.objetoRequest = ajaxObjeto();
    this.Request = function (Metodo, url, Datos) {
        var objetoActual = this;

        this.objetoRequest.open(Metodo, url, true); //Preparamos la conexión.
        this.objetoRequest.setRequestHeader("Content-type", "application/json");

        this.objetoRequest.onreadystatechange = function () {
            switch (objetoActual.objetoRequest.readyState) {
                case 1: //Función que se llama cuando se está cargando.
                    objetoActual.cargando();
                    break;
                case 2: //Función que se llama cuando se a cargado.
                    objetoActual.cargado();
                    break;
                case 3: //Función que se llama cuando se está en interactivo.
                    objetoActual.interactivo();
                    break;
                case 4: /* Función que se llama cuando se completo la transmisión, se le envían 4 parámetros. */
                    objetoActual.completado(objetoActual.objetoRequest.status,
                        objetoActual.objetoRequest.statusText,
                        objetoActual.objetoRequest.responseText,
                        objetoActual.objetoRequest.responseXML);
                    break;
            }
        }
        this.objetoRequest.send(Datos); //Iniciamos la transmisión de datos
    }
    //Definimos una serie de funciones que sería posible utilizar y las
    this.cargando = function objetoRequestCargando() {
        //console.log('Cargando');
    }
    this.cargado = function objetoRequestCargado() {
        //console.log('Ya está CArgado');
    }
    this.interactivo = function objetoRequestInteractivo() {
        //console.log('Llamando a la función');
    }
    this.completado = function objetoRequestCompletado(estado,
        estadoTexto, respuestaTexto, respuestaXML) {
        console.log('Los datos ya están listos: ' + respuestaTexto);
    }
}
/******************************************************* Ajax Objects *******************************************************/

function getXHRTable(arregloColumnas, idDiv, urlRequest, classNombre, IdTabla) {
    var XHR = new generarObjetoXMLHttpRequest();
    XHR.completado = function() {
        var JSONElemento = JSON.parse(this.objetoRequest.responseText);
        JSONElemento = JSON.parse(JSONElemento);
        JSONtoHtmlTableCustomColumnsIntoDiv(JSONElemento, arregloColumnas, idDiv, classNombre, IdTabla);
    };
  XHR.Request("GET",urlRequest,null);
}

function JSONtoHtmlTableCustomColumnsIntoDiv(JSONElemento, arregloColumnas, idDiv, classNombre, IdTabla) {
    try {

        var FullJson = JSON.stringify(JSONElemento);
        FullJson = JSON.parse(FullJson);
        IdTabla = IdTabla ? IdTabla : "";

        if (JSONElemento.length === 0)
            return null;

        var ColsLength = arregloColumnas[0].length;

        var col = [];
        var colKeys = [];
        for (var i = 0; i < JSONElemento.length; i++) {
            for (var key in JSONElemento[i]) {
                var pos = arregloColumnas[0].indexOf(key);
                if (pos >= 0) {
                    Newkey = typeof arregloColumnas[1][pos] === 'string' ? arregloColumnas[1][pos]:'';
                    if (col.indexOf(Newkey) === -1) {
                        col.push(Newkey);
                        colKeys.push(key);
                    }
                }
                else
                    delete JSONElemento[i][key];
            }
        }



        // CREATE DYNAMIC TABLE.
        var table = document.createElement("table");
        table.id = IdTabla;
        table.className = classNombre;
        // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE.
        var tr = table.insertRow(-1);                   // TABLE ROW.
        for (var i = 0; i < col.length; i++) {
            var th = document.createElement("th");      // TABLE HEADER.
            th.innerHTML = col[i];
            tr.appendChild(th);
        }
        for (var i = col.length; i < ColsLength;i++) {
            var th = document.createElement("th");      // TABLE HEADER.
            th.innerHTML = arregloColumnas[1][i];
            tr.appendChild(th);
        }

        // ADD JSON DATA TO THE TABLE AS ROWS.
        for (var i = 0; i < JSONElemento.length; i++) {
            tr = table.insertRow(-1);
            for (var j = 0; j < colKeys.length; j++) {
                //console.log(JSONElemento[i].keys);
                var tabCell = tr.insertCell(-1);
                tabCell.innerHTML = JSONElemento[i][colKeys[j]];
            }

            for (var j = colKeys.length; j < colKeys.length + arregloColumnas[2].length; j++) {
                var tabCell = tr.insertCell(-1);
                tabCell.innerHTML = arregloColumnas[2][j - colKeys.length];


                var HTMLElemento = tabCell.getElementsByTagName('*')[0];
                if (HTMLElemento) {
                    var keyDepends = HTMLElemento.dataset.depends;
                    if (keyDepends && HTMLElemento.tagName === 'INPUT') {
                        if (HTMLElemento.type === 'checkbox') {
                            HTMLElemento.checked = Boolean(FullJson[i][keyDepends]);
                        }
                        else if (HTMLElemento.type === 'text') {
                            HTMLElemento.value = FullJson[i][keyDepends];
                        }
                    }
                    else {
                        HTMLElemento.value = FullJson[i][keyDepends];
                    }
                }

            }
        }


        var divTabla = document.getElementById(idDiv);
        divTabla.appendChild(table);
        tableNormalization(table);
        //$(table).DataTable();
        document.lastTable = table;
        ObjectDTInit(table);
    }
    catch (e) {
        console.log("Error al intentar crear table en Div. :" + e.message);
    }
}
$('.input-number').on('input', function () { 
    this.value = this.value.replace(/[^0-9]/g,'');
});