$(document).ready(function () {

    onLoad();    

    function onLoad() {
        var isEdit = $("#ID").val() != undefined;
        $("#InstitutionFromID").prepend($("<option />").val("0").html("-- Seleccione --"));
        $("#InstitutionToID").prepend($("<option />").val("0").html("-- Seleccione --"));
        $("#InstitutionParentID").prepend($("<option />").val("0").html("-- Seleccione --"));

        if (!isEdit) {
            $("[name=InstitutionFromID]").val(0);
            $("[name=InstitutionToID]").val(0);
            $("[name=InstitutionParentID]").val(0);
        }
    }

});


