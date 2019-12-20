$(document).ready(function () {

    onLoad();

    function onLoad() {
        var total = 0;
        var total_Mas = 0;
        var total_Fem = 0;
        $("input[name^='Pop_PobTot']").each(function () {
           total += parseInt($(this).val());
        });
        $("input[name^='Pop_PobFem']").each(function () {
            total_Fem += parseInt($(this).val());
        });
        $("input[name^='Pop_PobMas']").each(function () {
            total_Mas += parseInt($(this).val());
        });
        $("#population").val(total);
        $("#Pop_PobTTotTot").val(total);
        $("#Pop_PobTFemTot").val(total_Fem);
        $("#Pop_PobTMasTot").val(total_Mas);
    }

    function toggleAccessLevel(isEdit) {
        accessLevel = $("#AccessLevel").val();
        if (accessLevel == "Country" || accessLevel == "Area" || accessLevel == "Regional") {
            $("#div-father").hide();
        } else {
            $("#div-father").show();
            if (isEdit) return;
            loadFatherInst();
        }
    }

    $("#AccessLevel").change(function () {
        toggleAccessLevel();
    });

    $("#InstitutionType").change(function () {
        toggleAccessLevel();
    });

    $("#cod_region_institucional").change(function () {
        toggleRegion();
    });

    $("#cod_region_salud").change(function () {
        toggleRegion();
    });

    $("#cod_region_pais").change(function () {
        toggleRegion();
    });


    $("input[name^='Pop_PobFem']").change(function (value_input) {
        if (isNaN($(this).val())) alert("Ingrese únicamente números");
        if ($(this).val() == "") $(this).val(0);

        //Suma total por renglon
        $(this).parent().next().next().children().val(parseInt($(this).val()) + parseInt($(this).parent().next().children().val()));

        //Suma total general
        onLoad();

    });

    $("input[name^='Pop_PobMas']").change(function (value_input) {

        if (isNaN($(this).val())) alert("Ingrese únicamente números");
        if ($(this).val() == "") $(this).val(0);

        console.log($(this).parent().prev());
        //Suma total por renglon
        $(this).parent().next().children().val(parseInt($(this).val()) + parseInt($(this).parent().prev().children().val()));

        //Suma total general
        onLoad();

    });


});


