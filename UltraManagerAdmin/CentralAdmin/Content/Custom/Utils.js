

function llenarTabla(div_table, data, modalName) {
    var myIds = [];
    $("#" + div_table + "> table > tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var p = data[i];
        var count = -1;

        var row = $("<tr />")
        $("#" + div_table + " > table").append(row);

        for (var prop in p) {
            if (p.hasOwnProperty(prop)) {
                ++count;
            }
        }
        var offset = 0;
        var idRow = "";
        for (var key in p) {
            if (offset == 0) {
                idRow = p[key];
                myIds.push(idRow);
            }

            if (p.hasOwnProperty(key)) {
                var uppered = key.toUpperCase();
                if (uppered.search("ESTADO") > -1) {
                    row.append($("<td><input type='checkbox' disabled " + (p[key] == 0 ? '' : 'checked') + "></td>"));
                }
                else row.append($("<td>" + p[key] + "</td>"));
            }

            if (count == offset) {
                row.append($("<td id='td_" + idRow + "'><a class='btn btn-secondary bg-gray' style='margin-right: 5px;' onclick='convertirAFactura(\"" + idRow + "\");'>\
                                <i class='fa fa-file' aria-hidden='true'></i></a><a class='btn btn-primary' style='margin-right: 5px;' onclick='" + modalName + "(\"" + idRow + "\");'>\
                                <i class='fa fa-edit' aria-hidden='true'></i></a></td>"));
            }
            offset++;
        }
    }
    EndLoadTable("",div_table, true);
    return myIds;
}