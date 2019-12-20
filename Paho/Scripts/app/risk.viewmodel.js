function RiskViewModel(app, dataModel) {
    var self = this;
    var date_format_ = app.dataModel.date_format_;
    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_influenza_1 = new Date();
    var date_influenza_2 = new Date();
    var date_antiviral = new Date();

    self.Id = "";
    self.UsrCountry = ko.observable(app.Views.Home.UsrCountry()); // Pais del usuario logueado
    self.hasReset = ko.observable(false);
    self.Pregnant = ko.observable();
    self.Pperium = ko.observable();
    self.PregnantWeek = ko.observable("");
    self.Trimester = ko.observable();
    self.Vaccin = ko.observable(null);

    self.selectedOccupationId = ko.observable();                    //**** CAFQ
    self.TrabajoDirecc = ko.observable("");                         //**** CAFQ 
    self.TrabajoEstablec = ko.observable("");                       //**** CAFQ
    self.ContactoAnimVivos = ko.observable();                       //**** CAFQ
    self.OcupacMercAnimVivos = ko.observable();                     //**** CAFQ

    self.InfeccHospit = ko.observable("");                          //**** CAFQ     Radiobutton
    self.InfeccHospitFecha = ko.observable("");                     //**** CAFQ
    self.IsInfeHospPrev = ko.computed(function () {                 //**** CAFQ
        return (self.InfeccHospit() == "1") ? true : false;
    }, self);

    self.ViajePrevSintoma = ko.observable();              //#### CAFQ
    self.DestPrevSintoma1 = ko.observable();              //#### CAFQ
    self.DestFechaLlegada1 = ko.observable("");              //#### CAFQ
    self.DestFechaSalida1 = ko.observable("");              //#### CAFQ
    self.DestPrevSintoma2 = ko.observable();              //#### CAFQ
    self.DestFechaLlegada2 = ko.observable("");              //#### CAFQ
    self.DestFechaSalida2 = ko.observable("");               //#### CAFQ
    self.DestPrevSintoma3 = ko.observable();              //#### CAFQ
    self.DestFechaLlegada3 = ko.observable("");              //#### CAFQ
    self.DestFechaSalida3 = ko.observable("");               //#### CAFQ
    self.ContacDirectoAnim = ko.observable();        //##### CAFQ
    self.AnimalNaturaContac = ko.observable("");        //##### CAFQ
    self.ExpuextoSimilSintoma = ko.observable();        //##### CAFQ
    self.NumeIdentContacto = ko.observable("");        //##### CAFQ
    self.InfluConfirContacto = ko.observable();        //##### CAFQ
    self.TipoRelaContacto = ko.observable("");        //##### CAFQ
    self.FamiDirecContacto = ko.observable();        //##### CAFQ
    self.TrabSaludRama = ko.observable("");                                 //##### CAFQ
    self.TrabLaboratorio = ko.observable(false);                            //##### CAFQ
    self.TrabLaboratorioRama = ko.observable("");                           //##### CAFQ
    self.selectedTrabSaludRamaId = ko.observable();                         //**** CAFQ
    self.selectedTrabLaboratorioRamaId = ko.observable();                   //**** CAFQ

    self.IsViajePrevSintoma = ko.computed(function () {                     //##### CAFQ
        return (self.ViajePrevSintoma() == 1) ? true : false;
    }, self);
    self.IsContacDirectoAnim = ko.computed(function () {                    //**** CAFQ
        return (self.ContacDirectoAnim() == 1) ? true : false;
    }, self);
    self.IsExpuextoSimilSintoma = ko.computed(function () {                    //**** CAFQ
        return (self.ExpuextoSimilSintoma() == 1) ? true : false;
    }, self);

    self.RiskFactors = ko.observable("");
    self.Comorbidities = ko.observable("");
    self.VacInfluenza = ko.observable("");
    self.VacInfluenzaDateFirst = ko.observable(new Date());
    self.VacInfluenzaDateSecond = ko.observable(new Date());
    self.VacBcg = ko.observable(null);
    self.VacBcgDosis = ko.observable(null);
    self.VacBcgDate = ko.observable(new Date());
    self.VacNeumococo = ko.observable(null);
    self.VacNeumococoDosis = ko.observable(null);
    self.VacNeumococoDate = ko.observable(new Date());
    self.VacTosFerina = ko.observable(null);
    self.VacTosFerinaDosis = ko.observable(null);
    self.VacTosFerinaDate = ko.observable(new Date());
    self.VacHaemophilus = ko.observable(null);
    self.VacHaemophilusDate = ko.observable(new Date());
    self.VaccinFuente = ko.observable(null);
    self.AntiViral = ko.observable("");
    self.AntiViralDate = ko.observable(new Date());

    self.AntiViralDate.subscribe(function (newAntiviralDate) {
        var current_value = typeof (newAntiviralDate) == "object" ? newAntiviralDate : parseDate(newAntiviralDate, date_format_);
        var date_fever_ = typeof (app.Views.Hospital.FeverDate()) == "object" ? app.Views.Hospital.FeverDate() : parseDate(app.Views.Hospital.FeverDate(), date_format_);

        if (self.hasReset() != true) {
            if (date_fever_ != null || date_fever_ != "") {
                if (moment(current_value).isBefore(moment(date_fever_),"days")) {
                    alert(msgValidationAntiviralDateGtFeverDate);
                    self.FeverDate("");
                }
            }
        }

    });

    self.AntiViralDateEnd = ko.observable(new Date());
    self.AntiViralType = ko.observable(null);
    self.AntiViralTypeOther = ko.observable(null);
    self.AntiViralTypeOtherVisible = ko.computed(function () {
        var bResu = false;

        if (self.UsrCountry() == 25) {              // 25: Suriname
            if (self.AntiViralType() == 6)
                bResu = true;
        }
        if (bResu == false)
            self.AntiViralTypeOther(null);

        return bResu;
    }, self);
    self.AStartDate = ko.observable(new Date());
    self.OseltaDose = ko.observable("");
    self.AntiViralDose = ko.observable("");
    self.AntiViralDays = ko.observable();
    // Antibiotic
    self.Antibiotic = ko.observable("");
    self.AntibioticName = ko.observable("");
    
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7 ) ? true : false;

    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.PregnantWeek.subscribe(function (NewWeek) {
        if (NewWeek == null || NewWeek == "") return false;
        if (isNaN(parseFloat(NewWeek)) === true) {
            alert(msgValidationGestationalWeekNum);
            self.PregnantWeek("");
            $("#PregnantWeek").focus();
        } else {
            if (NewWeek < 40) {
                self.Trimester(Math.round(NewWeek/12));
            } else {
                alert(msgValidationGestationalWeekLt40);
                self.PregnantWeek("");
            }
        }
    });

    self.EnableVacInfluenzaDateFirst = ko.computed(function () {
        var result = self.VacInfluenza() == "1";
        if (!result) self.VacInfluenzaDateFirst("");
        return result;
    }, self);
    self.EnableVacInfluenzaDateSecond = ko.computed(function () {
        var result = self.VacInfluenza() == "1" && !(parseFloat(app.Views.Contact.Age()) >= 5 && app.Views.Contact.AMeasure() == 'Year');
        if (!result) self.VacInfluenzaDateSecond("");
        return result;
    }, self);
    self.EnableVacBcgDate = ko.computed(function () {
        var result = self.VacBcg() == "1";
        if (!result) {
            self.VacBcgDate("");
            self.VacBcgDosis("");
        }
        return result;
    }, self);
    self.EnableVacNeumococoDate = ko.computed(function () {
        var result = self.VacNeumococo() == "1";
        if (!result) {
            self.VacNeumococoDate("");
            self.VacNeumococoDosis("");
        }
        return result;
    }, self);
    self.EnableVacTosFerinaDate = ko.computed(function () {
        var result = self.VacTosFerina() == "1";
        if (!result) {
            self.VacTosFerinaDate("");
            self.VacTosFerinaDosis("");
            ////
        }
        return result;
    }, self);
    self.EnableVacHaemophilusDate = ko.computed(function () {
        var result = self.VacHaemophilus() == "1"; if (!result) self.VacHaemophilusDate(""); return result;
    }, self);
    self.EnableAntiViralDate = ko.computed(function () {
        var result = self.AntiViral() == "1"; if (!result) self.AntiViralDate(""); return result;
    }, self);
    self.EnableAntiViralDateEnd = ko.computed(function () {
        var result = self.AntiViral() == "1"; if (!result) self.AntiViralDateEnd(""); return result;
    }, self);
    self.EnableAntiViralType = ko.computed(function () {
        var result = self.AntiViral() == "1"; 
        if (!result) {
            self.AntiViralType(null);
            self.OseltaDose(null);
            self.AntiViralDose("");
        }
        return result;
    }, self);
    self.EnableOseltamivir = ko.computed(function () {
        var result = self.AntiViralType() == "1"; if (!result) self.OseltaDose(null); return result;
    }, self);
    self.EnableAntiviralDose = ko.computed(function () {
        var result = (self.AntiViralType() == "" || self.AntiViralType() == "5");
        if (!result) self.AntiViralDose("");
        return !result;
    }, self);

    ///*//####CAFQ
    self.EnableAntiViralDays = ko.computed(function () {
        var result = (self.AntiViralType() == "" || self.AntiViralType() == "5");
        if (!result) self.AntiViralDays("");
        return !result;
    }, self);
    //*/

    self.VisibleAntibioticName = ko.computed(function () {
        var result = (self.Antibiotic() == "1");
        if (result) self.AntibioticName("");
        return result;
    }, self);

    self.HDisease = ko.observable(false);
    self.SickleCellDisease = ko.observable(false);
    self.Diabetes = ko.observable(false);
    self.Neuro = ko.observable(false);
    self.Asthma = ko.observable(false);
    self.Pulmonary = ko.observable(false);
    self.Liver = ko.observable(false);
    self.Renal = ko.observable(false);
    self.Immunsupp = ko.observable(false);
    self.ParaCerebral = ko.observable(false);
    self.Indigena = ko.observable(false);
    self.TrabSalud = ko.observable(false);
    self.Desnutricion = ko.observable(false);
    self.Prematuridad = ko.observable(false);
    self.BajoPesoNacer = ko.observable(false);
    self.AusLacMat = ko.observable(false);
    self.Smoking = ko.observable(false);
    self.Alcohol = ko.observable(false);
    self.DownSyn = ko.observable(false);
    self.Obesity = ko.observable("");
    self.OtherComment = ko.observable("");    
    self.OtherRisk = ko.observable("");
    self.IsFemal = ko.computed(function () {
        return app.Views.Contact.Gender() === "Female" && app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() >= 10 && app.Views.Contact.Age() <= 50;
    });
    self.NotShowQuarter = ko.computed(function () {
        if (self.Pregnant() == "1")
        {
            if (self.UsrCountry() == 7) {
                $("#Primero, #Segundo, #Tercera").prop('disabled', true);
            } else {
                $("#Primero, #Segundo, #Tercera").prop('disabled', false);
            }
            
            return true;
        } else {
            self.Trimester("");
            return false;
        }      
    }, self);

    self.NotShowChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() < 3)) {
            return true;
        } else {
            self.Desnutricion(false);
            self.Prematuridad(false);
            self.BajoPesoNacer(false);
            self.AusLacMat(false);
            return false;
        }
    }, self);

    self.UnderTeenYears = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() < 10)) {
            return true;
        } else {
            self.Smoking(false);
            self.Alcohol(false);
            self.TrabSalud(false);
            //self.AusLacMat(false);
            return false;
        }
    }, self);


    self.InfVacTR = ko.computed(function () {
        if ((app.Views.Contact.AMeasure() == "Month" && app.Views.Contact.Age() < 6) || app.Views.Contact.AMeasure() == "Day" ) {
            return false;
            self.VacInfluenza("");
            self.VacInfluenzaDateFirst(null);
            self.VacInfluenzaDateSecond(null);
        } else {
            return true;
        }
    }, self);

    self.DosisAdult = ko.computed(function () {
        if ( app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() > 5) {
            return true;
        } else {
            self.VacInfluenzaDateFirst(null);
            return false;
        }
    }, self);

    self.DosisSecondChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() <= 5)) {
            return true;
        } else {
            self.VacInfluenzaDateSecond(null);
            return false;
        }
    }, self);

    self.DosisChilds = ko.computed(function () {
        if (app.Views.Contact.AMeasure() == "Month" || app.Views.Contact.AMeasure() == "Day" || (app.Views.Contact.AMeasure() == "Year" && app.Views.Contact.Age() <= 5)) {
            return true;
        } else {
            return false;
        }
    }, self);

    self.IsTrabSalud = ko.computed(function () {                 //**** CAFQ
        if (self.TrabSalud() == true) {
            return ($('#SurvInusual').is(':checked') == true) ? true : false;
        } else {
            return false;
        }
    }, self);

    self.IsTrabLaboratorio = ko.computed(function () {            //**** CAFQ
        if (self.TrabLaboratorio() == true) {
            return ($('#SurvInusual').is(':checked') == true) ? true : false;
        } else {
            return false;
        }
    }, self);

    self.IsInusitadoAdulto = ko.computed(function () {            //**** CAFQ
        if (app.Views.Contact.IsInusitado() == true) {
            if (app.Views.Contact.ShowOnlyAdult() == true) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }, self);


    self.RiskFactors.subscribe(function (NewRiskFactors) {
        if (NewRiskFactors == 0 || NewRiskFactors == "" || NewRiskFactors == 9 ) {
            //self.ResetRisk();
            //console.log(app.Views.Contact.ActiveBOL());
            if (app.Views.Contact.ActiveBOL()) {

                self.TrabSalud(null);
                self.Alcohol(null);
                self.Smoking(null);
                self.Indigena(null);
                self.DownSyn(null);
                $("#TrabSalud, #Alcohol, #Smoking, #Indigena, #DownSyn").prop('disabled', true);
            } else {
                self.Alcohol(null);
                self.Smoking(null);
                self.TrabSalud(null);
                self.Indigena(null);
                self.DownSyn(null);
                self.HDisease(null);
                self.SickleCellDisease(null);
                self.Diabetes(null);
                self.Neuro(null);
                self.Asthma(null);
                self.Pulmonary(null);
                self.Liver(null);
                self.Renal(null);
                self.Immunsupp(null);
                self.ParaCerebral(null);
                self.Desnutricion(null);
                self.Prematuridad(null);
                self.BajoPesoNacer(null);
                self.AusLacMat(null);
                self.Obesity(null);
                $("#HDisease, #SickleCellDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', true);
            }
            

        }
        else {
            if ($("#ITy").val() == 2) {
                $("#HDisease, #SickleCellDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', true);
            } else {
                //console.log(app.Views.Contact.ActiveBOL());
                if (app.Views.Contact.ActiveBOL()) {
                    $("#TrabSalud, #Alcohol, #Smoking, #Indigena, #DownSyn").prop('disabled', false);
                } else {
                    $("#HDisease, #SickleCellDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral, #Indigena, #TrabSalud, #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat, #DownSyn, #Alcohol, #Smoking, #combObesity").prop('disabled', false);
                }


                
            }
            
        }
    });

    self.Comorbidities.subscribe(function (NewComorbidities) {
        //console.log(NewComorbidities);
        self.HDisease(null);
        self.SickleCellDisease(null);
        self.Diabetes(null);
        self.Neuro(null);
        self.Asthma(null);
        self.Pulmonary(null);
        self.Liver(null);
        self.Renal(null);
        self.Immunsupp(null);
        self.ParaCerebral(null);
        self.Desnutricion(null);
        self.Prematuridad(null);
        self.BajoPesoNacer(null);
        self.AusLacMat(null);
        self.Obesity(null);
        if ((NewComorbidities == 0 || NewComorbidities == "" || NewComorbidities == 9) && app.Views.Contact.ActiveBOL()) {
           
            $("#HDisease, #SickleCellDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral,  #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat,  #combObesity").prop('disabled', true);
        } else if (app.Views.Contact.ActiveBOL()) {

            $("#HDisease, #SickleCellDisease, #Diabetes, #Neuro, #Asthma, #Pulmonary, #Liver, #Renal, #Immunsupp, #ParaCerebral,  #Desnutricion, #Prematuridad , #BajoPesoNacer, #AusLacMat,  #combObesity").prop('disabled', false);
        }
    });

    self.Pregnant.subscribe(function (NewPregnant) {
        //console.log(NewPregnant);
        if (NewPregnant != 1) {
            self.Pregnant(null);
            self.Pperium(null);
            self.Trimester(null);
            $("#Pperium, #Primero, #Segundo, #Tercera, #Desconocido").prop('disabled', true);
        }
        else {
            $("#Pperium, #Primero, #Segundo, #Tercera, #Desconocido").prop('disabled', false);
        }
    });
   
    self.ResetRisk = function () {
        self.hasReset(true);
        self.Id = "";
        self.Pregnant("");
        self.Pperium(null);
        self.Trimester(null);
        self.PregnantWeek(null);
        self.Vaccin(null);
        self.RiskFactors(null);
        self.Comorbidities(null);
        self.HDisease(false);
        self.SickleCellDisease(false);
        self.Diabetes(false);
        self.Neuro(false);
        self.Asthma(false);
        self.Pulmonary(false);
        self.Liver(false);
        self.Renal(false);
        self.Immunsupp(false);
        self.ParaCerebral (false);
        self.Indigena(false);
        self.TrabSalud(false);
        self.Desnutricion(false);
        self.Prematuridad(false);
        self.BajoPesoNacer(false);
        self.AusLacMat(false);
        self.Smoking(false);
        self.Alcohol(false);
        self.DownSyn(false);
        self.Obesity("");
        self.OtherRisk("");
        self.OtherComment("");
        self.AStartDate(null);
        self.VacInfluenza("");
        self.VacInfluenzaDateFirst(null);
        self.VacInfluenzaDateSecond(null);
        self.VacBcg(null);
        self.VacBcgDate(null);
        self.VacBcgDosis("");
        self.VacNeumococo(null);
        self.VacNeumococoDate(null);
        self.VacNeumococoDosis("");
        self.VacTosFerina(null);
        self.VacTosFerinaDate(null);
        self.VacTosFerinaDosis("");
        self.VacHaemophilus(null);
        self.VacHaemophilusDate(null);
        self.VaccinFuente("");
        self.AntiViral("");
        self.AntiViralDate(null);
        self.AntiViralDateEnd(null);
        self.AntiViralType("");
        self.AntiViralTypeOther(null);
        self.OseltaDose("");
        self.AntiViralDose("");
        self.AntiViralDays();

        self.Antibiotic("");
        self.AntibioticName("");
        self.ResetRiskInusual();                    //#### CAFQ
        self.hasReset(false);
    };

    self.ResetRiskInusual = function () {           //#### CAFQ
        //self.hasReset(true);
        self.selectedOccupationId("");              //#### CAFQ
        self.TrabajoDirecc("");                     //**** CAFQ 
        self.TrabajoEstablec("");                   //**** CAFQ
        self.ContactoAnimVivos("");                 //**** CAFQ
        self.OcupacMercAnimVivos("");               //**** CAFQ

        self.InfeccHospitFecha(null);                 //**** CAFQ
        self.InfeccHospit("");                      //**** CAFQ

        self.ViajePrevSintoma("");              //#### CAFQ
        self.DestPrevSintoma1("");              //#### CAFQ
        self.DestFechaLlegada1(null);              //#### CAFQ
        self.DestFechaSalida1(null);              //#### CAFQ
        self.DestPrevSintoma2("");              //#### CAFQ
        self.DestFechaLlegada2(null);              //#### CAFQ
        self.DestFechaSalida2(null);               //#### CAFQ
        self.DestPrevSintoma3("");              //#### CAFQ
        self.DestFechaLlegada3(null);              //#### CAFQ
        self.DestFechaSalida3(null);               //#### CAFQ

        self.ContacDirectoAnim("");        //##### CAFQ
        self.AnimalNaturaContac("");        //##### CAFQ
        self.ExpuextoSimilSintoma("");        //##### CAFQ
        self.NumeIdentContacto("");        //##### CAFQ
        self.InfluConfirContacto("");        //##### CAFQ
        self.TipoRelaContacto("");        //##### CAFQ
        self.FamiDirecContacto("");        //##### CAFQ
        self.TrabSaludRama("");                                 //##### CAFQ
        self.TrabLaboratorio("");                            //##### CAFQ
        self.TrabLaboratorioRama("");                           //##### CAFQ
        self.selectedTrabSaludRamaId("");                         //**** CAFQ
        self.selectedTrabLaboratorioRamaId("");                   //**** CAFQ
        //self.hasReset(false);
    }

    self.GetRisk = function (id) {
        self.Id = id;
        //console.log("Getting ...");
        //console.log(self.Id);
        $.getJSON(app.dataModel.getRiskUrl, { id: id }, function (data, status) {
            self.hasReset(true);
                self.RiskFactors(data.RiskFactors);
                self.Comorbidities(data.Comorbidities); //Unicamente para Bolivia
                self.Vaccin(data.Vaccin);
                self.HDisease(data.HDisease);
                self.SickleCellDisease(data.SickleCellDisease)
                self.Diabetes(data.Diabetes);
                self.Neuro(data.Neuro);
                self.Asthma(data.Asthma);
                self.Pulmonary(data.Pulmonary);
                self.Liver(data.Liver);
                self.Renal(data.Renal);
                self.Immunsupp(data.Immunsupp);

                self.ParaCerebral(data.ParaCerebral);
                self.Indigena(data.Indigena);
                self.TrabSalud(data.TrabSalud);
                self.Desnutricion(data.Desnutricion);
                self.Prematuridad(data.Prematuridad);
                self.BajoPesoNacer(data.BajoPesoNacer);
                self.AusLacMat(data.AusLacMat);

                self.Pregnant(data.Pregnant);
                self.Pperium(data.Pperium);
                self.Trimester(data.Trimester);
                self.PregnantWeek(data.PregnantWeek);
                self.Smoking(data.Smoking);
                self.Alcohol(data.Alcohol);
                self.DownSyn(data.DownSyn);
                self.Obesity(data.Obesity);
                self.OtherRisk(data.OtherRisk);
                self.OtherComment(data.OtherComment);

                if (data.VacInfluenza) self.VacInfluenza(data.VacInfluenza);
                if (data.VacInfluenzaDateFirst) self.VacInfluenzaDateFirst(moment(data.VacInfluenzaDateFirst).clone().toDate()); else self.VacInfluenzaDateFirst(null);
                if (data.VacInfluenzaDateSecond) self.VacInfluenzaDateSecond(moment(data.VacInfluenzaDateSecond).clone().toDate()); else self.VacInfluenzaDateSecond(null);
                
                self.VacBcg(data.VacBcg);
                self.VacBcgDosis(data.VacBcgDosis);
                if (data.VacBcgDate) self.VacBcgDate(moment(data.VacBcgDate).clone().toDate()); else self.VacBcgDate(null);
                self.VacNeumococo(data.VacNeumococo);
                self.VacNeumococoDosis(data.VacNeumococoDosis);
                if (data.VacNeumococoDate) self.VacNeumococoDate(moment(data.VacNeumococoDate).clone().toDate()); else self.VacNeumococoDate(null);
                self.VacTosFerina(data.VacTosFerina);
                self.VacTosFerinaDosis(data.VacTosFerinaDosis);
                if (data.VacTosFerinaDate) self.VacTosFerinaDate(moment(data.VacTosFerinaDate).clone().toDate()); else self.VacTosFerinaDate(null);
                self.VacHaemophilus(data.VacHaemophilus);
                if (data.VacHaemophilusDate) self.VacHaemophilusDate(moment(data.VacHaemophilusDate).clone().toDate()); else self.VacHaemophilusDate(null);
                self.VaccinFuente(data.VaccinFuente);
                self.AntiViral(data.AntiViral);
                if (data.AntiViralDate) self.AntiViralDate(moment(data.AntiViralDate).clone().toDate()); else self.AntiViralDate(null);
                if (data.AntiViralDateEnd) self.AntiViralDateEnd(moment(data.AntiViralDateEnd).clone().toDate()); else self.AntiViralDateEnd(null);
                
                self.AntiViralType(data.AntiViralType);
                self.AntiViralTypeOther(data.AntiViralTypeOther);
                self.OseltaDose(data.OseltaDose);
                self.AntiViralDose(data.AntiViralDose);
                self.AntiViralDays(data.AntiViralDays);

                self.Antibiotic(data.Antibiotic);
                self.AntibioticName(data.AntibioticName);

                self.selectedOccupationId(data.Ocupacion);                         //#### CAFQ
                self.TrabajoDirecc(data.TrabajoDirecc);                         //#### CAFQ
                self.TrabajoEstablec(data.TrabajoEstablec);                       //#### CAFQ
                self.ContactoAnimVivos(data.ContactoAnimVivos);                       //#### CAFQ
                self.OcupacMercAnimVivos(data.OcupacMercAnimVivos);                       //#### CAFQ
                
                self.InfeccHospit(data.InfeccHospit);                    //#### CAFQ
                if (data.InfeccHospitFecha)                    //#### CAFQ
                    self.InfeccHospitFecha(moment(data.InfeccHospitFecha).clone().toDate());
                else self.InfeccHospitFecha(null);

                self.ViajePrevSintoma(data.ViajePrevSintoma);           //#### CAFQ
                self.DestPrevSintoma1(data.DestPrevSintoma1);           //#### CAFQ
                self.DestPrevSintoma2(data.DestPrevSintoma2);           //#### CAFQ
                self.DestPrevSintoma3(data.DestPrevSintoma3);           //#### CAFQ
                if (data.DestFechaLlegada1) self.DestFechaLlegada1(moment(data.DestFechaLlegada1).clone().toDate()); else self.DestFechaLlegada1(null);           //#### CAFQ
                if (data.DestFechaLlegada2) self.DestFechaLlegada2(moment(data.DestFechaLlegada2).clone().toDate()); else self.DestFechaLlegada2(null);           //#### CAFQ
                if (data.DestFechaLlegada3) self.DestFechaLlegada3(moment(data.DestFechaLlegada3).clone().toDate()); else self.DestFechaLlegada3(null);           //#### CAFQ
                if (data.DestFechaSalida1) self.DestFechaSalida1(moment(data.DestFechaSalida1).clone().toDate()); else self.DestFechaSalida1(null);           //#### CAFQ
                if (data.DestFechaSalida2) self.DestFechaSalida2(moment(data.DestFechaSalida2).clone().toDate()); else self.DestFechaSalida2(null);           //#### CAFQ
                if (data.DestFechaSalida3) self.DestFechaSalida3(moment(data.DestFechaSalida3).clone().toDate()); else self.DestFechaSalida3(null);           //#### CAFQ
                self.ContacDirectoAnim(data.ContacDirectoAnim);           //#### CAFQ
                self.AnimalNaturaContac(data.AnimalNaturaContac);           //#### CAFQ
                self.ExpuextoSimilSintoma(data.ExpuextoSimilSintoma);           //#### CAFQ
                self.NumeIdentContacto(data.NumeIdentContacto);           //#### CAFQ
                self.InfluConfirContacto(data.InfluConfirContacto);           //#### CAFQ
                self.TipoRelaContacto(data.TipoRelaContacto);           //#### CAFQ
                self.FamiDirecContacto(data.FamiDirecContacto);           //#### CAFQ

                //self.TrabSaludRama(data.TrabSaludRama);                         //#### CAFQ
                self.selectedTrabSaludRamaId(data.TrabSaludRama);               //#### CAFQ
                self.TrabLaboratorio(data.TrabLaboratorio);                     //#### CAFQ
                //self.TrabLaboratorioRama(data.TrabLaboratorioRama);             //#### CAFQ
                self.selectedTrabLaboratorioRamaId(data.TrabLaboratorioRama);   //#### CAFQ

                self.hasReset(false);

            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            });
    };

    self.validate = function (nextStep) {
        var msg = "";
        //date_influenza_1 = parseDate($("#VacInfluenzaDateFirst").val(), date_format_);
        //date_influenza_2 = parseDate($("#VacInfluenzaDateSecond").val(), date_format_); 
        //date_antiviral = parseDate($("#AntiViralDate").val(), date_format_);
        //date_antiviral_end = parseDate($("#AntiViralDateEnd").val(), date_format_);
        //date_neumococo = parseDate($("#VacNeumococoDate").val(), date_format_);
        date_DOB_dummy = jQuery.type(app.Views.Contact.DOB_dummy()) === 'date' ? app.Views.Contact.DOB_dummy() : parseDate($("#DOB_dummy").val(), date_format_);
        date_fever_risk = jQuery.type(app.Views.Hospital.FeverDate()) === 'date' ? app.Views.Hospital.FeverDate() : parseDate($("#FeverDate").val(), date_format_);

        date_influenza_1 = jQuery.type(self.VacInfluenzaDateFirst()) === 'date' ? self.VacInfluenzaDateFirst() : parseDate($("#VacInfluenzaDateFirst").val(), date_format_);
        date_influenza_2 = jQuery.type(self.VacInfluenzaDateSecond()) === 'date' ? self.VacInfluenzaDateSecond() : parseDate($("#VacInfluenzaDateSecond").val(), date_format_);
        date_neumococo = jQuery.type(self.VacNeumococoDate()) === 'date' ? self.VacNeumococoDate() : parseDate($("#VacNeumococoDate").val(), date_format_);
        date_tosferina = jQuery.type(self.VacTosFerinaDate()) === 'date' ? self.VacTosFerinaDate() : parseDate($("#VacTosFerinaDate").val(), date_format_);
        date_haemophilus = jQuery.type(self.VacHaemophilusDate()) === 'date' ? self.VacHaemophilusDate() : parseDate($("#VacHaemophilusDate").val(), date_format_);

        date_antiviral = jQuery.type(self.AntiViralDate()) === 'date' ? self.AntiViralDate() : parseDate($("#AntiViralDate").val(), date_format_);
        date_antiviral_end = jQuery.type(self.AntiViralDateEnd()) === 'date' ? self.AntiViralDateEnd() : parseDate($("#AntiViralDateEnd").val(), date_format_);

        date_DestFechaLlegada1 = jQuery.type(self.DestFechaLlegada1()) === 'date' ? self.DestFechaLlegada1() : parseDate($("#DestFechaLlegada1").val(), date_format_);            //#### CAFQ
        date_DestFechaLlegada2 = jQuery.type(self.DestFechaLlegada2()) === 'date' ? self.DestFechaLlegada2() : parseDate($("#DestFechaLlegada2").val(), date_format_);            //#### CAFQ
        date_DestFechaLlegada3 = jQuery.type(self.DestFechaLlegada3()) === 'date' ? self.DestFechaLlegada3() : parseDate($("#DestFechaLlegada3").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida1 = jQuery.type(self.DestFechaSalida1()) === 'date' ? self.DestFechaSalida1() : parseDate($("#DestFechaSalida1").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida2 = jQuery.type(self.DestFechaSalida2()) === 'date' ? self.DestFechaSalida2() : parseDate($("#DestFechaSalida2").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida3 = jQuery.type(self.DestFechaSalida3()) === 'date' ? self.DestFechaSalida3() : parseDate($("#DestFechaSalida3").val(), date_format_);            //#### CAFQ

        date_InfeccHospitFecha = jQuery.type(self.InfeccHospitFecha()) === 'date' ? self.InfeccHospitFecha() : parseDate($("#InfeccHospitFecha").val(), date_format_);        //#### CAFQ
        
        //alert($("#AntiViralDate").val());
        //alert(date_antiviral);
        //alert(parseDate($("#AntiViralDate").val(), date_format_));

        if (self.IsFemal()) {
//            alert($("input[name='Pregnant']:checked").val());
            if ($("input[name='Pregnant']:checked").val() == "")
                msg += "\n" + "Variable embarazo es requerida";

            if (self.Pregnant() == "1" && !self.Trimester())
                msg += "\n" + "Variable trimestre de embarazo es requerida";
        }

        if ((!self.VaccinFuente() || self.VaccinFuente() == "0") && self.UsrCountry() != 25 && self.UsrCountry() != 18 && self.UsrCountry() != 11 && self.UsrCountry() != 17 && self.UsrCountry() != 11 && self.UsrCountry() != 119)
            if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
             msg += "\n" + "Fuente de la información de la vacuna es requerida";


        // Validacion de Fecha de vacunas contra fecha de nacimiento

        if (date_neumococo != null)
            if (moment(date_neumococo).isBefore(moment(date_DOB_dummy))) {
                msg += "\n" + "La fecha de la vacuna de neumococo no puede ser menor a fecha de nacimiento";
                $("#VacNeumococoDate").focus();
            }

        if ((app.Views.Contact.AMeasure() == "Month" && app.Views.Contact.Age() > 5) && app.Views.Contact.AMeasure() != "Day") {
            if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                { 
                    if (!self.VacInfluenza() && !self.UsrCountry() == 7 && !self.UsrCountry() != 17)
                        msg += "\n" + "Información de la vacuna de Influenza es requerida";

                    if (date_influenza_1 != null && self.VacInfluenza() == "1" && !self.UsrCountry() != 17)
                        if (moment(date_influenza_1).isBefore(moment(date_DOB_dummy))) {
                            msg += "\n" + "La fecha de la vacuna de Influenza no puede ser menor a fecha de nacimiento";
                            $("#VacInfluenzaDateFirst").focus();
                        }

                    if (date_influenza_1 != null && date_influenza_2 != null && !self.UsrCountry() != 17) {
                        if (moment(date_influenza_2).isBefore(moment(date_DOB_dummy))) {
                            msg += "\n" + "La fecha de la vacuna de segunda dosis de Influenza no puede ser menor a fecha de nacimiento";
                            $("#VacInfluenzaDateSecond").focus();
                        }
                        if (moment(date_influenza_2).isBefore(moment(date_influenza_1)) && !self.UsrCountry() != 17) {
                            msg += "\n" + "La fecha de la vacuna de segunda dosis de Influenza no puede ser menor a fecha de la primera dosis de Influenza";
                            $("#VacInfluenzaDateSecond").focus();
                        }
                    }
            }
        }

        //Las fechas de las vacunas fueron desactivadas por petición de CRI
        
        //if (self.EnableVacInfluenzaDateFirst()){
        //    if ($("#VacInfluenzaDateFirst").val() == "")
        //        msg += "\n" + "Fecha de primera dosis de influenza es requerida";
        //    if ($("#VacInfluenzaDateFirst").val() != "" && !moment(moment(date_influenza_1).format(date_format_moment), [date_format_moment], true).isValid())
        //        msg += "\n" + "Fecha de primera dosis de influenza es inválida";
        //}

        //if (self.EnableVacInfluenzaDateSecond()) {
        //    if ($("#VacInfluenzaDateSecond").val() == "")
        //        msg += "\n" + "Fecha de segunda dosis de influenza es requerida";
        //    if ($("#VacInfluenzaDateSecond").val() != "" && !moment(moment(date_influenza_2).format(date_format_moment), [date_format_moment], true).isValid())
        //        msg += "\n" + "Fecha de segunda dosis de influenza es inválida";
        //}

        //if (!self.VacBcg())
        //    msg += "\n" + "Información para BCG es requerida";

        //if (self.EnableVacBcgDate()) {
        //    if (!self.VacBcgDate())
        //        msg += "\n" + "Fecha de aplicación de BCG es requerida";
        //    if (self.VacBcgDate() && !moment(moment(self.VacBcgDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de BCG es inválida";
        //}

        //if (!self.VacNeumococo())
        //    msg += "\n" + "Información de aplicación de Neumococo es requerida";

        //if (self.EnableVacNeumococoDate()) {
        //    if (!self.VacNeumococoDate())
        //        msg += "\n" + "Fecha de aplicación de Neumococo es requerida";
        //    if (self.VacNeumococoDate() && !moment(moment(self.VacNeumococoDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Neumococo es inválida";
        //}
        
        //if (!self.VacTosFerina())
        //    msg += "\n" + "Información de aplicación de Tos Ferina es requerida";

        //if (self.EnableVacTosFerinaDate()) {
        //    if (!self.VacTosFerinaDate())
        //        msg += "\n" + "Fecha de aplicación de Tos Ferina es requerida";
        //    if (self.VacTosFerinaDate() && !moment(moment(self.VacTosFerinaDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Tos Ferina es inválida";
        //}

        //if (!self.VacHaemophilus())
        //    msg += "\n" + Información de aplicación de Haemophilus es requerida";

        //if (self.EnableVacHaemophilusDate()) {
        //    if (!self.VacHaemophilusDate())
        //        msg += "\n" + "Fecha de aplicación de Haemophilus es requerida";
        //    if (self.VacHaemophilusDate() && !moment(moment(self.VacHaemophilusDate()).format("DD/MM/YYYY"), ["DD/MM/YYYY"], true).isValid())
        //        msg += "\n" + "Fecha de aplicación de Haemophilus es inválida";
        //}

        if (!self.AntiViral())
            if (!(self.UsrCountry() == 17 && app.Views.Contact.SurvInusual() == 1))    //#### CAFQ: 180604 - Jamaica Universal
                if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                    msg += "\n" + msgValidationAntiviralTreatment;

        if (self.EnableAntiViralDate() && app.Views.Contact.SurvSARI() == true) {
            if (date_antiviral == null)
                //msg += "\n" + "Fecha de tratamiento antiviral es requerida";
                msg += "\n" + msgValidationAntiviralDateRequired;
            else if (date_antiviral != null && !moment(moment(date_antiviral).format(date_format_moment), [date_format_moment], true).isValid())
                //msg += "\n" + "Fecha de tratamiento antiviral es inválida";
                msg += "\n" + msgValidationAntiviralDateInvalid;
            else if (moment(date_antiviral).isBefore(date_fever_risk, "days"))
                msg += "\n" + msgValidationAntiviralDateGtFeverDate;

        }

        if (app.Views.Contact.ActiveBOL() && !self.AntiViralType() && self.AntiViral() == "1" ) {
            msg += "\n" + "Información de tipo de tratamiento antiviral es requerida";
        }

        if (!self.RiskFactors() || self.RiskFactors() == "")
            if (!(self.UsrCountry() == 17 && app.Views.Contact.SurvInusual() == 1))    //#### CAFQ: 180604 - Jamaica Universal
                if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                    msg += "\n" + msgValidationRiskFactor;

        if (app.Views.Contact.ActiveBOL()) {
            if (!(self.UsrCountry() == 3 && app.Views.Contact.SurvInusual() == 1))    //#### Bolivia inusitado
                if (!self.Comorbidities() || self.Comorbidities() == "")
                    msg += "\n" + msgValidationRiskFactor;
        }
        

        if (msg !== "") { alert(msgValidationMedicalHistory + msg); $('#tabs').tabs({ active: 2 }); return false; }
        if (nextStep != null) nextStep();
        return true;
    };

    self.Save = function () {
        app.Views.Home.ValidateAll();
    };

    self.Cancel = function () {
        if (confirm(viewConfirmExit)) {
            app.Views.Home.CancelEdit();
        }
    };

    self.SaveRisk = function (nextStep) {
        date_influenza_1 = jQuery.type(self.VacInfluenzaDateFirst()) === 'date' ? self.VacInfluenzaDateFirst() : parseDate($("#VacInfluenzaDateFirst").val(), date_format_);
        date_influenza_2 = jQuery.type(self.VacInfluenzaDateSecond()) === 'date' ? self.VacInfluenzaDateSecond() : parseDate($("#VacInfluenzaDateSecond").val(), date_format_);
        date_neumococo = jQuery.type(self.VacNeumococoDate()) === 'date' ? self.VacNeumococoDate() : parseDate($("#VacNeumococoDate").val(), date_format_);
        date_tosferina = jQuery.type(self.VacTosFerinaDate()) === 'date' ? self.VacTosFerinaDate() : parseDate($("#VacTosFerinaDate").val(), date_format_);
        date_haemophilus = jQuery.type(self.VacHaemophilusDate()) === 'date' ? self.VacHaemophilusDate() : parseDate($("#VacHaemophilusDate").val(), date_format_);
        
        date_antiviral = jQuery.type(self.AntiViralDate()) === 'date' ? self.AntiViralDate() : parseDate($("#AntiViralDate").val(), date_format_);
        date_antiviral_end = jQuery.type(self.AntiViralDateEnd()) === 'date' ? self.AntiViralDateEnd() : parseDate($("#AntiViralDateEnd").val(), date_format_);
        
        date_DestFechaLlegada1 = jQuery.type(self.DestFechaLlegada1()) === 'date' ? self.DestFechaLlegada1() : parseDate($("#DestFechaLlegada1").val(), date_format_);            //#### CAFQ
        date_DestFechaLlegada2 = jQuery.type(self.DestFechaLlegada2()) === 'date' ? self.DestFechaLlegada2() : parseDate($("#DestFechaLlegada2").val(), date_format_);            //#### CAFQ
        date_DestFechaLlegada3 = jQuery.type(self.DestFechaLlegada3()) === 'date' ? self.DestFechaLlegada3() : parseDate($("#DestFechaLlegada3").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida1 = jQuery.type(self.DestFechaSalida1()) === 'date' ? self.DestFechaSalida1() : parseDate($("#DestFechaSalida1").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida2 = jQuery.type(self.DestFechaSalida2()) === 'date' ? self.DestFechaSalida2() : parseDate($("#DestFechaSalida2").val(), date_format_);            //#### CAFQ
        date_DestFechaSalida3 = jQuery.type(self.DestFechaSalida3()) === 'date' ? self.DestFechaSalida3() : parseDate($("#DestFechaSalida3").val(), date_format_);            //#### CAFQ

        date_InfeccHospitFecha = jQuery.type(self.InfeccHospitFecha()) === 'date' ? self.InfeccHospitFecha() : parseDate($("#InfeccHospitFecha").val(), date_format_);        //#### CAFQ

         $.post(app.dataModel.saveRiskUrl,
            {
                Id: self.Id,
                Vaccin: self.Vaccin(),
                HDisease: self.HDisease() != true ? false : self.HDisease(),
                SickleCellDisease: self.SickleCellDisease() != true ? false : self.SickleCellDisease(),
                Diabetes: self.Diabetes() != true ? false : self.Diabetes(),
                Neuro: self.Neuro() != true ? false : self.Neuro(),
                Asthma: self.Asthma() != true ? false: self.Asthma(),
                Pulmonary: self.Pulmonary() != true ? false : self.Pulmonary(),
                Liver: self.Liver() != true ? false : self.Liver(),
                Renal: self.Renal() != true ? false : self.Renal(),
                Immunsupp: self.Immunsupp() != true ? false : self.Immunsupp(),
                Pregnant: self.Pregnant(),
                Pperium: self.Pperium(),

                ParaCerebral: self.ParaCerebral() != true ? false : self.ParaCerebral(),
                Indigena: self.Indigena() != true ? false : self.Indigena(),
                TrabSalud: self.TrabSalud() != true ? false : self.TrabSalud(),
                Desnutricion: self.Desnutricion() != true ? false : self.Desnutricion(),
                Prematuridad: self.Prematuridad() != true ? false : self.Prematuridad(),
                BajoPesoNacer: self.BajoPesoNacer() != true ? false : self.BajoPesoNacer(),
                AusLacMat: self.AusLacMat() != true ? false : self.AusLacMat(),
                
                Trimester: self.Trimester(),
                PregnantWeek: self.PregnantWeek(),
                Smoking: self.Smoking() != true ? false : self.Smoking(),
                Alcohol: self.Alcohol() != true ? false : self.Alcohol(),
                DownSyn: self.DownSyn() != true ? false : self.DownSyn(),
                Obesity: self.Obesity(),
                OtherRisk: self.OtherRisk() == null ? "" : self.OtherRisk().toLocaleUpperCase(),
                OtherComment: self.OtherComment() == null ? "" : self.OtherComment().toLocaleUpperCase(),
                VacInfluenza: self.VacInfluenza(),
                VacInfluenzaDateFirst: $("#VacInfluenzaDateFirst").val() == "" ? null : moment(date_influenza_1).format(date_format_ISO),
                VacInfluenzaDateSecond: $("#VacInfluenzaDateSecond").val() == "" ? null : moment(date_influenza_2).format(date_format_ISO),
                VacBcg: self.VacBcg(),
                VacBcgDate: $("#VacBcgDate").val() == "" ? null : moment(self.VacBcgDate()).format(date_format_ISO),
                VacBcgDosis: self.VacBcgDosis(),
                VacNeumococo: self.VacNeumococo(),
                VacNeumococoDate: $("#VacNeumococoDate").val() == "" ? null : moment(date_neumococo).format(date_format_ISO),
                VacNeumococoDosis: self.VacNeumococoDosis,
                VacTosFerina: self.VacTosFerina(),
                VacTosFerinaDate: $("#VacTosFerinaDate").val() == "" ? null : moment(date_tosferina).format(date_format_ISO),
                VacTosFerinaDosis: self.VacTosFerinaDosis,
                VacHaemophilus: self.VacHaemophilus(),
                VacHaemophilusDate: $("#VacHaemophilusDate").val() == "" ? null : moment(date_haemophilus).format(date_format_ISO),
                VaccinFuente: self.VaccinFuente(),
                AntiViral: self.AntiViral(),
                AntiViralDate: $("#AntiViralDate").val() == "" ? null : moment(date_antiviral).format(date_format_ISO),
                AntiViralDateEnd: $("#AntiViralDateEnd").val() == "" ? null : moment(date_antiviral_end).format(date_format_ISO),
                AntiViralType: self.AntiViralType(),
                AntiViralTypeOther: self.AntiViralTypeOther() == null ? null : self.AntiViralTypeOther().toLocaleUpperCase(),
                OseltaDose: self.OseltaDose(),
                AntiViralDose: self.AntiViralDose(),
                AntiViralDays: self.AntiViralDays(),
                // Antibiotic
                Antibiotic: self.Antibiotic(),
                AntibioticName: self.AntibioticName(),

                RiskFactors: self.RiskFactors(),
                Comorbidities: self.Comorbidities(),

                Ocupacion: self.selectedOccupationId(),                                //#### CAFQ 
                TrabajoDirecc: self.TrabajoDirecc(),                                //#### CAFQ 
                TrabajoEstablec: self.TrabajoEstablec(),                                //#### CAFQ
                ContactoAnimVivos: self.ContactoAnimVivos(),                        //#### CAFQ
                OcupacMercAnimVivos: self.OcupacMercAnimVivos(),                     //#### CAFQ

                InfeccHospit: self.InfeccHospit(),                  //#### CAFQ
                InfeccHospitFecha: $("#InfeccHospitFecha").val() == "" ? null : moment(date_InfeccHospitFecha).format(date_format_ISO),    //#### CAFQ

                ViajePrevSintoma: self.ViajePrevSintoma(),          //#### CAFQ
                DestPrevSintoma1: self.DestPrevSintoma1(),          //#### CAFQ
                DestPrevSintoma2: self.DestPrevSintoma2(),          //#### CAFQ
                DestPrevSintoma3: self.DestPrevSintoma3(),          //#### CAFQ
                DestFechaLlegada1: $("#DestFechaLlegada1").val() == "" ? null : moment(date_DestFechaLlegada1).format(date_format_ISO),          //#### CAFQ
                DestFechaLlegada2: $("#DestFechaLlegada2").val() == "" ? null : moment(date_DestFechaLlegada2).format(date_format_ISO),          //#### CAFQ
                DestFechaLlegada3: $("#DestFechaLlegada3").val() == "" ? null : moment(date_DestFechaLlegada3).format(date_format_ISO),          //#### CAFQ
                DestFechaSalida1: $("#DestFechaSalida1").val() == "" ? null : moment(date_DestFechaSalida1).format(date_format_ISO),          //#### CAFQ
                DestFechaSalida2: $("#DestFechaSalida2").val() == "" ? null : moment(date_DestFechaSalida2).format(date_format_ISO),          //#### CAFQ
                DestFechaSalida3: $("#DestFechaSalida3").val() == "" ? null : moment(date_DestFechaSalida3).format(date_format_ISO),          //#### CAFQ
                ContacDirectoAnim: self.ContacDirectoAnim(),          //#### CAFQ
                AnimalNaturaContac: self.AnimalNaturaContac(),          //#### CAFQ
                ExpuextoSimilSintoma: self.ExpuextoSimilSintoma(),          //#### CAFQ
                NumeIdentContacto: self.NumeIdentContacto(),          //#### CAFQ
                InfluConfirContacto: self.InfluConfirContacto(),          //#### CAFQ
                TipoRelaContacto: self.TipoRelaContacto(),          //#### CAFQ
                FamiDirecContacto: self.FamiDirecContacto(),          //#### CAFQ
                /*TrabSaludRama: self.TrabSaludRama(),          //#### CAFQ
                TrabLaboratorio: self.TrabLaboratorio(),          //#### CAFQ
                TrabLaboratorioRama: self.TrabLaboratorioRama()          //#### CAFQ*/
                TrabSaludRama: self.selectedTrabSaludRamaId(),                          //#### CAFQ
                TrabLaboratorio: self.TrabLaboratorio(),                                //#### CAFQ
                TrabLaboratorioRama: self.selectedTrabLaboratorioRamaId()               //#### CAFQ
            },
            function (data) {
                if (nextStep) nextStep();
                //alert(data);
            },
            "json"
         );
        return true;
    };

    return self;

};

app.addViewModel({
    name: "Risk",
    bindingMemberName: "risk",
    factory: RiskViewModel
});