$(document).ready(function () {
        $('#Father_ID_hidd').attr('disabled', 'disabled');
        $('#Father_ID_hidd').hide();
    onLoad();    
    function onLoad() {
        var isVisibleButtonDelete = $("#DeleteInst").is(":visible");
        //console.log("Visible boton delete->" + isVisible);
        //****
        var isEdit = $("#ID").val() != undefined;
        //toggleAccessLevel();
        toggleRegion(isEdit);
        $("#RegionInstitucionalGroup").hide();
        $("#RegionSaludGroup").hide();
        $("#RegionPaisGroup").hide();
        $("#div-PCR").hide();
        $("#div-IFI").hide();
        $("#div-NPHL").hide();
        $("#div-OPbL").hide();

        if ($("#cod_region_institucional").children().length > 1)
            $("#RegionInstitucionalGroup").show();

        if ($("#cod_region_salud").children().length > 1)
            $("#RegionSaludGroup").show();

        if ($("#cod_region_pais").children().length > 1)
            $("#RegionPaisGroup").show();

        toggleInstitutionType();
        toggleAccessLevel();
        toggleLocationTypeID();            //#### CAFQ: 180911

        if (isVisibleButtonDelete) {
            $('#LocationTypeID').attr('disabled', 'disabled');
            $('#AreaID').attr('disabled', 'disabled');
            $('#FullName').attr('disabled', 'disabled');
            $('#Name').attr('disabled', 'disabled');
            $('#InstitutionType').attr('disabled', 'disabled');
            $('#AccessLevel').attr('disabled', 'disabled');
            $('#InstID').attr('disabled', 'disabled');
            $('#Father_ID').attr('disabled', 'disabled');
            $('#SARI').attr('disabled', 'disabled');
            $('#ILI').attr('disabled', 'disabled');
            $('#surv_unusual').attr('disabled', 'disabled');
            $('#IFI').attr('disabled', 'disabled');
            $('#PCR').attr('disabled', 'disabled');
            $('#Active').attr('disabled', 'disabled');
            $('#sentinel').attr('disabled', 'disabled');
            $('#LabNIC').attr('disabled', 'disabled');
            $('#orig_country').attr('disabled', 'disabled');
            $('#cod_region_institucional').attr('disabled', 'disabled');
            $('#cod_region_salud').attr('disabled', 'disabled');
            $('#cod_region_pais').attr('disabled', 'disabled');
            //InstType
        }
    }

    function toggleAccessLevel(isEdit) {
        accessLevel = $("#AccessLevel").val();
        console.log(accessLevel);
        if (accessLevel == "Service") {
            $("#div-father").show();
            if (isEdit) return;
            loadFatherInst();
        }
        else if (accessLevel == "Area") {
            $("#RegionInstitucionalGroup").hide();
            $("#RegionSaludGroup").hide();
            $("#RegionPaisGroup").hide();
            $("#div-father").hide();
        }
        else if (accessLevel != "Area" && accessLevel != "Service") {
            if ($("#cod_region_institucional").children().length > 1)
                $("#RegionInstitucionalGroup").show();

            if ($("#cod_region_salud").children().length > 1)
                $("#RegionSaludGroup").show();

            if ($("#cod_region_pais").children().length > 1)
                $("#RegionPaisGroup").show();
        }

        else {
            $("#div-father").hide();
        }
    }

    function toggleInstitutionType() {
        accessLevel = $("#InstitutionType").val();
        if (accessLevel == "Lab") {
            $("#div-PCR").show();
            $("#div-IFI").show();
            $("#div-NPHL").show();
            $("#div-OPbL").show();
            $("#div-LabNIC").show();
        } else {
            $("#div-PCR").hide();
            $("#div-IFI").hide();
            $("#div-NPHL").hide();
            $("#div-OPbL").hide();
            $("#div-LabNIC").hide();
        }
    }

    function toggleLocationTypeID() {                       //#### CAFQ: 180911
        LocationTypeID = $("#LocationTypeID").val();
        if (LocationTypeID == "2") {                        // Exterior
            $("#div-ForeignCountry").show();
            $("#div-ForeignInstitutionAddress").show();
        } else {
            $("#div-ForeignCountry").hide();
            $("#div-ForeignInstitutionAddress").hide();
        }
    }

    $("#AccessLevel").change(function () {
        toggleAccessLevel();
    });

    $("#LocationTypeID").change(function () {               //#### CAFQ: 180911
        toggleLocationTypeID();
    });

    $("#InstitutionType").change(function () {
        toggleInstitutionType();
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

    function loadFatherInst() {
        //console.log($("#Father_ID").val());

        $("#Father_ID").empty();
        $.ajax({
            type: 'POST',
            url: '/CatInstitucion/GetInstituciones',
            dataType: 'json',
            data: { tipo: $("#InstitutionType").val() },
            success: function (instituciones) {
                $.each(instituciones, function (i, institucion) {
                    $("#Father_ID").append('<option value="'
                                        + institucion.Value + '" ' + ((institucion.Value == $("#Father_ID_hidd").val() ) ?  'selected' : '')  +'>'
                                        + institucion.Text + '</option>');
                });
                //console.log(TempFatherID);
                //console.log($("#Father_ID").val());
                //$("#Father_ID").val(TempFatherID);

            },
            error: function (ex) {
                console.log('Failed to retrieve states.' + ex);
            }
        });
        return false;
    }

    function resetForLocationTypeID() {                     //#### CAFQ: 180911
        LocationTypeID = $("#LocationTypeID").val();
        if (LocationTypeID != "2") {                        // Exterior
            $("#ForeignCountryID").val("0");
            $("#ForeignInstitutionAddress").val("");
        }
    }
	
    function toggleRegion() {
        regInst = $("#cod_region_institucional");
        regSalud = $("#cod_region_salud");
        regPais = $("#cod_region_pais");

		regInstGroup = $("#RegionInstitucionalGroup");
		regSaludGroup = $("#RegionSaludGroup");
		regPaisGroup = $("#RegionPaisGroup");
	}
	
    $('form').on('submit', function (e) { 
	    $("#InstType").val($("#InstitutionType").val())
	    return;
    });
});


