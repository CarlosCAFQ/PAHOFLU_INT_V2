function InstitutionsFlow() {
    var self = this;
    self.LabID = "";
    self.LabName = ko.observable("");
    self.OPbyL = "";
};

function ContactViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_hospital = new Date();
    var date_reg_date = new Date();
    var date_DOB = new Date();
    //var date_Server = new Date();

    // autocomplete
    self.firstName = ko.observable();
    self.people = ko.observableArray();
    self.hasGet = ko.observable(false);
    self.hasHospitalID = ko.observable("");
    self.IntsFlow = ko.observableArray();

    self.FeverDateDummy = ko.observable("");

    self.Id = ko.observable("");
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado

    // Campos para flujo
    self.DataStatement = ko.observable($("#o_S").val()); // Estado del documento 1=Ingresando datos sin enviar, 2=Enviado al siguiente estado del flujo
    self.flow_record = ko.observable(0);
    self.flow_institution = ko.observable(0);
    self.flow_max = ko.observable(1);
    self.flow_close_case = ko.observable(99);  // Este valor siempre sera 99 cuando un caso este cerrado
    self.flow_open_always = ko.observable(false); // Esta variable es solo para los laboratorios que estan marcados en la tabla de configuracion de flujo

    self.SavePrev_var = ko.observable(true);
    self.IsSurv = ko.observable("");                                //#### CAFQ
    self.SurvInusual = ko.observable(false);                        //#### CAFQ
    self.SurvInusual.subscribe(function (inusualOK) {               //#### CAFQ
        if (!inusualOK) {
            app.Views.Risk.ResetRiskInusual();
            app.Views.Hospital.ResetHospitalInusual();
            self.hospitalIDCaseGenerating("");
        }
    });
    self.servicesArr = ko.observableArray(institutions);
    self.selectedServiceId = ko.observable("");
    self.hospitalIDCaseGenerating = ko.observable("");
    self.Brote = ko.observable(false);
    self.region_institucional = ko.observable("");
    self.region_salud = ko.observable("");
    self.region_pais = ko.observable("");
    self.hospitalName = ko.observable("");
    self.VisHopName = ko.computed(function () {
        if (self.hospitalName != "") {
            return true;
        } else {
            return false;
        }
    }, self);
    self.LName1 = ko.observable("");
    self.LName2 = ko.observable("");
    self.VisLName2Mand = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;

    }, self);
    self.FName1 = ko.observable("");
    self.FName2 = ko.observable("");
    self.DocumentType = ko.observable("");
    self.NoExpediente = ko.observable("");
    self.NationalId = ko.observable("");
    self.DOB = ko.observable(new Date());
    self.DOB_dummy = ko.observable(new Date());
    self.Age = ko.observable("");
    self.AMeasure = ko.observable("");
    self.AMeasure.subscribe(function(newmeasure){
        //self.CalculateDOB();
    });
    self.ResponsibleMinor = ko.observable("");      //####CAFQ 

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);
    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);
    self.ActiveDOM= ko.computed(function () {
        return (self.UsrCountry() == 11) ? true : false;
    }, self);
    self.NoActiveDOM = ko.computed(function () {
        return (self.UsrCountry() == 11) ? false : true;
    }, self);
    self.ActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? true : false;
    }, self);
    self.NoActiveCAY = ko.computed(function () {
        return (self.UsrCountry() == 119) ? false : true;
    }, self);
    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);       //#### CAFQ
    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);       //#### CAFQ
    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);
    self.NoActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? false : true;
    }, self);

    self.VisibleIdType = ko.computed(function () {
        return (self.UsrCountry() != 15) ? true : false;
    }, self);

    self.selectedNationalityID = ko.observable(app.Views.Home.UsrCountry());
    self.selectedNativepeopleID = ko.observable("");

    self.ShowOnlyAdult = ko.computed(function () {
            return (self.AMeasure() == "Month" || self.AMeasure() == "Day" || (self.AMeasure() == "Year" && self.Age() < 15)) ? false : true;
    }, self);

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

    // Validacion del RUT para chile
    self.esrut = function (rut) {
        //if (rut == 0) {
        //    alert("Debe ingresar un RUT");
        //    document.form1.Tx_RUN2.focus();//me posiciono en el campo nuevamente
        //    return false;
        //} else {
        if (rut.search("-") <= 0) {
            alert(msgValidationRUN + rut + msgValidationRUNDashedPart2);
            self.NoExpediente("");//limpio el valor del campo rut
            $("#NoExpediente").focus();//me posiciono en el campo nuevamente
            return false;
        }
            if (self.EsRut(rut) == false) {
                alert(msgValidationRUN + rut + msgValidationRUNValidPart2);
                self.NoExpediente("");//limpio el valor del campo rut
                $("#NoExpediente").focus();//me posiciono en el campo nuevamente
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

    self.displayService = ko.computed(function () {
        //console.log(self.servicesArr().length)
        return (self.servicesArr().length > 1) ? true : false;
    }, self);

    self.DocumentType.subscribe(function (NewRUN) {
        if (self.UsrCountry() != 25 && self.UsrCountry() != 11 && self.UsrCountry() != 18 && self.UsrCountry() != 17) {//Solo ejecuta este subscribe si el país es diferente de Suriname
            self.NoExpediente("");
            self.FName1("");
            self.FName2("");
            self.LName1("");
            self.LName2("");
        }
    });

    self.NoExpediente.subscribe(function (NewRUN) {

        if (self.UsrCountry() == 9 && NewRUN != "") {
            var string_length = NewRUN.length;
            //console.log(isNaN(self.NoExpediente()));
            //console.log(self.NoExpediente());
            //console.log(/^[1-9]\d$/.test(self.NoExpediente()));
            //|| /^[0-9]\d$/.test(self.NoExpediente()) == false
            if (self.DocumentType() == 1 && (string_length != 9 || isNaN(self.NoExpediente()) == true )) {
                alert(msgValidationDocumentFormat);
                self.NoExpediente("");
            } else if (self.DocumentType() == 2 && string_length != 10) {
                alert(msgValidationInsuredNumber);
                self.NoExpediente("");
            } else if (self.DocumentType() == 3 && string_length != 14) {
                alert(msgValidationInternalDoc);
                self.NoExpediente("");
            }

        }

        if (self.UsrCountry() == 7 && NewRUN != "" && self.DocumentType() == 4 && NewRUN != "9.999.999" && $("#ITy").val() != "2") {
            self.esrut(NewRUN);
        }

    });

    //self.NoExpediente.subscribe(function (NewDT) {

    //    if (self.UsrCountry() == 7 && self.NoExpediente() != "" && NewDT == 4 && self.NoExpediente() != "9.999.999" && $("#ITy").val() != "2") {
    //        self.esrut(NewRUN);
    //    }

    //});

    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
        
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.EnableCR = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;

    }, self);

    self.Age.subscribe(function (newage) {
        //if (self.AMeasure() != "" || self.AMeasure() != "Year") self.CalculateDOB();
        if (self.AMeasure() == "") self.AMeasure("Year");
        if (newage != "" && self.AMeasure() != "" && (self.DOB() == "" || self.DOB() == null)) $("#DOB").prop("disabled", true);


        if (app.Views.Home.AlertDefinitionBegin() >= 0 && app.Views.Home.AlertDefinitionEnd() >= 0) {
            if (app.Views.Contact.AMeasure() == "") app.Views.Contact.AMeasure("Year");
            var measure = app.Views.Contact.AMeasure() == "Year" ? "years" : (app.Views.Contact.AMeasure() == "Month" ? 'months' : 'days');
            var ConvertToMonth = ((measure == 'years') ? app.Views.Contact.Age() * 12 : (measure == 'days') ? app.Views.Contact.Age() / 12 : app.Views.Contact.Age());

            if (ConvertToMonth <= app.Views.Home.AlertDefinitionBegin() || ConvertToMonth >= app.Views.Home.AlertDefinitionEnd()) {
                $("#casedefinitionwarning").hide();
            }


        }

        // Esto es para el mensaje de alerta de condición de 

    });

    self.AMeasure.subscribe(function (newAMeasure) {
        //if (self.AMeasure() != "" || self.AMeasure() != "Year") self.CalculateDOB();
        if (newAMeasure != "" && self.Age() != "" && (self.DOB() == "" || self.DOB() == null)) $("#DOB").prop("disabled", true);
    });
    self.CalculateDOB = function () {
        if (self.AMeasure() == "") self.AMeasure("Year");
        var measure = self.AMeasure() == "Year" ? "years" : (self.AMeasure() == "Month" ? 'months' : 'days');
        var dob = new Date(moment().add(self.Age() * -1, measure));
            //.format(date_format_moment);
        if ((self.Age() && self.Age() != "") && (!self.DOB() || self.DOB() == "" || $("#DOB").prop("disabled"))) {
            $("#DOB").prop("disabled", true);
            self.DOB_dummy(dob);
        }
    };
    self.DOB.subscribe(function (nedob) {
        if ($("#DOB").prop("disabled")) return;
        if (self.hasGet() == true) return;
        if (self.DOB() && self.DOB() != "") {
            /*console.log(self.DOB());
            console.log(moment().diff(self.DOB(), 'years'));*/
            var cHospitalDate = self.HospitalDate().toISOString().substring(0, 10);

            //if (moment().diff(self.DOB(), 'years') > 0) {
            if (moment(cHospitalDate, "YYYY-MM-DD").diff(self.DOB(), 'years') > 0) {
                //self.Age(moment().diff(self.DOB(), 'years'));
                self.Age(moment(cHospitalDate, "YYYY-MM-DD").diff(self.DOB(), 'years'));
                self.AMeasure("Year");
            //} else if (moment().diff(self.DOB(), 'months') > 0) {   
            } else if (moment(cHospitalDate, "YYYY-MM-DD").diff(self.DOB(), 'months') > 0) {
                //self.Age(moment().diff(self.DOB(), 'months'));
                self.Age(moment(cHospitalDate, "YYYY-MM-DD").diff(self.DOB(), 'months'));
                self.AMeasure("Month");
            } else {
                //self.Age(moment().diff(self.DOB(), 'days'));
                self.Age(moment(cHospitalDate, "YYYY-MM-DD").diff(self.DOB(), 'days'));
                self.AMeasure("Day");
            }

            $("#Age").prop("disabled", true);
            $("#AMeasure").prop("disabled", true);
            self.DOB_dummy(self.DOB);
        } else {
            self.Age("");
            self.AMeasure("");
            $("#Age").prop("disabled", false);
            $("#AMeasure").prop("disabled", false);
        }
    });
    //*//####CAFQ
    self.IsMenorEdad = ko.computed(function () {
        var rm = self.ResponsibleMinor();

        if (self.AMeasure() == "Year" && self.Age() < 18) {
            return true;
        } else {
            self.ResponsibleMinor("");
            return false;
        }
    }, self);
    //*/
    self.Gender = ko.observable("");
    self.HospitalDate = ko.observable(new Date());
    self.RegDate = ko.observable(new Date());
    self.FullName = ko.computed(function () {
        return self.FName1() + " " + (self.FName2() || "") + " " + self.LName1() + " " + (self.LName2() || "");
    });
    self.AgeGroup = ko.computed(function () {      
        if (!(self.Age() || self.DOB())) return "";
        if (self.UsrCountry() == 25 || self.UsrCountry() == 11 || self.UsrCountry() == 18) {
            if (self.AMeasure() === "Day" || (self.AMeasure() === "Month" && self.Age() <= 6)) return "child under 6 months"
            if (self.AMeasure() === "Month" && self.Age() >= 6 && self.Age() <= 12) return "6 to 11 months";
            if ((self.AMeasure() === "Month" && self.Age() >= 12 && self.Age() <= 24) || (self.AMeasure() === "Year" && self.Age() == 1)) return "12 to 23 months";
            if (self.Age() < 5) return "2 to 4 years";
            if (self.Age() < 15) return "5 to 14 years";
            if (self.Age() < 50) return "15 to 49 years";
            if (self.Age() < 65) return "50 to 64 years";
            return "65 years and over";
        } else if (self.UsrCountry() == 17 || self.UsrCountry() == 11 || self.UsrCountry() == 119) {
            if (self.AMeasure() === "Day" || (self.AMeasure() === "Month" && self.Age() <= 6)) return "child under 6 months"
            if (self.AMeasure() === "Month" && self.Age() >= 6 && self.Age() <= 12) return "6 to 11 months";
            if ((self.AMeasure() === "Month" && self.Age() >= 12 && self.Age() <= 24) || (self.AMeasure() === "Year" && self.Age() == 1)) return "12 to 23 months";
            if (self.Age() < 5) return "2 to 4 years";
            if (self.Age() < 15) return "5 to 14 years";
            if (self.Age() < 50) return "15 to 49 years";
            if (self.Age() < 60) return "50 to 59 years";
            if (self.Age() < 65) return "60 to 64 years";
            return "65 years and over";
        } else if (self.UsrCountry() == 15) {
            if (self.AMeasure() == "Day" || (self.AMeasure() == "Month" && self.Age() <= 12)) return "Niños menores de 1 año"
            if (self.Age() < 1) return "Niños menores de 1 año";
            if (self.Age() < 5) return "1 a 4 años";
            if (self.Age() < 15) return "5 a 14 años";
            if (self.Age() < 50) return "15 a 49 años";
            if (self.Age() < 60) return "50 a 59 años";
            return "60 años y más";
        } 

        else if (self.UsrCountry() != 7) {
            if (self.AMeasure() == "Day" || (self.AMeasure() == "Month" && self.Age() <= 12)) return "Niños menores de 2 años"
            if (self.Age() < 2) return "Niños menores de 2 años";
            if (self.Age() < 5) return "2 a 4 años";
            if (self.Age() < 15) return "5 a 14 años";
            if (self.Age() < 34) return "15 a 34 años";
            if (self.Age() < 65) return "35 a 64 años";
            return "65 años y más";
        } 
        else
        {
            if (self.AMeasure() == "Day" || (self.AMeasure() == "Month" && self.Age() <= 12)) return "Niños menores de 2 años"
            if (self.Age() < 2) return "Niños menores de 2 años";
            if (self.Age() < 5) return "2 a 4 años";
            if (self.Age() < 20) return "5 a 19 años";
            if (self.Age() < 40) return "20 a 39 años";
            if (self.Age() < 60) return "40 a 59 años";
            return "60 años y más";
        }
        
    });

    self.NotShowHospitalization = ko.computed(function () {
        if (self.IsSurv() == "1" || (self.SurvInusual() == true && self.UsrCountry() == 17) || (self.SurvInusual() == true && self.UsrCountry() == 3)) // Universal Jamaica & Inusitado Bolivia
        {
            return true;
        } else {
            return false;
        }      
    }, self);

    self.SurvSARI = ko.computed(function () {
        if (self.IsSurv() != "2") {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILI = ko.computed(function () {
        if (self.IsSurv() == "2") {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvSARICHI = ko.computed(function () {
        if (self.IsSurv() != "2" && self.UsrCountry() == 7) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvSARIHON = ko.computed(function () {
        if (self.IsSurv() != "2" && self.UsrCountry() == 15) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.NacionalidadCHI = ko.computed(function () {
        if (self.selectedNationalityID() == "7") {
            return true;
        } else {
            self.selectedNativepeopleID("");
            return false;
        }
    }, self);
    self.NacionalidadHON = ko.computed(function () {
        if (self.selectedNationalityID() == "15") {
            return true;
        } else {
            self.selectedNativepeopleID("");
            return false;
        }
    }, self);

    self.SurvSARICR = ko.computed(function () {
        if (self.IsSurv() != "2" && self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILI = ko.computed(function () {
        if (self.IsSurv() == "2") {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILICR = ko.computed(function () {
        if (self.IsSurv() == "2" && self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.SurvILICH = ko.computed(function () {
        if (self.IsSurv() == "2" && self.UsrCountry() == 7) {
            return true;
        } else {
            return false;
        }
    }, self);
 
    self.IsInusitado = ko.computed(function () {            //**** CAFQ
        if (self.SurvInusual() == true) {
            // Desactivado por el momento requirimiento de RRR //16/Marzo/2018
            //return true;
            return true;
        } else {
            return false; 
        }
    }, self);

    self.displayhospitalIDCaseGenerating = ko.computed(function () {
        //console.log(self.servicesArr().length)
        return (self.IsInusitado()) ? true : false;
    }, self);

    self.AddFluCase = function () {
        app.Views.Home.ResetFluCase();
    };

    self.Borrar = function () {
        if (confirm(msgConfirmationDeleteRecord)) {
            $.getJSON(app.dataModel.deletetUrl, { id: self.Id() }, function(data, status) {
                    //console.log(data);
                    app.Views.Home.CancelEdit();
                })
                .fail(function(jqXHR, textStatus, errorThrown) {
                    //alert(errorThrown);
                    console.log(errorThrown);
                });
        }
    };

    self.ResetContact = function () {

        //self.Id = ko.observable("");
        if ($.isFunction(self.Id)) {
            self.Id = ko.observable("");
            self.Id("");
        }
        else {
            self.Id = "";
        }
        self.Id(null);
        if ($('input[name="IsSurv"][value="1"]').length > 0 && $('input[name="IsSurv"][value="2"]').length == 0) {
            self.IsSurv("1");
        } else if ($('input[name="IsSurv"][value="2"]').length > 0 && $('input[name="IsSurv"][value="1"]').length == 0) {
            self.IsSurv("2");
        } else if ($('input[name="IsSurv"][value="2"]').length == 0 && $('input[name="IsSurv"][value="1"]').length == 0) {
            self.IsSurv("");
            self.SurvInusual(true);
        } else {
            self.IsSurv("");
        }
 
        //self.SurvInusual(false);
        self.Brote(false);
        self.hospitalName("");
        self.hospitalIDCaseGenerating("");
        self.LName1("");
        self.LName2("");
        self.FName1("");
        self.FName2("");
        self.DocumentType("");
        self.NoExpediente("");
        self.NationalId("");
        self.DOB(null);
        self.DOB_dummy(null);
        self.Age("");
        self.AMeasure("");
        self.Gender("");
        self.ResponsibleMinor("");
        self.HospitalDate(null);
        self.RegDate(new Date());
        if (self.UsrCountry() == 15) {
            self.selectedNationalityID(app.Views.Home.UsrCountry());
        } else {
            self.selectedNationalityID("");
        }

        self.selectedNativepeopleID("");
        self.hasHospitalID("");
        // esto es para resetear el flujo
        self.flow_institution(0);
        self.flow_record(0);
        self.DataStatement(1);
        // Regiones
        self.region_institucional(app.Views.Home.UsrRegInst());
        self.region_salud(app.Views.Home.UsrRegSalud());
        self.region_pais(app.Views.Home.UsrRegPais());
    };

    self.FormatDateWrite = function (node_date) {
        //var self_date = node_date
        //console.log(self_date);
        //console.log(node_date);

    };

    self.EnableSavePrev = ko.computed(function () {

        if (self.UsrCountry() == 7) {

            if (self.IsSurv() && self.DocumentType() && self.NoExpediente() != "" && self.LName1() != "" && self.FName1() != "" && self.DOB() != "" && self.Gender()
            && app.Views.GEO.selectedCountryId() != "" && app.Views.GEO.selectedAreaId() != "" && app.Views.GEO.selectedStateId() != "" && app.Views.GEO.selectedNeighborhoodId() != ""
            && app.Views.Risk.VaccinFuente() && app.Views.Risk.VacInfluenza() && app.Views.Risk.AntiViral() && app.Views.Risk.RiskFactors()
            && app.Views.Hospital.FeverDate() != "" && app.Views.Hospital.AntecedentesFiebre() && app.Views.Hospital.IsSample()) {
                self.SavePrev_var(false);
                if ($("#ITy").val() == "2") {
                    //$("#tab-lab :input").prop('disabled', false);
                }
                return false
            } else {
                if (self.IsSurv() && self.DocumentType() && self.NoExpediente() != "" && self.LName1() != "" && self.FName1() != "" && self.DOB() != "" && self.Gender() && self.HospitalDate() != ""
                    && app.Views.GEO.selectedCountryId() != "" && app.Views.Hospital.FeverDate() != "" && app.Views.Hospital.IsSample() && $("#ITy").val() == "2") {
                    //$("#tab-lab :input").prop('disabled', false);
                } else {
                    //$("#tab-lab :input").prop('disabled', true);
                }
                self.SavePrev_var(true);
                
                return true;
            }
        } else {
            self.SavePrev_var(false);
            return false;
        }
        
    }, self);

    self.OnlyCR = ko.computed(function () {
        if (self.UsrCountry() == 9) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.validate = function (nextStep) {
        var msg = "";

        //date_hospital = parseDate($("#HospitalDate").val(), date_format_);
        //date_reg_date = parseDate($("#RegDate").val(), date_format_);
        //date_inicio_sintomas = parseDate($("#FeverDate").val(), date_format_);


        date_hospital = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_);
        date_reg_date = jQuery.type(self.RegDate()) === 'date' ? self.RegDate() : parseDate($("#RegDate").val(), date_format_);
        date_inicio_sintomas = jQuery.type(app.Views.Hospital.FeverDate()) === 'date' ? app.Views.Hospital.FeverDate() : parseDate($("#FeverDate").val(), date_format_);
        date_DOB = jQuery.type(self.DOB()) === 'date' ? self.DOB() : parseDate(self.DOB(), date_format_);

        if (date_inicio_sintomas != null)
            if (moment(date_hospital).isBefore(moment(date_inicio_sintomas))) {
                msg += "\n" + msgValidationOnsetFeverDate;
                $("#HospitalDate").focus();
            }
                

        if (self.IsSurv() == "")
        {
            if (self.UsrCountry() != 17 && self.IsInusitado() != true)
                if (self.UsrCountry() != 3 && self.IsInusitado() != true)  // Inusitado Bolivia
                    msg += "\n" + msgValidationSurvType;

        }
            
        if (!self.DocumentType() && (self.UsrCountry() != 25 && self.UsrCountry() != 11 && self.UsrCountry() != 18 && self.UsrCountry() != 17 && self.UsrCountry() != 15))//agregado el 25 para que esta validación ignore SURINAME
            if (!(self.UsrCountry() == 3 && self.SurvInusual() == 1))    //####  Bolivia Inusistado
                msg += "\n" + msgValidationTypeOfDocument; // Este no esta en la base

        if (!self.NoExpediente()) {
            if (!(self.UsrCountry() == 17 && self.SurvInusual() == 1) )    //#### CAFQ: 180604 - Jamaica Universal
            {  
               if (!(self.UsrCountry() == 3 && self.SurvInusual() == 1) )    //####  Bolivia Inusistado
                {             
                if (self.UsrCountry() != 15)
                    msg += "\n" + msgValidationDocumentIDRequired;
                }
            }
                
        }
            

        if ($("#HospitalDate").val() == "")
            msg += "\n" + msgValidationNotificationDateRequired;
        if ($("#HospitalDate").val() != "" && !moment(moment(date_hospital).format(date_format_moment), [date_format_moment], true).isValid())
            msg += "\n" + msgValidationNotificationDate;
        //if ($("#RegDate").val() == "")
        //    msg += "\n" + "Fecha de notificación es requerido";
        if ($("#RegDate").val() != "" && !moment(moment(date_reg_date).format(date_format_moment), [date_format_moment], true).isValid())
            msg += "\n" + msgValidationRecordDate;
        if (date_hospital != null && date_reg_date != null && moment(date_hospital).isAfter(moment(date_reg_date)))
        {
            msg += "\n" + msgValidationRecordDateLater;
            $("#HospitalDate").focus();
        }
            
        if (!self.LName1())
            msg += "\n" + msgValidationFirstLastNameRequired;
        if (!self.LName2() && self.UsrCountry == 9 )
            msg += "\n" + msgValidationSecondLastNameRequired;
        if (!self.FName1())
            msg += "\n" + msgValidationFirstNameRequired;

        // validacion del combo de servicios
        
        if ($("#cmbServices option").length > 1) {
            //console.log(self.selectedServiceId());
            if (self.selectedServiceId() == 0 )
            {
                msg += "\n" + msgValidationService;
            }
        }
            


//        if (!self.DOB())
//            msg += "\n" + "Date of birth is invalid";
//        if (self.DOB() && !moment(moment(self.DOB()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
//            msg += "\n" + "Date of birth is invalid";
//        if (self.HospitalDate() && self.DOB() && moment(self.HospitalDate()).isBefore(moment(self.DOB())))
//            msg += "\n" + "Date of birth is posterior to the Capture Date";
//        if (self.RegDate() && self.DOB() && moment(self.RegDate()).isBefore(moment(self.DOB())))
//            msg += "\n" + "Fecha de nacimiento no puede ser posterior a la de notificación ";
        if (!self.Gender())
            //if (!(self.UsrCountry() == 17 && self.SurvInusual() == 1))    //#### CAFQ: 180604 - Jamaica Universal
            msg += "\n" + msgValidationSexRequired;

        // Validaciones Chile

        if (self.UsrCountry() == 7) {
            if (date_DOB == null || date_DOB == "")
                msg += "\n" + msgValidationTypeDOB;
            if (moment(date_DOB).isAfter(moment(date_hospital)))
                msg += "\n" + msgValidationDOBLater;
        }

        if ((date_DOB == null || date_DOB == "") && (self.Age() == ""))
            if (!(self.UsrCountry() == 17 && self.SurvInusual() == 1))    //#### CAFQ: 180604 - Jamaica Universal
                if (!(self.UsrCountry() == 3 && self.SurvInusual() == 1))    //#### Bolivia Inusitado
                    msg += "\n" + msgValidationTypeDOBorAge;
                
        //if (msg !== "") { alert('DAT. VIGILANCIA:' + msg); $('#tabs').tabs({ active: 0 }); return false; }
        if (msg !== "") { alert(msgValidationSurvData + msg); $('#tabs').tabs({ active: 0 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.Cancel = function () {
        if (confirm(msgConfirmationExitRecord)) {
            app.Views.Home.CancelEdit();
        }
    };

    self.GetPatient = function () {

        if ((self.DocumentType() == "" && self.UsrCountry() != 25 && self.UsrCountry() != 11 && self.UsrCountry() != 18 && self.UsrCountry() != 17) || self.NoExpediente() == "")
        {
            alert(msgConfirmationDocumentTypeNumberRequired);
        } else {
            $.getJSON(app.dataModel.getPatientInformationUrl, { DTP: self.DocumentType(), DNP: self.NoExpediente() }, function (data, status) {
                if (data.length <= 0) {
                    alert(msgValidationPacientInfoMissing);
                }
                else {
                    self.FName1(data[0].nombre1);
                    self.FName2(data[0].nombre2);
                    self.LName1(data[0].apellido1);
                    self.LName2(data[0].apellido2);
                    var DOB_dumm = new Date(data[0].DOB);
                    DOB_dumm.setHours(DOB_dumm.getHours() + 8);
                    self.DOB(DOB_dumm);
                    //console.log(moment.utc(moment(data[0].DOB.replace(/"/g, '').replace(/[*?^${}|[\]\\]/g, '').replace('-0600', '')).format(date_format_moment)).toDate())  // seconds
                    self.Gender(data[0].sexo);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(msgValidationServerError);
                //console.log(errorThrown);
            });
        }
            
        
    };

    self.GetContact = function (id) {
        self.Id(id);
        self.hasGet(true);
        $("#RecordNumber").text(id);
        
        $.getJSON(app.dataModel.getContactUrl, { id: id }, function (data, status) {
                self.IsSurv(data.Surv);
                self.SurvInusual(data.SurvInusual);
                self.Brote(data.Brote);
                self.hospitalName(data.hospitalName);
                self.region_institucional(data.region_institucional);
                self.region_salud(data.region_salud);
                self.region_pais(data.region_salud);
                self.DocumentType(data.DocumentType);
                self.selectedServiceId(data.selectedServiceId);
                self.LName1(data.LName1);
                self.LName2(data.LName2);
                self.FName1(data.FName1);
                self.FName2(data.FName2);
                self.NoExpediente(data.NoExpediente);
                self.NationalId(data.NationalId);
                if (data.DOB) {
                    self.DOB(moment(data.DOB).clone().toDate());
                    self.DOB_dummy(moment(data.DOB).clone().toDate());
                } else {
                    self.DOB(null);
                    self.CalculateDOB();
                }
                self.Age(data.Age);
                self.AMeasure(data.AMeasure);
                if (data.Gender)
                    self.Gender(data.Gender);
                self.ResponsibleMinor(data.ResponsibleMinor);
                self.HospitalDate(moment(data.HospitalDate).clone().toDate());
                self.RegDate(moment(data.RegDate).clone().toDate());
                self.selectedNationalityID(data.nationality);
                self.selectedNativepeopleID(data.nativepeople);
                self.hasHospitalID(data.hospitalIDRecord);
                self.hasHospitalID() > 0 ? app.Views.Home.selectedInstitutionId(self.hasHospitalID()) : "";
                self.hospitalIDCaseGenerating(data.hospitalIDCaseGenerating);
                self.DataStatement(data.DataStatement);
                self.flow_record(data.flow_record);
                self.flow_institution(data.flow_institution);
                self.flow_max(data.flow_max);
                self.flow_open_always(data.flow_open_always);
                self.IntsFlow([]);
                if (data.IntsFlow != "") {
                    //console.log("Array de Instituciones ----- " + data.IntsFlow.length);
                    //console.log(data.IntsFlow);
                    for (index = 0; index < data.IntsFlow.length; ++index) {
                        var instflow = new InstitutionsFlow();
                        instflow.LabID = data.IntsFlow[index].Id;
                        instflow.LabName(data.IntsFlow[index].Name);
                        instflow.OPbyL = data.IntsFlow[index].orden;
                        self.IntsFlow.push(instflow);
                    }
                    //console.log("Array de Instituciones local de contact ----- " );
                    //console.log(self.IntsFlow());
                }

                //self.LabsFlow(data.CLOrdDisplay);  AM el array de instituciones
                $("#o_S").val(data.DataStatement);
                
                $("button[id^='Siguiente']").attr("disabled", false);
                $("button[id^='Atras']").attr("disabled", false);
                

            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                //alert(errorThrown);
                console.log(errorThrown);
            });
        //console.log("Array contact -- LabsFlow")
        //console.log(self.LabsFlow);
        self.hasGet(false);
    };

    self.Save = function (option_Save, object_Save) {
        if (option_Save == 2) {
            if (confirm(msgValidationSaveAndSend))
            {
                app.Views.Home.ValidateAll(option_Save);
            }
            else {
                alert(msgValidationDontSaveAndSend);
            }
        }
        else {
            app.Views.Home.ValidateAll(option_Save);
        }
        
    };

    self.SavePrev = function () {
        app.Views.Contact.validate(function () {
            app.Views.Home.SaveAll();
        });
    };

    self.SaveContact = function (nextStep) {
        date_hospital = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_);
        date_reg_date = jQuery.type(self.RegDate()) === 'date' ? self.RegDate() : parseDate($("#RegDate").val(), date_format_);
        date_DOB = jQuery.type(self.DOB()) === 'date' ? self.DOB() : parseDate($("#DOB").val(), date_format_);
        //#### CAFQ: 180604 - Jamaica Universal
        //date_fever_dummy = jQuery.type(app.Views.Hospital.FeverDate()) === 'date' ? app.Views.Hospital.FeverDate() : parseDate($("#FeverDate").val(), date_format_);
        //console.log("x1->" + self.ResponsibleMinor());
        if (self.UsrCountry() == 17 && self.SurvInusual() == 1) {
            date_fever_dummy = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_);
        } else if (self.UsrCountry() == 3 && self.SurvInusual() == 1) {  //  Inusitado Bolivia
            date_fever_dummy = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_);
        } else {
            date_fever_dummy = jQuery.type(app.Views.Hospital.FeverDate()) === 'date' ? app.Views.Hospital.FeverDate() : parseDate($("#FeverDate").val(), date_format_);
        }
        if (self.DocumentType() == "" && (self.UsrCountry() == 25 || self.UsrCountry() == 11 || self.UsrCountry() == 18 || self.UsrCountry() == 17)) {//sirve para que en el caso de Suriname, el DocumentType se llene con un valor predeterminado            
            self.DocumentType(8);
        }
         $.post(app.dataModel.saveContactUrl,
            {
                id: self.Id(),
                Surv: self.IsSurv(),
                SInusual: self.SurvInusual(),
                Brote: self.Brote(),
                LName1: self.LName1() == null ? "" : self.LName1().toLocaleUpperCase(),
                LName2: self.LName2() == null ? "" : self.LName2().toLocaleUpperCase(),
                FName1: self.FName1() == null ? "" : self.FName1().toLocaleUpperCase(),
                FName2: self.FName2() == null ? "" : self.FName2().toLocaleUpperCase(),
                DocumentType: self.DocumentType(),
                NationalId: self.NationalId() == null ? "" : self.NationalId().toLocaleUpperCase(),
                DOB: moment(date_DOB).format(date_format_ISO),
                Age: self.Age(),
                AMeasure: self.AMeasure(),
                Gender: self.Gender(),
                ResponsibleMinor: self.ResponsibleMinor(),
                HospitalDate: moment(date_hospital).format(date_format_ISO),
                RegDate: moment(date_reg_date).format(date_format_ISO),
                //HospitalId: self.hasHospitalID() > 0 ? self.hasHospitalID() : self.selectedServiceId() > 0 ? self.selectedServiceId() : app.Views.Home.selectedInstitutionId(),
                HospitalId: self.selectedServiceId() > 0 ? self.selectedServiceId() : app.Views.Home.selectedInstitutionId(),
                HospitalID_CaseGenerating : self.hospitalIDCaseGenerating,
                nativepeople: self.selectedNativepeopleID(),
                nationality: self.selectedNationalityID(),
                NoExpediente: self.NoExpediente() == null ? "" : self.NoExpediente().toLocaleUpperCase(),
                DateFeverDummy: moment(date_fever_dummy).format(date_format_ISO),
                IsSample: app.Views.Hospital.IsSample() === "true" ? true : (app.Views.Hospital.IsSample() === "false" ? false : null),
            },
            function (data) {
                if (typeof data != "number") {
                    alert(data);
                    self.displayFilters(true);
                    self.editMode(false);
                    self.showGrid(true);
                    self.ReloadFluCases();
                } else {
                    app.Views.Contact.Id(data);
                    app.Views.GEO.Id = data;
                    app.Views.Hospital.Id = data;
                    app.Views.Risk.Id = data;
                    app.Views.Lab.Id = data;
                    if (nextStep) nextStep();
                }
                
            },
            "json"
         );
        return true;
    };
    // Revision del flujo si es epidemiologico o laboratorio
    self.Flow_Local_Institution_Lab = function () {
        if ($("#ITy").val() == "2" && app.Views.Home.URmod_lab() == true) {
            return true;
        } else if ((self.flow_close_case() == 99 && self.flow_open_always() == true)) {
            return true;
        } else if ((self.flow_record() == (self.flow_institution() - 1) && (self.DataStatement() == 2 || self.DataStatement() == null || self.flow_open_always() == true) && app.Views.Home.SaveAndAdd_1() == true))
        {
            return true;
        } else if ((self.flow_record() == self.flow_institution() && (self.DataStatement() == 1 || self.DataStatement() == null || self.flow_open_always() == true)))
        {
            return true;
        }
        else {
            console.log("Flow Lab - false");
            return false;
        }

    }


    self.Flow_Local_Institution_Lab_send = function () {

        if ($("#ITy").val() != "2" && app.Views.Home.URmod_lab() == true) {
            return false;
        } else if ((self.flow_record() == (self.flow_institution() - 1) && (self.DataStatement() == 2 || self.DataStatement() == null || self.flow_open_always() == true) && app.Views.Home.SaveAndAdd_1() == true) || (self.flow_record() == self.flow_institution() && (self.DataStatement() == 1 || self.DataStatement() == null || self.flow_open_always() == true)) || (self.flow_close_case() == 99 && self.flow_open_always() == true)) {
            return true;
        }
        else {
            return false;
        }

    };

    self.Flow_Local_Institution_Epi = function () {

        //console.log("Contact epi - frecord_epi -- " + self.flow_record() + ", finstitution_epi -- " + self.flow_institution() + ", dataStatement_epi -- " + self.DataStatement() + ", userRole " + app.Views.Home.UserRole() + ", Inst" + $("#ITy").val());

        if ($("#ITy").val() == "2") {
            return false;
            //} else if ($("#ITy").val() != "2" && app.Views.Home.UserRole() == "adm") {
        } else if ($("#ITy").val() != "2" && app.Views.Home.URadm() == true) {
            return true;
            //} else if ($("#ITy").val() != "2"  && app.Views.Home.UserRole() == "stf") {
        } else if ($("#ITy").val() != "2" && app.Views.Home.URstf() == true) {
            
            return true;
            //} else if ($("#ITy").val() != "2" && app.Views.Home.UserRole() == "mod_epi") {
        } else if ($("#ITy").val() != "2" && app.Views.Home.URmod_epi() == true) {
            return true;
            //} else if ($("#ITy").val() != "2" && app.Views.Home.UserRole() == "clo_case" && app.Views.Hospital.CaseStatus() == "") {
        } else if ($("#ITy").val() != "2" && app.Views.Home.URclo_case() == true && app.Views.Hospital.CaseStatus() == "") {
            console.log("Flow_Local_Institution_Epi _ URclo_case");
            return true;
        } 
        //else if ((self.flow_record() == 0 && self.DataStatement() == 1) ||  (self.flow_record() == self.flow_institution() && self.DataStatement() == 2) || (self.Id() == "") || (self.flow_record() == self.flow_max() && (self.DataStatement() == 2 || self.DataStatement() == null))) {  // Case Status ==3 cuando esta cerrado  -- Modificacion requerida por RRR 20170924 no desactivar en caso cerrado
        else if ((self.flow_record() == 0 && self.DataStatement() == 1) ||  (self.flow_record() == self.flow_institution() && self.DataStatement() == 2) || (self.Id() == "") || (self.flow_record() == self.flow_max() && (self.DataStatement() == 2 || self.DataStatement() == null))) {  
            return true;
        }
        else {
            return false;
        }

    };

    self.addPerson = function () {
        self.people.push({ firstName: vm.firstName() });
        self.firstName('');
    }

    return self;

};

app.addViewModel({
    name: "Contact",
    bindingMemberName: "contact",
    factory: ContactViewModel
});