
window.common = (function () {
    var common = {};



    common.getFragment = function getFragment() {
        if (window.location.hash.indexOf("#") === 0) {
            return parseQueryString(window.location.hash.substr(1));
        } else {
            return {};
        }
    };

    function parseQueryString(queryString) {
        var data = {},
            pairs, pair, separatorIndex, escapedKey, escapedValue, key, value;

        if (queryString === null) {
            return data;
        }

        pairs = queryString.split("&");

        for (var i = 0; i < pairs.length; i++) {
            pair = pairs[i];
            separatorIndex = pair.indexOf("=");

            if (separatorIndex === -1) {
                escapedKey = pair;
                escapedValue = null;
            } else {
                escapedKey = pair.substr(0, separatorIndex);
                escapedValue = pair.substr(separatorIndex + 1);
            }

            key = decodeURIComponent(escapedKey);
            value = decodeURIComponent(escapedValue);

            data[key] = value;
        }

        return data;
    }

    return common;
})();

$(document).ready(function () {

    // Configuración para que el Ajax no sea Asincronico
    $.ajaxSetup({
        async: false
    });

    //var moment = require('moment-timezone');
    moment.tz.setDefault('UTC');
    moment.locale('es-es');
    //var dec = moment("2014-12-01T00:00:00Z");
    ////console.log(moment.tz.names());
    //var s = dec.toLocaleString();
    //console.log(s);

    (function ($) {
        $.postJSON = function (url, data) {
            var o = {
                url: url,
                type: "POST",
                dataType: "json",
                contentType: 'application/json; charset=utf-8'
            };
            if (data !== undefined) {
                o.data = JSON.stringify(data);
            }
            return $.ajax(o);
        };
    }(jQuery));
    
    $('#DiagPrinAdm').autocomplete({
        minLength: 3,
        source:function (request, response)
            {
                $.ajax({
                    url : "/cases/GetCIE10",
                    type : "GET",
                    data: { term: $('#DiagPrinAdm').val(), max: 15, code: "-J"  },
                    dataType: "json",
                    async: true,
                    success: function (data) {
                        response($.map(data, function (el) {
                            return {
                                label: el.label,
                                value: el.value
                            };
                        }));
                    }
                });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                alert("Select a valid option");
                $('#DiagPrinAdm').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#DiagPrinAdmVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#DiagPrinAdm').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    $('#DiagEg').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/cases/GetCIE10",
                type: "GET",
                data: { term: $('#DiagEg').val(), max: 15, code: "" },
                dataType: "json",
                async: true,
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.label,
                            value: el.value
                        };
                    }));
                }
            });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                alert("Select a valid option");
                $('#DiagEg').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#DiagEgVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#DiagEg').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    $('#Salon').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/cases/GetSalon",
                type: "GET",
                data: { term: $('#Salon').val(), max: 15 },
                dataType: "json",
                async: true,
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.label,
                            value: el.value
                        };
                    }));
                }
            });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                // do whatever you want to when the item is not found
                alert("Select a valid option");
                $('#Salon').val("");
            }

        },
        select: function (event, ui) {
            // just in case you want to see the ID
            var accountVal = ui.item.value;
            $('#SalonVal').val(accountVal).change();

            // now set the label in the textbox
            var accountText = ui.item.label;
            $('#Salon').val(accountText).change();

            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });
    
    $('#SearchUbicaResid').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/cases/GetSearchUbicaResid",
                type: "GET",
                data: { term: $('#SearchUbicaResid').val(), max: 15, code: "-J" },
                dataType: "json",
                async: true,
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (el) {
                        return {
                            label: el.label,
                            value: el.value,
                            typeubic: el.typeubic,
                            areaID: el.areaID,
                            areaName: el.areaName,
                            stateID: el.stateID,
                            stateName: el.stateName,
                            neighborhoodID: el.neighborhoodID,
                            neighborhoodName: el.neighborhoodName,
                            hamletID: el.hamletID,
                            hamletName: el.hamletName,
                            colonyID: el.colonyID,
                            colonyName: el.colonyName
                        };
                    }));
                }
            });
        },
        change: function (event, ui) {
            if (ui.item) {
                // do whatever you want to when the item is found
            }
            else {
                alert("Select a valid option");
                $('#SearchUbicaResid').val("");
            }
        },
        select: function (event, ui) {
            var selectedValue = ui.item.value;                      // just in case you want to see the ID
            var selectedText = ui.item.label;                       // now set the label in the textbox
            var typeUbic = ui.item.typeubic;
            var areaID = ui.item.areaID;
            var areaName = ui.item.areaName;
            var stateID = ui.item.stateID;
            var stateName = ui.item.stateName;
            var neighborhoodID = ui.item.neighborhoodID;
            var neighborhoodName = ui.item.neighborhoodName;
            var hamletID = ui.item.hamletID;
            var hamletName = ui.item.hamletName;
            var colonyID = ui.item.colonyID;
            var colonyName = ui.item.colonyName;

            $('#SearchUbicaResid').val("").change();
            
            if (typeUbic == "AR") {
                $('#Area').val(areaID).change();
            } else if (typeUbic == "ST") {
                $('#Area').val(areaID).change();
                $('#provincia').val(stateID).change();
            } else if (typeUbic == "NE") {
                $('#Area').val(areaID).change();
                $('#provincia').val(stateID).change();
                $('#Neighborhoods').val(neighborhoodID).change();
            } else if (typeUbic == "HA") {
                $('#Area').val(areaID).change();
                $('#provincia').val(stateID).change();
                $('#Neighborhoods').val(neighborhoodID).change();
                $('#Hamlets').val(hamletID).change();
            } else if (typeUbic == "CO") {
                $('#Area').val(areaID).change();
                $('#provincia').val(stateID).change();
                $('#Neighborhoods').val(neighborhoodID).change(); 
                $('#Hamlets').val(hamletID).change();
                $('#Colonies').val(colonyID).change();
            }
            //****
            return false;
        },
        focus: function (event, ui) {
            // this is to prevent showing an ID in the textbox instead of name 
            // when the user tries to select using the up/down arrow of his keyboard
            //$('#DiagPrinAdm').val(ui.item.label);
            return false;
        }
    });

    //$('#NoExpediente').autocomplete({
    //    minLength: 8,
    //    source: function (request, response) {
    //        $.ajax({
    //            url: "/cases/GetPatientInformation",
    //            type: "POST",
    //            data: { DTP: $('#DocumentType').val(), DNP: $('#NoExpediente').val(), max: 15 },
    //            dataType: "json",
    //            async: true,
    //            success: function (data) {
    //                response($.map(data, function (el) {
    //                    return {
    //                        label: el.label,
    //                        value: el.value
    //                    };
    //                }));
    //            }
    //        });
    //    },
    //    change: function (event, ui) {
    //        if (ui.item) {
    //            // do whatever you want to when the item is found
    //        }
    //        else {
    //            alert("Seleccione una opción valida");
    //            $('#NoExpediente').val("");
    //        }

    //    },
    //    select: function (event, ui) {
    //        // just in case you want to see the ID
    //        var accountVal = ui.item.value;
    //        $('#NoExpediente').val(accountVal).change();

    //        // now set the label in the textbox
    //        var accountText = ui.item.label;
    //        $('#NoExpediente').val(accountText).change();

    //        return false;
    //    },
    //    focus: function (event, ui) {
    //        // this is to prevent showing an ID in the textbox instead of name 
    //        // when the user tries to select using the up/down arrow of his keyboard
    //        //$('#DiagPrinAdm').val(ui.item.label);
    //        return false;
    //    }
    //});

    /*
    function updateSelect(selectedValue, selectedText, cCampo, bSelect) {
        var newOptions = new Object();
        newOptions[selectedValue] = selectedText;

        var select = $(cCampo);

        if (select.prop) {
            var options = select.prop('options');
        }
        else {
            var options = select.attr('options');
        }
        //$('option', select).remove();						        // Elimina todas las opciones existente

        //$.each(newOptions, function (val, text) {			        // Agregando las nuevas opciones
        //    options[options.length] = new Option(text, val);
        //});

        //if(bSelect == true)
            select.val(selectedValue).change();							// Seleccionando un elemento
    }// END updateSelect 
    */
})

/*
function cargarListaAreas(countryID, areaID) {
    console.log("cargarListaAreas->START");
    $.get("/cases/GetAreas", { CountryID: countryID }, function (data) {
        $("#AreaID").empty();
        $.each(data, function (index, row) {
            if (areaID == row.Id)
                $("#AreaID").append("<option value='" + row.Id + "' selected='selected'>" + row.Name + "</option>")
            else
                $("#AreaID").append("<option value='" + row.Id + "'>" + row.Name + "</option>")
        });
    });
    console.log("cargarListaAreas->END");
}
*/

