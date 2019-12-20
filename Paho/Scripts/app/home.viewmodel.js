function HomeViewModel(app, dataModel) {
    var self = this;    
    self.newEnable = ko.observable(false);
    self.showGrid = ko.observable(true);
    self.editMode = ko.observable(false);
    self.newMode = ko.observable(false);
    self.displayFilters = ko.observable(true);
    self.myHometown = ko.observable("");
    self.countries = ko.observableArray(countries);
    self.regions = ko.observableArray(regions);
    self.activecountries = ko.computed(function () {
        return $.grep( self.countries(), function(v) {
            return v.Active === true;
        });
    });
    self.CanConclude = ko.observable(false);
    self.SaveAndAdd_1 = ko.observable(true);
    self.SaveAndAdd_2 = ko.observable(true);
    self.SaveAndAdd_3 = ko.observable(true);
    // Catalogos de laboratorio
    self.CTT = ko.observableArray(CTT);
    self.CTR = ko.observableArray(CTR);
    self.CVT = ko.observableArray(CVT);
    self.CVST = ko.observableArray(CVST);
    self.CVL = ko.observableArray(CVL);

    self.CNP = ko.observableArray(CNP);
    self.CVS = ko.observableArray(CVS);

    // Control de flujo
    self.UserRole = ko.observable(URle);
    self.URadm = ko.observable(URleadm);
    self.URstf = ko.observable(URlestf);
    self.URmod_epi = ko.observable(URlemod_epi);
    self.URmod_lab = ko.observable(URlemod_lab);
    self.URclo_case = ko.observable(URleclo_case);
    self.URrpt = ko.observable(URlerpt);

    self.UsrCountry = ko.observable(selcty);
    // Alert Definition
    self.AlertDefinitionBegin = ko.observable(AlertDefinitionBegin);
    self.AlertDefinitionEnd = ko.observable(AlertDefinitionEnd);
    

    // Date of server
    var date_format_ = app.dataModel.date_format_;
    //var DoS_temp = (jQuery.type($("#DSrv").val()) === 'date') ? parseDate($("#DSrv").val(), date_format_) : null;
    //self.DoS = ko.observable(DoS_temp); // Date of server
    //console.log("Fecha del server")
    //console.log(jQuery.type($("#DSrv").val()));
    self.UsrRegInst = ko.observable(reg_inst_usr);
    self.UsrRegSalud = ko.observable(reg_salu_usr);
    self.UsrRegPais = ko.observable(reg_pais_usr);
    self.selectedCountryId = ko.observable("");
    self.selectedAreaId = ko.observable("");
    self.selectedRegionId = ko.observable("");
    self.institutions = ko.observableArray(institutions);
    self.InstitutionsCaseGenerarting = ko.observableArray(InstitutionsCaseGenerarting);
    self.selectedInstitutionId = ko.observable("");
    self.SCasenumber = ko.observable("");
    self.SNStartDate = ko.observable("");
    self.SNEndDate = ko.observable("");
    
    self.labs = ko.observableArray(labs);
    self.labsExternal = ko.observableArray(labsExternal);
    self.labsHospital = ko.observableArray([]);
    self.RecordHistoryItems = ko.observableArray([]);

    self.shouldcaselist = ko.computed(function () {
        return typeof self.selectedInstitutionId() !== "undefined" && self.selectedInstitutionId() !== "" && self.showGrid();
    }, self);    
    self.shouldtabs = ko.computed(function () {
        return self.editMode();
    });
    self.newButton = ko.computed(function () {
        return typeof self.selectedInstitutionId() !== "undefined" && self.selectedInstitutionId() !== "" && !self.editMode();
    });


    // Inicia EsRut validacion para Chile
    self.revisarDigito = function (componente) {
        var crut = componente
        largo = crut.length;
        if (largo < 2) {
            return false;
        }
        if (largo > 2)
            rut = crut.substring(0, largo - 1);
        else
            rut = crut.charAt(0);
        dv = crut.charAt(largo - 1);

        if (dv != '0' && dv != '1' && dv != '2' && dv != '3' && dv != '4' && dv != '5' && dv != '6' && dv != '7' && dv != '8' && dv != '9' && dv != 'k' && dv != 'K') {
            return false;
        }

        if (rut == null || dv == null)
            return 0

        var dvr = '0'
        suma = 0
        mul = 2

        for (i = rut.length - 1 ; i >= 0; i--) {
            suma = suma + rut.charAt(i) * mul
            if (mul == 7)
                mul = 2
            else
                mul++
        }
        res = suma % 11
        if (res == 1)
            dvr = 'k'
        else if (res == 0)
            dvr = '0'
        else {
            dvi = 11 - res
            dvr = dvi + ""
        }
        if (dvr.toUpperCase() != dv.toUpperCase()) {
            return false
        }

        return true
    }; // Fin revisar digito RUT

    // Inicia EsRut validacion para Chile
    self.EsRut = function (texto) {
        var tmpstr = "";
        for (i = 0; i < texto.length ; i++)
            if (texto.charAt(i) != ' ' && texto.charAt(i) != '.' && texto.charAt(i) != '-')
                tmpstr = tmpstr + texto.charAt(i);
        texto = tmpstr;
        largo = texto.length;

        if (largo < 2) {
            return false;
        }

        for (i = 0; i < largo ; i++) {
            if (texto.charAt(i) != "0" && texto.charAt(i) != "1" && texto.charAt(i) != "2" && texto.charAt(i) != "3" && texto.charAt(i) != "4" && texto.charAt(i) != "5" && texto.charAt(i) != "6" && texto.charAt(i) != "7" && texto.charAt(i) != "8" && texto.charAt(i) != "9" && texto.charAt(i) != "k" && texto.charAt(i) != "K") {
                return false;
            }
        }

        var invertido = "";
        for (i = (largo - 1), j = 0; i >= 0; i--, j++)
            invertido = invertido + texto.charAt(i);
        var dtexto = "";
        dtexto = dtexto + invertido.charAt(0);
        dtexto = dtexto + '-';
        cnt = 0;

        for (i = 1, j = 2; i < largo; i++, j++) {
            if (cnt == 3) {
                dtexto = dtexto + '.';
                j++;
                dtexto = dtexto + invertido.charAt(i);
                cnt = 1;
            }
            else {
                dtexto = dtexto + invertido.charAt(i);
                cnt++;
            }
        }

        invertido = "";
        for (i = (dtexto.length - 1), j = 0; i >= 0; i--, j++)
            invertido = invertido + dtexto.charAt(i);

        if (self.revisarDigito(texto))
            return true;

        return false;
    }; // fin EsRut

    self.esrut = function (rut) {
        //if (rut == 0) {
        //    alert("Debe ingresar un RUT");
        //    document.form1.Tx_RUN2.focus();//me posiciono en el campo nuevamente
        //    return false;
        //} else {
        if (rut.search("-") <= 0) {
            alert(viewValidateRUN + rut + viewValidateRUNSinDash);
            self.SCasenumber("");//limpio el valor del campo rut
            $("#SCasenumber").focus();//me posiciono en el campo nuevamente
            return false;
        }
        if (self.EsRut(rut) == false) {
            alert(viewValidateRUN + rut + viewValidateRUNNotValid);
            self.NoExpediente("");//limpio el valor del campo rut
            $("#SCasenumber").focus();//me posiciono en el campo nuevamente
            return false;
        } else {
            if (rut.length == 9) {
                var la = rut.substring(0, 8);
                var digito = rut.charAt(8);

            } else {
                var la = rut.substring(0, 8);
                var digito = rut.charAt(9);
            }

        }
        //}
    };  // Fin validacion es RUT

    self.SCasenumber.subscribe(function (NewRUN) {
        if (self.UsrCountry() == 7 && NewRUN != "" && NewRUN != "9.999.999") {
            self.esrut(NewRUN);
        }
    });

    self.ReloadFluCases = function () {
        if (typeof self.selectedInstitutionId() === 'undefined' && typeof self.selectedRegionId() === 'undefined' && typeof self.selectedAreaId() === 'undefined') {
            $("#caselist").jqGrid('clearGridData');
            $("#SCasenumber").val("");
            $("#SHospRegis").val("");
            $("#SCaseStatus").val("");
            $("#SNStartDate").val("");
            $("#SNEndDate").val("");
            $("#caselist_pend").jqGrid('clearGridData');
            //console.log("reload_1");

        }
        else {

            //if (self.selectedAreaId() > 0) {
            //    self.ReloadInstitutions();
            //}
            //if (self.selectedRegionId() > 0) {
            //    self.ReloadInstitutions();
            //}
            //else {
            //    self.loadInstitutions();
            //}
            $("#caselist").trigger("reloadGrid");
            $.getJSON(app.dataModel.getLabsHospitalUrl, { institutionId: self.selectedInstitutionId() }, function (data, status) {
                self.labsHospital(data.LabsHospital);
            });
            $("#caselist_pend").trigger('reloadGrid');
            //console.log("reload_2");
        }
    };

    self.NewFluCase = function () {
        //self.ResetFluCase();
        //self.EditFluCase();
        $("#RecordNumber").text(msgviewHome_btnNew);
        if ($("#ITy").val() == "1") {
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            $("#RegDate").attr('disabled', true);
            $("#tab-lab :input").prop('disabled', true);
            self.ResetFluCase();
            self.EditFluCase();
            if (app.Views.Contact.Id() == null) {
                $("a[href*='tab-case']").hide();
                $("#tab-case").hide();
                $("a[href*='tab-lab']").hide();
            }

        } else
        {
            $("#tab-lab :input").prop('disabled', false);
        }
        $("#tabs").tabs("option", "active", 0);
        self.selectedInstitutionId() != "" ?  app.Views.Contact.hospitalName($('#Hospitals option:selected').text()) : "";
    };


    self.CancelEdit = function () {
        $("#Hospitals option:selected").prop("selected", false);
        $("#Hospitals option:first").attr('selected', 'selected');
        //$('#Hospitals').removeAttr('selected').find('option:first').attr('selected', 'selected');
        self.selectedInstitutionId($("#Hospitals").val());
        self.displayFilters(true);
        self.editMode(false);
        self.showGrid(true);
        self.ReloadFluCases();

        //self.selectedInstitutionId("");
    };

    self.EditFluCase = function () {
        $("#caselist").jqGrid('clearGridData');
        $("#caselist_pend").jqGrid('clearGridData');
        self.showGrid(false);
        self.editMode(true);
        self.displayFilters(false);
        $('#tabs').tabs({ active: 0 });
        app.Views.Hospital.LabsHospital(self.labsHospital());
        app.Views.Lab.LabsResult(self.labs());
        //app.Views.Lab.LabsExterno(self.labs());

    };


    self.ResetFluCase = function () {
        app.Views.Contact.ResetContact();
        app.Views.GEO.ResetGEO();
        app.Views.Risk.ResetRisk();
        app.Views.Hospital.ResetHospital();
        app.Views.Lab.ResetLab();
        //$("#save").attr("disabled", false);
        //$("#saveGEO").attr("disabled", false);
        //$("#saveRisk").attr("disabled", false);
        //$("#saveHospital").attr("disabled", false);
        //$("#saveLab").attr("disabled", false);
        $("#saveGeneral").attr("disabled", false);
        $("#saveGeneralPrev").attr("disabled", true);

        //$("#cancel").attr("disabled", false);
        //$("#cancelGEO").attr("disabled", false);
        //$("#cancelRisk").attr("disabled", false);
        //$("#cancelHospital").attr("disabled", false);
        //$("#cancelLab").attr("disabled", false);
        $("#cancelGeneral").attr("disabled", false);
        //$("#BorrarGeneral").attr("disabled", false);
        $("#RecordNumber").text(msgviewHome_btnNew);
    };

    self.SaveAll = function (option_Save) {
        $("#save").attr("disabled", true);
        $("#saveGEO").attr("disabled", true);
        $("#saveRisk").attr("disabled", true);
        $("#saveHospital").attr("disabled", true);
        $("#saveLab").attr("disabled", true);
        $("#saveGeneral").attr("disabled", true);
        $("#saveGeneralPrev").attr("disabled", true); 
        $("#BorrarGeneral").attr("disabled", true);
        $("#o_S").val(option_Save);

        if ($("#ITy").val() == "2") {
            app.Views.Lab.SaveLab(function () {
                alert(viewValidateSavedInfo + app.Views.Lab.Id);
                if (option_Save == 1 || option_Save ==2)
                    app.Views.Home.CancelEdit();
                if (option_Save == 0)
                    app.Views.Home.NewFluCase();
            });
        }
        else {
            app.Views.Contact.SaveContact(function () {
                app.Views.GEO.SaveGEO(function () {
                    app.Views.Risk.SaveRisk(function () {
                        app.Views.Hospital.SaveHospital(function () {
                            if ($("#ITy").val() != "2") {
                                if (self.URmod_lab() == true) {
                                    app.Views.Lab.SaveLab(function () {
                                        alert(viewValidateSavedInfo + app.Views.Hospital.Id);
                                    });
                                } else {
                                    alert(viewValidateSavedRecord + app.Views.Hospital.Id);
                                }
                                
                                //app.Views.Home.CancelEdit();
                                if (option_Save == 1 || option_Save == 2) app.Views.Home.CancelEdit();
                                //console.log("Combo Hospitals" + $("#Hospitals").children().length);
                                //$("#Hospitals option:first").attr('selected', 'selected');
                                if (option_Save == 0) app.Views.Home.NewFluCase();

                                //app.Views.Home.ResetFluCase();
                            } else {
                                app.Views.Lab.SaveLab(function () {
                                    alert(viewValidateSavedRecord + app.Views.Hospital.Id);
                                    if (option_Save == 1 || option_Save == 2) app.Views.Home.CancelEdit();
                                    if (option_Save == 0) app.Views.Home.NewFluCase();
                                });
                            }
                            
                        });
                    });
                });
            });
        }
         
    };

    self.ValidateAll = function (option_Save) {
        if ($("#ITy").val() == "2") {
            app.Views.Lab.validate(function () {
                self.SaveAll(option_Save);
            });
        } else {
            app.Views.Contact.validate(function () {
                app.Views.GEO.validate(function () {
                    app.Views.Risk.validate(function () {
                        app.Views.Hospital.validate(function () {
                            if ($("#ITy").val() != "2") {
                                self.SaveAll(option_Save);
                            } else {
                                app.Views.Lab.validate(function () {
                                    self.SaveAll(option_Save);
                                });
                            }
                        });
                    });
                });
            });
        };
    };

    self.ReloadInstitutions = function () {        
        if (typeof self.selectedCountryId() === "undefined") {
            return;
        }
        self.loadInstitutions();
        $("#HospitalsGroup").show();
        $("#AreasGroup").show();
        $("#RegionalsGroup").show();
    };

    self.loadInstitutions = function () {
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() === "" ? self.UsrCountry() : self.selectedCountryId(), AreaID: self.selectedAreaId(), RegionID: self.selectedRegionId() }, function (data, status) {
            self.institutions(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
        
    };

    self.selectedAreaId.subscribe(function (NewAreaID) {

        if (NewAreaID > 0) {
            //("#Regionals").attr('disabled', true);
            self.selectedRegionId(0);

        }

        self.loadInstitutions();
        $("#caselist").trigger("reloadGrid");
        $("#caselist_pend").trigger('reloadGrid');

        if (NewAreaID == 0 && self.institutions().length > 1) {
            self.selectedInstitutionId(0);
        }

    });

    self.selectedRegionId.subscribe(function (NewRegionID) {

        if (NewRegionID > 0) {
            self.selectedAreaId(0);
        }

        self.loadInstitutions();
        $("#caselist").trigger("reloadGrid");
        $("#caselist_pend").trigger('reloadGrid');

        if (NewRegionID == 0 && self.institutions().length > 1) {
            self.selectedInstitutionId(0);
        }

    });

       
    Sammy(function () {
        this.get('#home', function () {           
            // Make a call to the protected Web API by passing in a Bearer Authorization Header
            $.ajax({
                method: 'get',
                url: app.dataModel.userInfoUrl,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    self.myHometown(data.hometown);
                }
            }); 
        });
       
        this.get('/', function () { this.app.runRoute('get', '#home') });
    });

    // Search case
    self.SearchFlucases = function () {
        if (typeof self.selectedInstitutionId() === 'undefined') {
            $("#caselist").jqGrid('clearGridData');
            $("#caselist_pend").jqGrid('clearGridData');
        }
        else {
            $("#caselist").trigger("reloadGrid");
            $("#caselist_pend").trigger("reloadGrid");
        }
    };

    self.FlowData = function () {
        //console.log("FlowData - frecord_flowMax -- " + app.Views.Contact.flow_max() + ", frecord_flowdata -- " + app.Views.Contact.flow_record() + ", finsti_flowdata -- " + app.Views.Contact.flow_institution() + ", dataStatement_flowdata -- " + app.Views.Contact.DataStatement() + ", userRole " + self.UserRole() + ", Inst" + $("#ITy").val());
        

        //if (($("#ITy").val() != 2) && self.UserRole() == "adm") {
        if (($("#ITy").val() != 2) && self.URadm() == true) {
            //console.log("aqui _ adm");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            //self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
        }
        //else if (($("#ITy").val() == "1" && self.UserRole() == "stf" && app.Views.Contact.flow_record() == app.Views.Contact.flow_max() && (app.Views.Contact.DataStatement() == 2 || app.Views.Contact.DataStatement() == null))) {
        //    console.log("aqui _ stf 1");
        //    $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
        //    self.ModDataNo();
        //    $("#tab-lab :input").prop('disabled', true);
        //}   // Esto fue comentariado porque ya no necesitamos flujo en la parte epidemiologica, fue desactivado por pedido de RRR
        //else if (($("#ITy").val() == "1" && self.UserRole() == "stf" && app.Views.Contact.flow_record() == 0)) {  // Este cambio porque ya no necesitamos flujo en la parte epidemiologica por requerimiento de RRR
        //else if (($("#ITy").val() == "1" && self.UserRole() == "stf")) {
        else if (($("#ITy").val() == "1" && self.URstf() == true)) {
            //console.log("aqui _ stf 2");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
         } else if ($("#ITy").val() == "1" &&  app.Views.Contact.Flow_Local_Institution_Epi() == false)
         {
             //console.log("aqui _ stf 3");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', true);
            self.ModDataNo();
            $("#tab-lab :input").prop('disabled', true);
        }
    };

    self.FlowDataHospital = function () {
        //console.log("FlowDataHospital");
            $("#div_hospital :input").prop('disabled', false);
    };

    self.FlowDataCaseStatus = function () {
        //console.log("FlowCaseStatus");
        var flow_check = $.grep(app.Views.Lab.LabTests(), function (x) {         
            return x.EndFlow() === "TRUE";
        });
        //Esta modificación es porque es necesario modificar todos los datos aunque no este cerrado el caso
        if (($("#ITy").val() != 2) && self.URmod_epi() == true) {
            //console.log("aqui _ CaseStatus 1"); //
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            $("#tab-lab :input").prop('disabled', true);
            $("a[href*='tab-case']").show();
            $("#tab-case").show()
            $("#CaseStatus").attr("disabled", true);
            $("#tabs").tabs("refresh");
            if (app.Views.Hospital.Destin() != "" && app.Views.Lab.CanConclude() == true && app.Views.Hospital.HospExDate() != null) {
                //console.log("aqui _ CaseStatus 1"); //
                $("#CaseStatus").attr("disabled", false);
                $('a[href="#tab-case"]').click();
            } else {
              //console.log("aqui _ CaseStatus2"); //
              if (app.Views.Hospital.CaseStatus() != "") {
                    $('a[href="#tab-contact"]').click();
                } else {
                    $('a[href="#tab-hospital"]').click();
                }
                
            }
            
            //$('#tab-case').tabs({ active:  });
        }
        if (($("#ITy").val() != 2) && self.URmod_epi() == true && app.Views.Hospital.CaseStatus() == "3") {
            //console.log("aqui _ CaseStatus 2"); //
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            $("#tab-lab :input").prop('disabled', true);
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", true);
            $("#tabs").tabs("refresh");
            //console.log("aqui _ CaseStatus 3"); //
            if(app.Views.Hospital.CaseStatus() != "") {
                $('a[href="#tab-contact"]').click();
            }
            else {
                $('a[href="#tab-case"]').click();
            }
            

            //} else if (app.Views.Hospital.CaseStatus() == "3" && self.UserRole() != "adm") {
        } else if (app.Views.Hospital.CaseStatus() == "3" && self.URmod_epi() != true) {
            //console.log("aqui _ CaseStatus 3"); //
            $("#tabs :input").prop('disabled', true); // Modificacion para que se pueda modificar el registro aunque este cerrado el caso.
            self.ModDataNo();
            if (self.OpenAlwaysLab() == true) {
                $("#PrintM1").prop('disabled', false);
                $("#addLabTest_1").prop('disabled', false);
                $("#PrintM2").prop('disabled', false);
                $("#addLabTest_2").prop('disabled', false);
                $("#PrintM3").prop('disabled', false);
                $("#addLabTest_3").prop('disabled', false);
                
            }
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", true);
            $("#tabs").tabs("refresh");
            //console.log("aqui _ CaseStatus 4"); //
            if(app.Views.Hospital.CaseStatus() != "") {
                    $('a[href="#tab-contact"]').click();
                }
                else {
                    $('a[href="#tab-case"]').click();
                }
        }
        //else if (flow_check.length > 0 && app.Views.Contact.DataStatement() == 2) {
        //    console.log("aqui _  Flow_check");
        //    $("a[href*='tab-case']").show();
        //    $("#tab-case").show();
        //    $("#CaseStatus").attr("disabled", false);
        //    $("#tabs").tabs("refresh");
            //}
        else if (app.Views.Hospital.IsSample() === "false" && (app.Views.Contact.IsSurv() == "2" || app.Views.Contact.SurvInusual() == true)) {
            //console.log("aqui _ CaseStatus ETI"); //
            //console.log("aqui _ CaseStatus SurvInusual"); //
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
            //console.log("aqui _ CaseStatus 5"); //
            if (app.Views.Hospital.CaseStatus() != "") {
                $('a[href="#tab-contact"]').click();
            }
            else {
                $('a[href="#tab-case"]').click();
            }
        }
        else if (app.Views.Hospital.IsSample() === "true" && app.Views.Contact.SurvInusual() == true && app.Views.Lab.CanConclude() == true) {
            //console.log("aqui _ CaseStatus SurvInusual"); //
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
            //console.log("aqui _ CaseStatus 5"); //
            if (app.Views.Hospital.CaseStatus() != "") {
                $('a[href="#tab-contact"]').click();
            }
            else {
                $('a[href="#tab-case"]').click();
            }
        }
        else if (app.Views.Contact.SurvSARI() == true && app.Views.Hospital.IsSample() === "true" && (app.Views.Lab.FinalResult() == "" || app.Views.Hospital.Destin() == "" || app.Views.Hospital.HospExDate() == "" || app.Views.Hospital.HospExDate() == null || app.Views.Lab.CanConclude() == false)) {
            //console.log("aqui _ no processed 3"); //
            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
        }
        else if ($("#ITy").val() == "1"
                && ((app.Views.Hospital.IsSample() === "true" && app.Views.Lab.Processed() === "false" && self.UsrCountry() != 15)
                    || (app.Views.Lab.NPHL() == true && app.Views.Hospital.IsSample() === "true" && app.Views.Lab.NPHL_Processed() === "false"))) {
            //console.log("aqui _  Flow status no processed"); //
            // revisar si hay que habilitar para CR
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
            $('a[href="#tab-hospital"]').click();
        }
        else if ($("#ITy").val() == "1"
            && ((app.Views.Hospital.IsSample() === "true" && app.Views.Lab.Processed() === "false" && app.Views.Lab.Processed_National() === "false" && self.UsrCountry() == 15))) {
            //console.log("aqui _  Flow status no processed"); //
            // Revisar si hay que habilitar para CR
            $("a[href*='tab-case']").show();
            $("#tab-case").show();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
            $('a[href="#tab-hospital"]').click();
        }
        else if ($("#ITy").val() == "1" && (app.Views.Hospital.CaseStatus() != "" && app.Views.Hospital.CaseStatus() != null)) {
            //console.log("aqui _ CaseStatus"); //
            $("#tab-lab :input").prop('disabled', true);
            self.FlowDataHospital();
        } else if (app.Views.Contact.SurvILI() == true && app.Views.Hospital.IsSample() === "true" && app.Views.Lab.CanConclude() == false) {
            console.log("aqui _ ILI CanConclude false 3"); //
            $("a[href*='tab-case']").hide();
            $("#tab-case").hide();
            $("#CaseStatus").attr("disabled", false);
            $("#tabs").tabs("refresh");
        }

        //console.log("Value - CanConclude - ")
        //console.log(app.Views.Lab.CanConclude());
        if (app.Views.Lab.CanConclude() == true && ($("#ITy").val() == "1" || (app.Views.Hospital.CaseStatus() == "" && self.URclo_case() == true) || (app.Views.Hospital.CaseStatus() == "" && self.URmod_epi() == true))) {
            //console.log("aqui _ CaseStatus IT = 1"); //
            //console.log(app.Views.Lab.CanConclude());
            //console.log(app.Views.Hospital.CaseStatus());
            //console.log(self.URmod_epi() == true);
            //console.log(self.URclo_case() == true);
            $("#HospExDate").attr("disabled", false);
            $("#Destin").attr("disabled", false)
            //console.log("aqui _ CaseStatus 6"); //
            if (app.Views.Hospital.Destin() != "" && app.Views.Lab.CanConclude() == true && app.Views.Hospital.HospExDate() != null) {
                $("#CaseStatus").attr("disabled", false);
                $("#tab-case").show();
                $("#tabs").tabs("refresh");
                if (app.Views.Hospital.CaseStatus() != "") {
                    $('a[href="#tab-contact"]').click();
                }
                else {
                    $('a[href="#tab-case"]').click();
                }
            } else {
                $('a[href="#tab-hospital"]').click();
            }

            if (app.Views.Contact.SurvILI() == true) {
                console.log("ILI true  ")
                $("a[href*='tab-case']").show();
                $("#tab-case").show();
                $("#CaseStatus").attr("disabled", false);
                $("#tabs").tabs("refresh");
                if (app.Views.Hospital.CaseStatus() != "") {
                    $('a[href="#tab-contact"]').click();
                }
                else {
                    $('a[href="#tab-case"]').click();
                }
            }

        } else
        {
            //console.log("aqui _ CaseStatus else"); //
            // Para Jamaica activar la fecha de egreso de hospitalización y el Destino para que lo puedan ingresar en cualquier momento
            if (self.UsrCountry() == 17 || (self.UsrCountry() == 7 && self.URmod_epi() == true)) {
                $("#HospExDate").attr("disabled", false);
                $("#Destin").attr("disabled", false)
            }
            else {
                $("#HospExDate").attr("disabled", true);
                $("#Destin").attr("disabled", true)
            }

            // Grupo genetico siempre editable para NIC
            if (lab_NIC_usr){                
                $("#GeneticGroup").attr("disabled", false);
                $("#GeneticGroup_2").attr("disabled", false);
                $("#GeneticGroup_3").attr("disabled", false);
            }
            else {
                $("#GeneticGroup").attr("disabled", true);
                $("#GeneticGroup_2").attr("disabled", true);
                $("#GeneticGroup_3").attr("disabled", true);
            }            
        }

        if ($("#ITy").val() == "2" && app.Views.Lab.NPHL() == true && self.UsrCountry() != 25) {
            //console.log("aqui _ CaseStatus 2 and NPHL"); //
            $("#tab-lab :input").prop('disabled', true);
            $("#Rec_Date_NPHL").prop('disabled', false);
            $("#Temp_NPHL").prop('disabled', false);
            $("#Identification_Test_NPHL").prop('disabled', false);
            $("#Ship_Date_NPHL").prop('disabled', false);
            $("#Observation_NPHL").prop('disabled', false);
            $("input[id*='NPHL_Processed']").prop('disabled', false);
            $("#NPHL_NoProRenId").prop('disabled', false);
            $("#NPHL_NoReason").prop('disabled', false);

            $("#Rec_Date_NPHL_2").prop('disabled', false);
            $("#Temp_NPHL_2").prop('disabled', false);
            $("#Identification_Test_NPHL_2").prop('disabled', false);
            $("#Ship_Date_NPHL_2").prop('disabled', false);
            $("#Observation_NPHL_2").prop('disabled', false);
            $("input[id*='NPHL_Processed_2']").prop('disabled', false);
            $("#NPHL_NoProRenId_2").prop('disabled', false);
            $("#NPHL_NoReason_2").prop('disabled', false);

            $("#Rec_Date_NPHL_3").prop('disabled', false);
            $("#Temp_NPHL_3").prop('disabled', false);
            $("#Identification_Test_NPHL_3").prop('disabled', false);
            $("#Ship_Date_NPHL_3").prop('disabled', false);
            $("#Observation_NPHL_3").prop('disabled', false);
            $("input[id*='NPHL_Processed_3']").prop('disabled', false);
            $("#NPHL_NoProRenId_3").prop('disabled', false);
            $("#NPHL_NoReason_3").prop('disabled', false);
            
            
        }

        if (self.URmod_lab() == true) {
            //console.log("aqui _ CaseStatus _ rol mod_lab");
            //$("#tab-lab :input").prop('disabled', false);
            //
            $("#LabTests :input").prop('disabled', false);
            $("#LabTests_Sample2 :input").prop('disabled', false);
            $("#LabTests_Sample3 :input").prop('disabled', false);
            $("#resultadofinal :input").prop('disabled', true);
            $("div[id^='Test_'] :input").prop('disabled', false);
            $("#NPHL_section :input").prop('disabled', false);
            
        }

    };      // END self.FlowDataCaseStatus

    self.FlowDataLab = function () {
        if ($("#ITy").val() == "2" && app.Views.Contact.Flow_Local_Institution_Lab() == true) {
            //console.log("aqui _ FlowDataLab _ institution lab");
            $("#tab-lab :input").prop('disabled', false);

            //if (app.Views.Lab.Processed() != "true" || app.Views.Lab.Processed_National() != "true")
            //{
            //    $("#addLabTest_1").hide();
            //}
            //else if (app.Views.Lab.Processed() == "true" || app.Views.Lab.Processed_National() == "true") {
            //    $("#addLabTest_1").show();
            //}

        }
        //else if (self.URmod_lab() == true) {
        //    console.log("aqui _ FlowDataLab _ rol mod_lab");
        //    $("#tab-lab :input").prop('disabled', false);
        //}
    };

    self.EnableTestBiologicalSUR = function () {
        //console.log("EnableTestBiologicalSUR");
        if ($("#ITy").val() == "2" && (self.UsrCountry() == 25 && app.Views.Lab.NPHL()) && self.CanConclude() == false) {
            // Laboratorio Nacional
            $("#RecDate_National").attr('disabled', true);
            $("#TempSample_National").attr('disabled', true);
            $('input[id="Processed_National"]').attr('disabled', true);
            $("#NoProRenId_National").attr('disabled', true);
            $("#NoProRen_National").attr('disabled', true);
            $("#Identification_Test_National").attr('disabled', true);

            //console.log("Si cumple condiciones")
            // Laboratorio Regional
            $("#RecDate").attr('disabled', false);
            $("#TempSample1").attr('disabled', false);
            $('input[id="Processed"]').attr('disabled', false);
            $("#NoProRenId").attr('disabled', false);
            $("#NoReason").attr('disabled', false);
            $("#Identification_Test").attr('disabled', false);
        }


    }

    self.EnableTestNationalGlobal = function () {
        //console.log("EnableTestNationalGlobal");
        //console.log(app.Views.Lab.NPHL_FlowExist());
        if ($("#ITy").val() == "2" && ( self.UsrCountry() == 15 || self.UsrCountry() == 9 || (self.UsrCountry() == 25 && app.Views.Lab.NPHL_FlowExist() ))  && app.Views.Lab.flow_max_record() == app.Views.Contact.flow_institution() && app.Views.Contact.Flow_Local_Institution_Lab()) {
            // Laboratorio Nacional
            $("#RecDate_National").attr('disabled', false);
            $("#TempSample_National").attr('disabled', false);
            $('input[id="Processed_National"]').attr('disabled', false);
            $("#NoProRenId_National").attr('disabled', false);
            $("#NoProRen_National").attr('disabled', false);
            $("#Identification_Test_National").attr('disabled', false);

            //console.log("Opcion verdadera");
            // Laboratorio Regional
            $("#RecDate").attr('disabled', true);
            $("#TempSample1").attr('disabled', true);
            $('input[id="Processed"]').attr('disabled', true);
            $("#NoProRenId").attr('disabled', true);
            $("#NoReason").attr('disabled', true);
            $("#Identification_Test").attr('disabled', true);

            }
        else {

            //console.log("Opcion falsa");
            // Laboratorio Nacional
            $("#RecDate_National").attr('disabled', true);
            $("#TempSample_National").attr('disabled', true);
            $('input[id="Processed_National"]').attr('disabled', true);
            $("#NoProRenId_National").attr('disabled', true);
            $("#NoProRen_National").attr('disabled', true);
            $("#Identification_Test_National").attr('disabled', true);
            // Laboratorio Regional
            $("#RecDate").attr('disabled', false);
            $("#TempSample1").attr('disabled', false);
            $('input[id="Processed"]').attr('disabled', false);
            $("#NoProRenId").attr('disabled', false);
            $("#NoReason").attr('disabled', false);
            $("#Identification_Test").attr('disabled', false);
        }

    }

    self.ViewTestCR = function () {

            //console.log("----   Lab NIC CR ----");
            //console.log(lab_NIC_usr);
            //console.log(app.Views.Contact.flow_institution());
            //console.log(app.Views.Lab.flow_max_record());
            //console.log(self.UsrCountry());
            //console.log("----   Lab NIC CR ----");
            if (self.UsrCountry() == 9) {
                if (app.Views.Lab.flow_max_record() > 1 ) {
                    $('div[id^="Test_1"]').show();
                    if ($("#ITy").val() == 2)
                    {
                        $("div[id^='Test_1'] :input").prop('disabled', false);
                    }
                    
                } else {
                    $('div[id^="Test_1"]').hide();
                    $("div[id^='Test_1'] :input").prop('disabled', true);
                }
            }
            


    }

    self.FlowDataLabAfter = function () {

        var flow_check = $.grep(app.Views.Lab.LabTests(), function (x) {         
            //console.log("Home - FlowDataLabAfter - EndFlow " + x.EndFlow());
            return x.EndFlow() === "TRUE";
        });
        //console.log(flow_check.length);


        if (($("#ITy").val() == "1" || $("#ITy").val() == "3") && flow_check.length > 0) {
            //console.log("DataLabAfter epi 1" + flow_check.length);
            //console.log("aqui _ FlowDataLabAfter _ 1");
            $("#tab-contact :input, #tab-GEO :input, #tab-hospital :input, #tab-risk :input, #tab-case :input").attr('disabled', false);
            self.ModDataNo();
            $("#saveGeneral_1").show();
            $("#saveGeneral_2").show();
        }
        else if ($("#ITy").val() == "2" && (app.Views.Contact.flow_record() > app.Views.Contact.flow_institution()) && self.OpenAlwaysLab() == false) {
            //console.log("aqui _ FlowDataLabAfter 3");
            //console.log("DataLabAfter epi 2" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        } else if ($("#ITy").val() == "2" && (app.Views.Contact.flow_record() == app.Views.Contact.flow_institution() && app.Views.Contact.DataStatement() == 2) && self.OpenAlwaysLab() == false) {
            //console.log("aqui _ FlowDataLabAfter 4");
            //console.log("DataLabAfter epi 3" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        } else if ($("#ITy").val() == "1" || $("#ITy").val() == "3") {
            //console.log("aqui _ FlowDataLabAfter 5");
            //console.log("DataLabAfter epi 4" + flow_check.length);
            $("#tab-lab :input").prop('disabled', true);
        }

        //console.log("aqui _ LabAfter");
        //console.log(app.Views.Contact.flow_record());
        if ($("#ITy").val() == "2" && (self.UsrCountry() == 15 || self.UsrCountry() == 9 || (self.UsrCountry() == 25))) {
            self.EnableTestNationalGlobal();
            //console.log(app.Views.Lab.NPHL());
            //if (app.Views.Lab.NPHL()) {
            //    self.EnableTestBiologicalSUR();
            //}
            
        }
        self.ViewTestCR();



        $("#PrintM1").prop('disabled', false);
        $("#PrintM2").prop('disabled', false);
        $("#PrintM3").prop('disabled', false);

    };


    self.ModDataNo = function () {
            $("#RegDate").attr('disabled', true);
            $("#FeverDate").attr('disabled', true);
            $("#HospitalDate").attr('disabled', true);
            $("#DocumentType").attr('disabled', true);
            $("#NoExpediente").attr('disabled', true);
            $("#Apellidos1").attr('disabled', true);
            $("#Nombres1").prop('disabled', true);
            $("#Apellidos1").attr('disabled', true);
            $("#DOB").prop('disabled', true);
            $("#Age").attr('disabled', true);
            $("#AMeasure").prop('disabled', true);
            $("input[id*='Sex']").prop('disabled', true);
            //$("input[id*='IsSample']").prop('disabled', true);
    };

    self.PDFViewer = function (id) {
        $.ajax({
            url: "serverSideFunctonWhichRetrivesPdfAsBase64",
            type: "post",
            data: { downloadHelperTransferData: transferData },
            success: function (result) {
                $("#object-pdf-shower").attr("data", result);
            }
        });
    };

    self.OpenAlwaysLab = function () {
        //console.log("Home OpenAlwaysLab - frecord_lab -- " + self.flow_record() + ", finstitution_lab -- " + self.flow_institution() + ", dataStatement_lab -- " + self.DataStatement() + ", userRole " + app.Views.Home.UserRole() + ", Inst" + $("#ITy").val() + ", OpenAlways" + self.flow_open_always());
        if (app.Views.Contact.flow_close_case() == 99 && app.Views.Contact.flow_open_always() == true) {
            return true;
        }
        else {
            return false;
        }
    };

    self.AddRecordHistoryItems = function (element, index, array) {
        //self.RecordHistoryItems.push(new RecordHistoryItems(element));
        self.RecordHistoryItems.push(element);

        //alert(JSON.stringify(element));
    };
   

    self.GetRecordHistory = function (id) {
        $.postJSON(app.dataModel.getRecordHistoryURL, {caseId: id})
            .success(function (data, textStatus, jqXHR) {
                self.RecordHistoryItems([]);
                //alert(JSON.stringify(data));
                data.forEach(self.AddRecordHistoryItems);
                //data.forEach(function (name) { alert(JSON.stringify(name)); });
                //console.log(data);
                //self.SummayItems([]);
                //data.forEach(self.AddSummayItem);                             

            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        
    }

    return self;
}

app.addViewModel({
    name: "Home",
    bindingMemberName: "home",
    factory: HomeViewModel
});




