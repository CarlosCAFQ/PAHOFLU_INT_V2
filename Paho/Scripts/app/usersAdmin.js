$(document).ready(function () {

    onLoad();    

    function onLoad() {

        $("#Modify_Epi").hide();
        $("#Modify_Lab").hide();
        $("#Close_case").hide();
        toggleRoleAdmin();

    }

    function toggleRoleAdmin() {
        //var AdminCountry = $("#ITy").val();
        console.log(AdminCountry);
        if (AdminCountry == "country") {
            $("#Modify_Epi").show();
            $("#Modify_Lab").show();
            $("#Close_case").show();
        } else {
            $("#Modify_Epi").hide();
            $("#checkModify_Epi").attr("checked", false);
            $("#Modify_Lab").hide();
            $("#checkModify_Lab").attr("checked", false);
            $("#Close_case").hide();
            $("#checkClose_case").attr("checked", false);
        }
    }

    //$("#checkAdmin").change(function () {
    //    if ($("#checkAdmin").is(":checked")) {
    //        $("#Modify_Epi").show();
    //        $("#Modify_Lab").show();
    //        $("#Close_case").show();
    //    } else {
    //        $("#Modify_Epi").hide();
    //        $("#checkModify_Epi").attr("checked", false);
    //        $("#Modify_Lab").hide();
    //        $("#checkModify_Lab").attr("checked", false);
    //        $("#Close_case").hide();
    //        $("#checkClose_case").attr("checked", false);
    //    }

    //});

 
});


