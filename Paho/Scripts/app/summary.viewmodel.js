
function SummaryYearItem(data) {
    var self = this;
    //****
    self.ColETINumST = data.ColETINumST;                    // ETI: Bandeja
    self.ColETIDenoST = data.ColETIDenoST;
    self.ColETINumEmerST = data.ColETINumEmerST;

    self.ColHospTST = data.ColHospTST;                      // SARI: Bandeja
    self.ColUCITST = data.ColUCITST;
    self.ColFalleTST = data.ColFalleTST;
    self.ColHospSARITST = data.ColHospSARITST;   
    self.ColUCISARITST = data.ColUCISARITST;       
    self.ColFalleSARITST = data.ColFalleSARITST;    
    self.ColNeuTST = data.ColNeuTST;
    self.ColCCSARITST = data.ColCCSARITST;
    self.ColVentTST = data.ColVentTST;

    self.ColILICasesST = data.ColILICasesST;                // ILI Jamaica: Bandeja
    self.ColILISamplesTakenST = data.ColILISamplesTakenST;
    self.ColTotalVisitsST = data.ColTotalVisitsST;

    self.ColILICaseGroup = data.ColILICaseGroup;            // ILI Jamaica: Bandeja: Totales establecimeintos x Pais/Area/Region
    self.ColILISampGroup = data.ColILISampGroup;
    self.ColTotVisiGroup = data.ColTotVisiGroup;

    self.ColEpiYear = data.EpiYear;
    self.ColEpiWeek = data.EpiWeek;
    self.ColEpiYearWeek = data.EpiYear + "-" + data.EpiWeek;
    self.StartDateOfWeek = data.StartDateOfWeek;
    self.WeekendDate = data.WeekendDate

    self.UsrCountry = ko.observable(selcty);                // Pais del usuario logueado
    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;
    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
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

    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);

    self.ActiveSLU = ko.computed(function () {
        return (self.UsrCountry() == 18) ? true : false;
    }, self);
    //****
    self.selectedHospitalId = ko.observable();      //####AAAA

    self.UCI_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === $("#combHospital1").val()) {         // Codigo de Hospital
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("OTHERS") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    self.Hospitalizacion_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === $("#combHospital1").val()) {         // Codigo de Hospital
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("- ICU") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    //****
}

function SummayItem(data) {
    var self = this;

    //console.log(data);
    self.Id = data.Id;                                      // Id del denominador
    self.UsrCountry = ko.observable(selcty);
    self.CaseSummaryId = data.CaseSummaryId;
    self.AgeGroup = data.AgeGroup;
    //self.selectedHospitalId()

    if (self.UsrCountry() != 17)                             // 17: Jamaica #### CAFQ: 190227
        self.Surv = ko.observable(SurvGlobal);

    self.AgeGroupDescription = CatAgeGroup[parseInt(self.AgeGroup) - 1].AgeGroup;
    self.AgeGroupDesc = ko.observable(self.AgeGroupDescription);
    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;
    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
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

    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);

    self.ActiveSLU = ko.computed(function () {
        return (self.UsrCountry() == 18) ? true : false;
    }, self);
    //****
    self.UCI_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === $("#combHospital1").val()) {         // Codigo de Hospital
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("OTHERS") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    self.Hospitalizacion_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === $("#combHospital1").val()) {         // Codigo de Hospital
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("- ICU") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    //****
    self.ILI_OK = ko.computed(function () {
        bResu = true;
        //----
        if (self.ActiveHON() || self.ActiveCOS())
            if (self.Surv() == "2")
                bResu = true;
            else
                bResu = false;
        //----
        return bResu;
    }, self);

    self.SARI_OK = ko.computed(function () {
        bResu = true;
        //----
        if (self.ActiveHON() || self.ActiveCOS())
            if (self.Surv() == "1")
                bResu = true;
            else
                bResu = false;
        //----
        return bResu
    }, self);

    //------------------------------------------------------
    self.HOS_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.HOS_IRAG_ShowCol_F = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('F');
    }, self);

    self.HOS_IRAG_ShowCol_M = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('M');
    }, self);

    self.HOS_IRAG_ShowCol_T = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------
    self.UCI_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.UCI_IRAG_ShowCol_F = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('F');
    }, self);

    self.UCI_IRAG_ShowCol_M = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('M');
    }, self);

    self.UCI_IRAG_ShowCol_T = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------
    self.DEF_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.DEF_IRAG_ShowCol_F = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('F');
    }, self);

    self.DEF_IRAG_ShowCol_M = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('M');
    }, self);

    self.DEF_IRAG_ShowCol_T = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------

    //****
    self.ETIDenoFem = ko.observable(data.ETIDenoFem);
    self.ETIDenoMaso = ko.observable(data.ETIDenoMaso);
    self.ETIDenoST = ko.observable(data.ETIDenoST);
    self.ETIDenoST_HN = ko.observable(data.ETIDenoST);
    self.ETIDenoST_HN_Tmp = ko.observable();

    self.ETIDenoST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.ETIDenoFem());
        var mas = parseInt(self.ETIDenoMaso());
        var tot = parseInt(self.ETIDenoST_HN());

        (fem == 0 && mas == 0) ? self.ETIDenoST_HN(self.ETIDenoST_HN()) : self.ETIDenoST_HN(fem + mas)

        return 0;
    }, self);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.ETIDenoST = ko.computed(function () {
            return parseInt(self.ETIDenoFem()) + parseInt(self.ETIDenoMaso());
        }, self);
    } else {
        if (!self.ActiveHON()) {
            self.ETIDenoST = ko.observable(data.ETIDenoST);
        }
    }
    //****
    self.ETINumFem = ko.observable(data.ETINumFem);
    self.ETINumMaso = ko.observable(data.ETINumMaso);
    self.ETINumST = ko.observable(data.ETINumST);
    self.ETINumST_HN = ko.observable(data.ETINumST);
    self.ETINumST_HN_Tmp = ko.observable();

    self.ETINumST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.ETINumFem());
        var mas = parseInt(self.ETINumMaso());
        var tot = parseInt(self.ETINumST_HN());

        (fem == 0 && mas == 0) ? self.ETINumST_HN(self.ETINumST_HN()) : self.ETINumST_HN(fem + mas)

        return 0;
    }, self);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.ETINumST = ko.computed(function () {
            return parseInt(self.ETINumFem()) + parseInt(self.ETINumMaso());
        }, self);

    } else {
        if (!self.ActiveHON()) {
            self.ETINumST = ko.observable(data.ETINumST);
        }
    }
    //****
    self.ETINumEmerFem = ko.observable(data.ETINumEmerFem);
    self.ETINumEmerMaso = ko.observable(data.ETINumEmerMaso);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.ETINumEmerST = ko.computed(function () {
            return parseInt(self.ETINumEmerFem()) + parseInt(self.ETINumEmerMaso());
        }, self);

    } else {
        self.ETINumEmerST = ko.observable(data.ETINumEmerST);
    }
    //****
    self.HospFem = ko.observable(data.HospFem);
    self.HospMaso = ko.observable(data.HospMaso);
    self.HospST = ko.observable(data.HospST);
    self.HospST_HN = ko.observable(data.HospST);
    self.HospST_HN_Tmp = ko.observable();

    self.HospST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.HospFem());
        var mas = parseInt(self.HospMaso());
        var tot = parseInt(self.HospST_HN());

        (fem == 0 && mas == 0) ? self.HospST_HN(self.HospST_HN()) : self.HospST_HN(fem + mas)

        return 0;
    }, self);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.HospST = ko.computed(function () {
            return parseInt(self.HospFem()) + parseInt(self.HospMaso());
        }, self);

    } else {
        if (!self.ActiveHON()) {
            self.HospST = ko.observable(data.HospST);
        }
    }

    //****
    self.HospSARIFem = ko.observable(data.HospSARIFem);
    self.HospSARIMaso = ko.observable(data.HospSARIMaso);
    //self.HospSARIST = ko.observable(data.HospSARIST);
    self.HospSARIST_HN = ko.observable(data.HospSARIST);
    self.HospSARIST_HN_Tmp = ko.observable();

    self.HospSARIST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.HospSARIFem());
        var mas = parseInt(self.HospSARIMaso());
        var tot = parseInt(self.HospSARIST_HN());

        (fem == 0 && mas == 0) ? self.HospSARIST_HN(self.HospSARIST_HN()) : self.HospSARIST_HN(fem + mas)
        return 0;
    }, self);

    //****
    self.UCIFem = ko.observable(data.UCIFem);
    self.UCIMaso = ko.observable(data.UCIMaso);
    self.UCIST = ko.observable(data.UCIST);
    self.UCIST_HN = ko.observable(data.UCIST);
    self.UCIST_HN_Tmp = ko.observable();

    self.UCIST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.UCIFem());
        var mas = parseInt(self.UCIMaso());
        var tot = parseInt(self.UCIST_HN());

        (fem == 0 && mas == 0) ? self.UCIST_HN(self.UCIST_HN()) : self.UCIST_HN(fem + mas)

        return 0;
    }, self);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.UCIST = ko.computed(function () {
            return parseInt(self.UCIFem()) + parseInt(self.UCIMaso());
        }, self);
    } else {
        if (!self.ActiveHON()) {
            self.UCIST = ko.observable(data.UCIST);
        }
    }

    //****
    self.UCISARIFem = ko.observable(data.UCISARIFem);
    self.UCISARIMaso = ko.observable(data.UCISARIMaso);
    //self.UCISARIST = ko.observable(data.UCISARIST);
    self.UCISARIST_HN = ko.observable(data.UCISARIST);
    self.UCISARIST_HN_Tmp = ko.observable();

    self.UCISARIST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.UCISARIFem());
        var mas = parseInt(self.UCISARIMaso());
        var tot = parseInt(self.UCISARIST_HN());

        (fem == 0 && mas == 0) ? self.UCISARIST_HN(self.UCISARIST_HN()) : self.UCISARIST_HN(fem + mas)

        return 0;
    }, self);

    //****
    self.DefFem = ko.observable(data.DefFem);
    self.DefMaso = ko.observable(data.DefMaso);
    self.DefST = ko.observable(data.DefST);
    self.DefST_HN = ko.observable(data.DefST);
    self.DefST_HN_Tmp = ko.observable();

    self.DefST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.DefFem());
        var mas = parseInt(self.DefMaso());
        var tot = parseInt(self.DefST_HN());

        (fem == 0 && mas == 0) ? self.DefST_HN(self.DefST_HN()) : self.DefST_HN(fem + mas)

        return 0;
    }, self);

    if (self.DisableCHI() || self.ActiveBOL()) {
        self.DefST = ko.computed(function () {
            return parseInt(self.DefFem()) + parseInt(self.DefMaso());
        }, self);
    } else {
        if (!self.ActiveHON()) {
            self.DefST = ko.observable(data.DefST);
        }
    }

    //****
    self.DefSARIFem = ko.observable(data.DefSARIFem);
    self.DefSARIMaso = ko.observable(data.DefSARIMaso);
    //self.DefSARIST = ko.observable(data.DefSARIST);
    self.DefSARIST_HN = ko.observable(data.DefSARIST);
    self.DefSARIST_HN_Tmp = ko.observable();

    self.DefSARIST_HN_Tmp = ko.computed(function () {
        var fem = parseInt(self.DefSARIFem());
        var mas = parseInt(self.DefSARIMaso());
        var tot = parseInt(self.DefSARIST_HN());

        (fem == 0 && mas == 0) ? self.DefSARIST_HN(self.DefSARIST_HN()) : self.DefSARIST_HN(fem + mas)

        return 0;
    }, self);

    //****
    self.NeuFem = ko.observable(data.NeuFem);
    self.NeuMaso = ko.observable(data.NeuMaso);

    self.NeuST = ko.observable(data.NeuST);

    //****
    self.CCSARIFem = ko.observable(data.CCSARIFem);
    self.CCSARIMaso = ko.observable(data.CCSARIMaso);

    if (self.ActiveHON()) {
        self.CCSARIST = ko.computed(function () {
            return parseInt(self.CCSARIFem()) + parseInt(self.CCSARIMaso());
        }, self);

    } else {
        self.CCSARIST = ko.observable(data.CCSARIST);
    }

    //****
    self.VentFem = ko.observable(data.VentFem);
    self.VentMaso = ko.observable(data.VentMaso);
    self.VentST = ko.observable(data.VentST);

    //**** ILI Jamaica
    self.ILICases = ko.observable(data.ILICases);
    self.ILISamplesTaken = ko.observable(data.ILISamplesTaken);
    self.TotalVisits = ko.observable(data.TotalVisits);

    //****
    self.ValidarIngresoDatosOK = ko.computed(function () {
        var bResu = true;

        if (self.ActiveHON()) {
            //---- Hospitalizaciones
            var hFem = parseInt(self.HospFem());
            var hMas = parseInt(self.HospMaso());
            var hTot = parseInt(self.HospST_HN());

            var hsFem = parseInt(self.HospSARIFem());
            var hsMas = parseInt(self.HospSARIMaso());
            var hsTot = parseInt(self.HospSARIST_HN());

            var cErrorHosp;
            cErrorHosp = "";

            if (hsFem > hFem) {
                cErrorHosp += "'Hospitalizados IRAG' FEMENINO no puede ser mayor que 'Hospitalizados' FEMENINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (hsMas > hMas) {
                cErrorHosp += "'Hospitalizados IRAG' MASCULINO no puede ser mayor que 'Hospitalizados' MASCULINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (cErrorHosp === "") {
                if (hsTot > hTot) {
                    cErrorHosp += "TOTAL 'Hospitalizados IRAG' no puede ser mayor que TOTAL 'Hospitalizados'" + " en " + self.AgeGroupDesc() + "\n";
                }
            }
            //---- UCI
            var uFem = parseInt(self.UCIFem());
            var uMas = parseInt(self.UCIMaso());
            var uTot = parseInt(self.UCIST_HN());

            var usFem = parseInt(self.UCISARIFem());
            var usMas = parseInt(self.UCISARIMaso());
            var usTot = parseInt(self.UCISARIST_HN());

            var cErrorUCI;
            cErrorUCI = "";

            if (usFem > uFem) {
                cErrorUCI += "'UCI IRAG' FEMENINO no puede ser mayor que 'UCI' FEMENINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (usMas > uMas) {
                cErrorUCI += "'UCI IRAG' MASCULINO no puede ser mayor que 'UCI' MASCULINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (cErrorUCI === "") {
                if (usTot > uTot) {
                    cErrorUCI += "TOTAL 'UCI IRAG' no puede ser mayor que TOTAL 'UCI'" + " en " + self.AgeGroupDesc() + "\n";
                }
            }
            //---- Fallecidos
            var dFem = parseInt(self.DefFem());
            var dMas = parseInt(self.DefMaso());
            var dTot = parseInt(self.DefST_HN());

            var dsFem = parseInt(self.DefSARIFem());
            var dsMas = parseInt(self.DefSARIMaso());
            var dsTot = parseInt(self.DefSARIST_HN());

            var cErrorDef;
            cErrorDef = "";

            if (dsFem > dFem) {
                cErrorDef += "'Fallecidos IRAG' FEMENINO no puede ser mayor que 'Fallecidos' FEMENINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (dsMas > dMas) {
                cErrorDef += "'Fallecidos IRAG' MASCULINO no puede ser mayor que 'Fallecidos' MASCULINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (cErrorDef === "") {
                if (dsTot > dTot) {
                    cErrorDef += "TOTAL 'Fallecidos IRAG' no puede ser mayor que TOTAL 'Fallecidos'" + " en " + self.AgeGroupDesc() + "\n";
                }
            }
            //---- Total atención vs Atenciones ETI
            var aFem = parseInt(self.ETIDenoFem());
            var aMas = parseInt(self.ETIDenoMaso());
            var aTot = parseInt(self.ETIDenoST_HN());

            var aeFem = parseInt(self.ETINumFem());
            var aeMas = parseInt(self.ETINumMaso());
            var aeTot = parseInt(self.ETINumST_HN());

            var cErrorEti;
            cErrorEti = "";

            if (aeFem > aFem) {
                cErrorEti += "'Atenciones ETI' FEMENINO no puede ser mayor que 'Atenciones' FEMENINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            if (aeMas > aMas) {
                cErrorEti += "'Atenciones ETI' MASCULINO no puede ser mayor que 'Atenciones' MASCULINO" + " en " + self.AgeGroupDesc() + "\n";
            }
            //console.log(">" + cErrorEti + "<" + aTot + " - " + aeTot)
            if (cErrorEti === "") {
                if (aeTot > aTot) {
                    cErrorEti += "TOTAL 'Atenciones ETI' no puede ser mayor que TOTAL 'Atenciones'" + " en " + self.AgeGroupDesc() + "\n";
                }
            }

            //----
            if (cErrorHosp != "" || cErrorUCI != "" || cErrorDef != "" || cErrorEti != "") {
                alert(cErrorHosp + cErrorUCI + cErrorDef + cErrorEti);
                bResu = false;
            }
        }

        return bResu
    }, self);

    //****
    self.MakeValueOfSummayItem = function () {
        return {
            Id: self.Id,
            CaseSummaryId: self.CaseSummaryId,
            AgeGroup: self.AgeGroup,
            AgeGroupDescription: self.AgeGroupDesc(),

            ETINumFem: typeof (self.ETINumFem()) !== "number" ? parseInt(self.ETINumFem()) : self.ETINumFem(),
            ETINumMaso: typeof (self.ETINumMaso()) !== "number" ? parseInt(self.ETINumMaso()) : self.ETINumMaso(),
            ETINumST: self.ActiveHON() ? (typeof (self.ETINumST_HN()) !== "number" ? parseInt(self.ETINumST_HN()) : self.ETINumST_HN()) :
                                         (typeof (self.ETINumST()) !== "number" ? parseInt(self.ETINumST()) : self.ETINumST()),

            ETIDenoFem: typeof (self.ETIDenoFem()) !== "number" ? parseInt(self.ETIDenoFem()) : self.ETIDenoFem(),
            ETIDenoMaso: typeof (self.ETIDenoMaso()) !== "number" ? parseInt(self.ETIDenoMaso()) : self.ETIDenoMaso(),
            ETIDenoST: self.ActiveHON() ? (typeof (self.ETIDenoST_HN()) !== "number" ? parseInt(self.ETIDenoST_HN()) : self.ETIDenoST_HN()) :
                                          (typeof (self.ETIDenoST()) !== "number" ? parseInt(self.ETIDenoST()) : self.ETIDenoST()),

            ETINumEmerFem: typeof (self.ETINumEmerFem()) !== "number" ? parseInt(self.ETINumEmerFem()) : self.ETINumEmerFem(),
            ETINumEmerMaso: typeof (self.ETINumEmerMaso()) !== "number" ? parseInt(self.ETINumEmerMaso()) : self.ETINumEmerMaso(),
            ETINumEmerST: typeof (self.ETINumEmerST()) !== "number" ? parseInt(self.ETINumEmerST()) : self.ETINumEmerST(),

            HospFem: typeof (self.HospFem()) !== "number" ? parseInt(self.HospFem()) : self.HospFem(),
            HospMaso: typeof (self.HospMaso()) !== "number" ? parseInt(self.HospMaso()) : self.HospMaso(),
            HospST: self.ActiveHON() ? (typeof (self.HospST_HN()) !== "number" ? parseInt(self.HospST_HN()) : self.HospST_HN()) :
                                       (typeof (self.HospST()) !== "number" ? parseInt(self.HospST()) : self.HospST()),

            HospSARIFem: typeof (self.HospSARIFem()) !== "number" ? parseInt(self.HospSARIFem()) : self.HospSARIFem(),
            HospSARIMaso: typeof (self.HospSARIMaso()) !== "number" ? parseInt(self.HospSARIMaso()) : self.HospSARIMaso(),
            HospSARIST: self.ActiveHON() ? (typeof (self.HospSARIST_HN()) !== "number" ? parseInt(self.HospSARIST_HN()) : self.HospSARIST_HN()) : 0,

            UCIFem: typeof (self.UCIFem()) !== "number" ? parseInt(self.UCIFem()) : self.UCIFem(),
            UCIMaso: typeof (self.UCIMaso()) !== "number" ? parseInt(self.UCIMaso()) : self.UCIMaso(),
            UCIST: self.ActiveHON() ? (typeof (self.UCIST_HN()) !== "number" ? parseInt(self.UCIST_HN()) : self.UCIST_HN()) :
                                      (typeof (self.UCIST()) !== "number" ? parseInt(self.UCIST()) : self.UCIST()),

            UCISARIFem: typeof (self.UCISARIFem()) !== "number" ? parseInt(self.UCISARIFem()) : self.UCISARIFem(),
            UCISARIMaso: typeof (self.UCISARIMaso()) !== "number" ? parseInt(self.UCISARIMaso()) : self.UCISARIMaso(),
            UCISARIST: (self.ActiveHON() || self.ActiveCOS()) ? (typeof (self.UCISARIST_HN()) !== "number" ? parseInt(self.UCISARIST_HN()) : self.UCISARIST_HN()) : 0,

            DefFem: typeof (self.DefFem()) !== "number" ? parseInt(self.DefFem()) : self.DefFem(),
            DefMaso: typeof (self.DefMaso()) !== "number" ? parseInt(self.DefMaso()) : self.DefMaso(),
            DefST: self.ActiveHON() ? (typeof (self.DefST_HN()) !== "number" ? parseInt(self.DefST_HN()) : self.DefST_HN()) :
                                      (typeof (self.DefST()) !== "number" ? parseInt(self.DefST()) : self.DefST()),

            DefSARIFem: typeof (self.DefSARIFem()) !== "number" ? parseInt(self.DefSARIFem()) : self.DefSARIFem(),
            DefSARIMaso: typeof (self.DefSARIMaso()) !== "number" ? parseInt(self.DefSARIMaso()) : self.DefSARIMaso(),
            DefSARIST: (self.ActiveHON() || self.ActiveCOS()) ? (typeof (self.DefSARIST_HN()) !== "number" ? parseInt(self.DefSARIST_HN()) : self.DefSARIST_HN()) : 0,

            ILICases: typeof (self.ILICases()) !== "number" ? parseInt(self.ILICases()) : self.ILICases(),
            TotalVisits: typeof (self.TotalVisits()) !== "number" ? parseInt(self.TotalVisits()) : self.TotalVisits(),
            ILISamplesTaken: typeof (self.ILISamplesTaken()) !== "number" ? parseInt(self.ILISamplesTaken()) : self.ILISamplesTaken(),

            NeuFem: typeof (self.NeuFem()) !== "number" ? parseInt(self.NeuFem()) : self.NeuFem(),
            NeuMaso: typeof (self.NeuMaso()) !== "number" ? parseInt(self.NeuMaso()) : self.NeuMaso(),
            NeuST: typeof (self.NeuST()) !== "number" ? parseInt(self.NeuST()) : self.NeuST(),

            CCSARIFem: typeof (self.CCSARIFem()) !== "number" ? parseInt(self.CCSARIFem()) : self.CCSARIFem(),
            CCSARIMaso: typeof (self.CCSARIMaso()) !== "number" ? parseInt(self.CCSARIMaso()) : self.CCSARIMaso(),
            CCSARIST: typeof (self.CCSARIST()) !== "number" ? parseInt(self.CCSARIST()) : self.CCSARIST(),

            VentFem: typeof (self.VentFem()) !== "number" ? parseInt(self.VentFem()) : self.VentFem(),
            VentMaso: typeof (self.VentMaso()) !== "number" ? parseInt(self.VentMaso()) : self.VentMaso(),
            VentST: typeof (self.VentST()) !== "number" ? parseInt(self.VentST()) : self.VentST()
        };
    }
}

function SummaryViewModel(app, dataModel) {
    var self = this;

    var date_format_moment = app.dataModel.date_format_moment;
    var date_format_ISO = app.dataModel.date_format_ISO;
    var date_format_ = app.dataModel.date_format_;
    var date_format_DatePicker = app.dataModel.date_format_DatePicker;

    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.selectedCountryId = ko.observable("");
    self.countries = ko.observableArray(countries);
    //self.Surv = ko.observable("1");
    if(self.UsrCountry() != 17)                             // 17: Jamaica #### CAFQ: 190227
        self.Surv = ko.observable(SurvGlobal);

    self.activecountries = ko.computed(function () {
        return $.grep(self.countries(), function (v) {
            return v.Active === true;
        });
    });
    self.selectedHospitalId = ko.observable("");
    self.selectedHospitalId.subscribe(function (HospitalSelect) {
        self.ChangeHospitalAndDate();
    });

    self.hospitals = ko.observableArray(institutions);
    self.HospitalDate = ko.observable(null);
    self.HospitalEW = ko.observable("");
    self.HospitalYE = ko.observable("");

    self.availableYears = ko.computed(function () {
        var years = [];
        var yearCurr = (new Date).getFullYear();

        for (var i = yearCurr; i >= 2010; i--) {
            years.push(i.toString());
        }

        return years;
    }, self);
    self.EpiYear = ko.observable((new Date).getFullYear().toString());

    self.ColHospTST01 = ko.observable("");
    self.ColUCITST01 = ko.observable("");
    self.ColFalleTST01 = ko.observable("");

    self.ColILICasesST01 = ko.observable("");
    self.ColILISamplesTakenST01 = ko.observable("");
    self.ColTotalVisitsST01 = ko.observable("");

    self.ColETIFST = ko.observable("");
    self.ColETIMST = ko.observable("");

    self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
        if ($("#" + FieldDate).val() != "") {
            var date_ew = new Date($("#" + FieldDate).datepicker('getDate', {
                dateFormat: date_format_DatePicker
            }));
            var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
            var weekno = moment(date_ew).week();
            var weeknoISO = moment(date_ew).isoWeek();

            if (date_ew == null) {

                FieldAct(null);
                FieldActYear(null);

            } else {

                if (fwky_date > 3) {
                    var month = 11, day = 31;
                    var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

                    if (weekno == 1 && moment(date_ew).month() == 0) {
                        var fwky_date_ant = new Date(moment(date_ew).year() - 1, 0, 1).getDay();
                        var fwdoyant = moment(end_date_year_ant).isoWeek();
                        if (fwky_date_ant > 3) {

                            FieldAct(fwdoyant - 1);

                        } else {

                            if (weekno == 1 && moment(date_ew).month() == 0 && fwky_date_ant <= 3) {
                                FieldAct(53);
                                fwdoyant = 53;
                            }
                            else
                                FieldAct(fwdoyant);
                        }
                        if (FieldActYear != "")
                            if (fwdoyant == 52 || fwdoyant == 53)
                                FieldActYear(date_ew.getFullYear() - 1);
                            else
                                FieldActYear(date_ew.getFullYear());
                    }
                    else if (weekno == 1 && moment(date_ew).month() != 0) {
                        FieldAct(moment(date_ew).isoWeek() - 1);
                        if (FieldActYear != "")
                            FieldActYear(date_ew.getFullYear());
                    }
                    else {
                        FieldAct(weekno - 1);
                        if (FieldActYear != "")
                            FieldActYear(date_ew.getFullYear());
                    }
                } else {
                    if (weekno == 1 && moment(date_ew).month() == 11) {
                        var fwky_date_prox = new Date(date_ew.getFullYear() + 1, 0, 1).getDay();

                        if (fwky_date_prox > 3) {
                            FieldAct(53);
                            FieldActYear(date_ew.getFullYear());
                        } else {
                            FieldAct(weekno);
                            FieldActYear(date_ew.getFullYear() + 1);
                        }
                    } else {
                        FieldAct(weekno);
                        if (FieldActYear != "")
                            FieldActYear(date_ew.getFullYear());
                    }
                }
            }
        }
    };

    self.HospitalDate.subscribe(function (HospitalDate) {
        self.ChangeHospitalAndDate();
        self.CalculateEW("HospitalDate", self.HospitalEW, self.HospitalYE);
    });

    self.SummayItems = ko.observableArray([]);
    self.SummaryForYearItems = ko.observableArray([]);

    //****
    self.EnableCHI = ko.computed(function () {
        return (self.UsrCountry() != 7) ? true : false;
    }, self);

    self.DisableCHI = ko.computed(function () {
        return (self.UsrCountry() == 7) ? true : false;

    }, self);

    self.ActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? true : false;
    }, self);

    self.NoActiveBOL = ko.computed(function () {
        return (self.UsrCountry() == 3) ? false : true;
    }, self);

    self.ActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? true : false;
    }, self);

    self.NoActiveSUR = ko.computed(function () {
        return (self.UsrCountry() == 25) ? false : true;
    }, self);

    self.ActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? true : false;
    }, self);

    self.NoActiveJAM = ko.computed(function () {
        return (self.UsrCountry() == 17) ? false : true;
    }, self);

    self.ActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? true : false;
    }, self);

    self.NoActiveCOS = ko.computed(function () {
        return (self.UsrCountry() == 9) ? false : true;
    }, self);

    self.ActiveDOM = ko.computed(function () {
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
    
    self.ActiveHON = ko.computed(function () {
        return (self.UsrCountry() == 15) ? true : false;
    }, self);

    self.ActiveSLU = ko.computed(function () {
        return (self.UsrCountry() == 18) ? true : false;
    }, self);
    //****
    self.PickFirstDay = function (date) {
        var day = date.getDay();
        return [day == 0, " "];
    };

    self.PickLastDay = function (date) {
        var day = date.getDay();
        return [day == 6, " "];
    };

    self.AddHospital = function (element, index, array) {
        self.hospitals.push({ Id: element.Id, Name: element.Name });
    };

    self.ReloadHospitals = function () {
        if (typeof self.selectedCountryId === "undefined") {
            return;
        }
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.hospitals(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };

    self.AddSummayItem = function (element, index, array) {
        self.SummayItems.push(new SummayItem(element));
    };

    self.AddSummaryForYearItems = function (element, index, array) {
        self.SummaryForYearItems.push(new SummaryYearItem(element));
    };

    self.ILI_OK = ko.computed(function () {
        bResu = true;
        //----
        if (self.ActiveHON() || self.ActiveCOS()) {
            if (self.Surv() == "2") {
                SurvGlobal = "2";
                bResu = true;
            }
            else {
                SurvGlobal = "1";
                bResu = false;
            }
        }
        //----
        return bResu;
    }, self);

    self.SARI_OK = ko.computed(function () {
        bResu = true;
        //----
        if (self.ActiveHON() || self.ActiveCOS()) {
            if (self.Surv() == "1") {
                SurvGlobal = "1";
                bResu = true;
            }
            else {
                SurvGlobal = "2";
                bResu = false;
            }
        }
        //----
        return bResu;
    }, self);

    self.UCI_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === self.selectedHospitalId()) {
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("- OTHERS") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    //------------------------------------------------------
    self.HOS_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.HOS_IRAG_ShowCol_F = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('F');
    }, self);

    self.HOS_IRAG_ShowCol_M = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('M');
    }, self);

    self.HOS_IRAG_ShowCol_T = ko.computed(function () {
        return self.HOS_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------
    self.UCI_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.UCI_IRAG_ShowCol_F = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('F');
    }, self);

    self.UCI_IRAG_ShowCol_M = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('M');
    }, self);

    self.UCI_IRAG_ShowCol_T = ko.computed(function () {
        return self.UCI_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------
    self.DEF_IRAG_ShowCol = function (tipoColu) {
        bResu = false;
        if (self.UsrCountry() == 9) {
            if (tipoColu == "T")
                bResu = true;

        } else {
            bResu = true;
        }
        return bResu;
    };

    self.DEF_IRAG_ShowCol_F = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('F');
    }, self);

    self.DEF_IRAG_ShowCol_M = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('M');
    }, self);

    self.DEF_IRAG_ShowCol_T = ko.computed(function () {
        return self.DEF_IRAG_ShowCol('T');
    }, self);
    //------------------------------------------------------

    self.Hospitalizacion_OK = ko.computed(function () {
        bResu = true;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === self.selectedHospitalId()) {
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("- ICU") > 0)
                        bResu = false;
                    break;
                }
            }
        }

        return bResu;
    }, self);

    self.ServPedriatic_SURI = ko.computed(function () {
        bResu = false;

        if (self.ActiveSUR()) {
            for (var i = 0; i < institutions.length; i++) {
                if (institutions[i].Id === self.selectedHospitalId()) {
                    var institSelect = institutions[i].Name.toUpperCase();
                    if (institSelect.indexOf("- PEDIATRIC") > 0)  
                        bResu = true;
                    break;
                }
            }
        }

        return bResu;
    }, self);
    
    //***********************  Esto es para calcular los totales de las columnas
    //****
    self.ColETIDenoFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETIDenoFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIDenoMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETIDenoMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIDenoST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (self.ActiveHON()) {
                numberofitems += parseInt(r.ETIDenoST_HN());
            } else {
                numberofitems += parseInt(r.ETIDenoST());
            }
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColETIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (self.ActiveHON()) {
                numberofitems += parseInt(r.ETINumST_HN());
            } else {
                numberofitems += parseInt(r.ETINumST());
            }
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColETINumEmerFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumEmerMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColETINumEmerST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.ETINumEmerST());
        });
        return parseInt(numberofitems);
    }, self);

    //****
    self.ColHospFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColHospMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColHospTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (self.ActiveHON()) {
                numberofitems += parseInt(r.HospST_HN());
            } else {
                numberofitems += parseInt(r.HospST());
            }
        });
        return parseInt(numberofitems);
    }, self);

    //****
    self.ColHospSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColHospSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.HospSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    if (self.ActiveHON()) {
        self.ColHospSARITST = ko.computed(function () {
            var numberofitems = 0;
            ko.utils.arrayForEach(self.SummayItems(), function (r) {
                if (self.ActiveHON()) {
                    numberofitems += parseInt(r.HospSARIST_HN());
                } else {
                    numberofitems += parseInt(r.HospSARIST());
                }
            });
            return parseInt(numberofitems);
        }, self);
    }
    //****
    self.ColUCIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (self.ActiveHON()) {
                numberofitems += parseInt(r.UCIST_HN());
            } else {
                numberofitems += parseInt(r.UCIST());
            }
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColUCISARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCISARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColUCISARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.UCISARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    if (self.ActiveHON() || self.ActiveCOS()) { // || self.ActiveCOS()
        self.ColUCISARITST = ko.computed(function () {
            var numberofitems = 0;
            ko.utils.arrayForEach(self.SummayItems(), function (r) {
                if (self.ActiveHON() || self.ActiveCOS()) {
                    numberofitems += parseInt(r.UCISARIST_HN());
                } else {
                    numberofitems += parseInt(r.UCISARIST());
                }
            });
            return parseInt(numberofitems);
        }, self);
    }
    //****
    self.ColFalleFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (self.ActiveHON()) {
                numberofitems += parseInt(r.DefST_HN());
            } else {
                numberofitems += parseInt(r.DefST());
            }
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColFalleSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColFalleSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.DefSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    if (self.ActiveHON() || self.ActiveCOS()) {
        self.ColFalleSARITST = ko.computed(function () {
            var numberofitems = 0;
            ko.utils.arrayForEach(self.SummayItems(), function (r) {
                if (self.ActiveHON() || self.ActiveCOS()) {
                    numberofitems += parseInt(r.DefSARIST_HN());
                } else {
                    numberofitems += parseInt(r.DefSARIST());
                }
            });
            return parseInt(numberofitems);
        }, self);
    }
    //****
    self.ColNeuTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.NeuST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColCCSARIFST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIFem());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColCCSARIMST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIMaso());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColCCSARITST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.CCSARIST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColVentTST = ko.computed(function () {
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            numberofitems += parseInt(r.VentST());
        });
        return parseInt(numberofitems);
    }, self);
    //****
    self.ColILICasesST = ko.computed(function () {                      //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['ILICases'] != undefined)
                numberofitems += parseInt(r.ILICases());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColILISamplesTakenST = ko.computed(function () {               //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['ILISamplesTaken'] != undefined)
                numberofitems += parseInt(r.ILISamplesTaken());
        });
        return parseInt(numberofitems);
    }, self);

    self.ColTotalVisitsST = ko.computed(function () {                   //#### CAFQ: ILI Jamaica
        var numberofitems = 0;
        ko.utils.arrayForEach(self.SummayItems(), function (r) {
            if (r['TotalVisits'] != undefined)
                numberofitems += parseInt(r.TotalVisits());
        });
        return parseInt(numberofitems);
    }, self);
    //*********************** Aquí termina el calculo de los totales

    //****
    self.MakeValuesOfSummayItems = function () {
        var index;
        var ValuesOfSummayItems = [];
        for (index = 0; index < self.SummayItems().length; ++index) {
            ValuesOfSummayItems.push(self.SummayItems()[index].MakeValueOfSummayItem());
        };
        //console.log(ValuesOfSummayItems);
        return ValuesOfSummayItems;
    };

    self.ChangeHospitalAndDate = function () {
        self.SummayItems.removeAll();
        $("#TotalSummary").hide();
        $("#LabelSummary").hide();
        $("#ButtonSummary").hide();
        $("#SurvContainer").show();
    };

    self.CancelarItems = function () {
        self.SummayItems.removeAll();

        if ($.isFunction(self.Id)) {
            self.Id = ko.observable("");
            self.Id("");
        }
        else {
            self.Id = "";
        }

        self.HospitalDate("");
        self.HospitalEW("");
        self.HospitalYE("");
        $("#TotalSummary").hide();
        $("#LabelSummary").hide();
        $("#ButtonSummary").hide();
        $("#SurvContainer").show();
    };

    self.GetYearSummaryForYearItems = function () {
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "") {
            $.postJSON(app.dataModel.getSummaryForYearUrl, { hospitalId: self.selectedHospitalId(), epiYear: self.EpiYear() })       // self.EpiYear
               .success(function (data, textStatus, jqXHR) {
                   $("#LabelBandeja").show();
                   $("#TotalBandeja").show();
                   self.SummaryForYearItems([]);
                   data.forEach(self.AddSummaryForYearItems);
               })
               .fail(function (jqXHR, textStatus, errorThrown) {
                   alert(errorThrown);
               })
        }
    };

    self.GetYearSummaryForYearItemsJM = function () {
        if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "" || self.selectedHospitalId() == "0") {  
            self.HospitalDate(null);
            self.HospitalEW(null);
            self.HospitalYE(null);
        }

        $.postJSON(app.dataModel.getSummaryForYearUrlJM, { hospitalId: self.selectedHospitalId(), epiYear: self.EpiYear() })
            .success(function (data, textStatus, jqXHR) {
                $("#LabelBandeja").show();
                $("#TotalBandeja").show();
                self.SummaryForYearItems([]);
                data.forEach(self.AddSummaryForYearItems);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
    };

    self.GetSummaryExcel = function () {
        //console.log("self.GetSummaryExcel->START_1043");
        //var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId() }
        //if (self.validate() == true)
        var namevalues = { hospitalId: self.selectedHospitalId(), hospitalDate: moment(self.HospitalDate()).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() };
        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            window.open(app.dataModel.getSummayDetailsExcel + "?" + $.param(namevalues, true), "_blank");
        } else {
            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                //alert("Seleccionar un establecimiento es requerido");
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    }

    self.IsTieneServicios = function (myArray, Valor) {
        var vOK = false;
        for (var i = 0; i < myArray.length; i++) {
            if (myArray[i].Id === Valor) {
                vOK = true;
            }
        }
        return vOK
    };

    self.GetSummayItems = function () {
        if (self.selectedHospitalId() == 0) {
            alert(msgViewSummaryAlertSelectHospital);
            return;
        }
        if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "" || self.HospitalDate() == null) {
            alert(msgViewSummaryAlertSelectDate);
            return;
        }
        if (self.IsTieneServicios(institutionsWithServ, self.selectedHospitalId())) {
            alert(msgViewSummaryAlertServices);
            return
        }

        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            DatetoSend = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_)
            $.postJSON(app.dataModel.getSummayDetailsUrl, {
                hospitalId: self.selectedHospitalId(),
                hospitalDate: moment(DatetoSend).format(date_format_ISO),
                EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE()
            })
            .success(function (data, textStatus, jqXHR) {
                self.SummayItems([]);
                data.forEach(self.AddSummayItem);
                $("#LabelSummary").show();
                $("#TotalSummary").show();
                $("#ButtonSummary").show();
                $("#SurvContainer").hide();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        } else {

            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    };

    self.GetSummayItemsJM = function () {
        if (self.selectedHospitalId() == 0) {
            self.HospitalDate(null);
            self.HospitalEW(null);
            self.HospitalYE(null);
            //self.CancelarItems()
            alert(msgViewSummaryAlertSelectILISentinelSite);
            return;
        }

        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
            DatetoSend = jQuery.type(self.HospitalDate()) === 'date' ? self.HospitalDate() : parseDate($("#HospitalDate").val(), date_format_)
            $.postJSON(app.dataModel.getSummayDetailsUrlJM, { hospitalId: self.selectedHospitalId(), hospitalDate: moment(DatetoSend).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() })
            .success(function (data, textStatus, jqXHR) {
                self.SummayItems([]);
                data.forEach(self.AddSummayItem);
                $("#LabelSummary").show();
                $("#TotalSummary").show();
                $("#ButtonSummary").show();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            })
        } else {
            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Hospital is required");
                else
                    alert("Seleccionar un establecimiento es requerido");
            }

            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Date is required");
                else
                    alert("La fecha es requerida");
            }
        }
    };

    function ValidarDatosSave() {
        var cError = "";

        for (index = 0; index < self.SummayItems().length; ++index) {
            item = self.SummayItems()[index].MakeValueOfSummayItem();

            //**** Hospitalizados
            var hFem = parseInt(item.HospFem);
            var hMas = parseInt(item.HospMaso);
            var hTot = parseInt(item.HospST);

            var hsFem = parseInt(item.HospSARIFem);
            var hsMas = parseInt(item.HospSARIMaso);
            var hsTot = parseInt(item.HospSARIST);

            cTemp = "";
            if (hsFem > hFem)
                cTemp += "'Hospitalizados IRAG' FEMENINO no puede ser mayor que 'Hospitalizados' FEMENINO" + " en " + item.AgeGroupDescription + "\n";

            if (hsMas > hMas)
                cTemp += "'Hospitalizados IRAG' MASCULINO no puede ser mayor que 'Hospitalizados' MASCULINO" + " en " + item.AgeGroupDescription + "\n";

            if (cTemp === "")
                if (hsTot > hTot)
                    cTemp += "TOTAL 'Hospitalizados IRAG' no puede ser mayor que TOTAL 'Hospitalizados'" + " en " + item.AgeGroupDescription + "\n";

            cError += cTemp

            //**** UCI
            var uFem = parseInt(item.UCIFem);
            var uMas = parseInt(item.UCIMaso);
            var uTot = parseInt(item.UCIST);

            var usFem = parseInt(item.UCISARIFem);
            var usMas = parseInt(item.UCISARIMaso);
            var usTot = parseInt(item.UCISARIST);

            cTemp = "";
            if (usFem > uFem)
                cTemp += "'UCI IRAG' FEMENINO no puede ser mayor que 'UCI' FEMENINO" + " en " + item.AgeGroupDescription + "\n";

            if (usMas > uMas)
                cTemp += "'UCI IRAG' MASCULINO no puede ser mayor que 'UCI' MASCULINO" + " en " + item.AgeGroupDescription + "\n";

            console.log("1->" + cTemp + "<-" + usTot + " - " + uTot);
            if (cTemp === "")
                if (usTot > uTot)
                    cTemp += "TOTAL 'UCI IRAG' no puede ser mayor que TOTAL 'UCI'" + " en " + item.AgeGroupDescription + "\n";

            cError += cTemp

            //**** Fallecidos
            var dFem = parseInt(item.DefFem);
            var dMas = parseInt(item.DefMaso);
            var dTot = parseInt(item.DefST);

            var dsFem = parseInt(item.DefSARIFem);
            var dsMas = parseInt(item.DefSARIMaso);
            var dsTot = parseInt(item.DefSARIST);

            cTemp = "";
            if (dsFem > dFem)
                cTemp += "'Fallecidos IRAG' FEMENINO no puede ser mayor que 'Fallecidos' FEMENINO" + " en " + item.AgeGroupDescription + "\n";

            if (dsMas > dMas)
                cTemp += "'Fallecidos IRAG' MASCULINO no puede ser mayor que 'Fallecidos' MASCULINO" + " en " + item.AgeGroupDescription + "\n";

            if (cTemp === "")
                if (dsTot > dTot)
                    cTemp += "TOTAL 'Fallecidos IRAG' no puede ser mayor que TOTAL 'Fallecidos'" + " en " + item.AgeGroupDescription + "\n";

            cError += cTemp

            //**** Atenciones vs atenciones ETI
            var aFem = parseInt(item.ETIDenoFem);
            var aMas = parseInt(item.ETIDenoMaso);
            var aTot = parseInt(item.ETIDenoST);

            var aeFem = parseInt(item.ETINumFem);
            var aeMas = parseInt(item.ETINumMaso);
            var aeTot = parseInt(item.ETINumST);

            cTemp = "";
            if (aeFem > aFem)
                cTemp += "'Atenciones ETI' FEMENINO no puede ser mayor que 'Atenciones' FEMENINO" + " en " + item.AgeGroupDescription + "\n";

            if (aeMas > aMas)
                cTemp += "'Atenciones ETI' MASCULINO no puede ser mayor que 'Atenciones' MASCULINO" + " en " + item.AgeGroupDescription + "\n";

            if (cTemp === "")
                if (aeTot > aTot)
                    cTemp += "TOTAL 'Atenciones ETI' no puede ser mayor que TOTAL 'Atenciones'" + " en " + item.AgeGroupDescription + "\n";

            cError += cTemp

            //**** 
        };

        return cError;
    }

    self.SaveSummayItems = function () {
        if (self.ActiveHON()) {
            var cErrorF = "";
            cErrorF = ValidarDatosSave();
            if (cErrorF != "") {
                alert("No es posible grabar, se detectaron los siguientes errores: \n\n" + cErrorF);
                return 0;
            }
        }

        //****
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: app.dataModel.saveSummayDetailsUrl,
            data: JSON.stringify({ casesummaryDetails: self.MakeValuesOfSummayItems() }),
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Data saved...");
                    ///////alert(msgSavedData);
                else
                    alert(data);
                self.CancelarItems();
                self.GetYearSummaryForYearItems();  //***********
            },
        })
    };

    self.SaveSummayItemsJM = function () {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: app.dataModel.saveSummayDetailsUrlJM,
            data: JSON.stringify({ casesummaryDetails: self.MakeValuesOfSummayItems() }),
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
                    alert("Data saved...");
                    ///////alert(msgSavedData);
                else
                    alert(data);
                self.CancelarItems();
                self.GetYearSummaryForYearItemsJM();  //***********
            },
        })
    };

    self.showEpiWeek_AM = function (option, item) {
        console.log(self.HospitalDate());
    };

    return self;
};

app.addViewModel({
    name: "Summary",
    bindingMemberName: "summary",
    factory: SummaryViewModel
});

/** Bandeja de denominadores **/
function showEpiWeek(data, event) {
    var date = moment.unix(data.StartDateOfWeek).utc().format(moment_date_format);
    var week = data.ColEpiWeek;
    var year = data.ColEpiYear;

    $("#HospitalDate").val(date);
    $("#HospitalDate").change();
    $("#HospitalEW").val(week);
    $("#HospitalEW").change();
    $("#HospitalYE").val(year);
    $("#HospitalYE").change();
    $("#search").click();
}

function showEpiWeekJM(data, event) {
    var date = moment.unix(data.WeekendDate).utc().format(moment_date_format);
    var week = data.ColEpiWeek;
    var year = data.ColEpiYear;

    $("#HospitalDate").val(date);
    $("#HospitalDate").change();
    $("#HospitalEW").val(week);
    $("#HospitalEW").change();
    $("#HospitalYE").val(year);
    $("#HospitalYE").change();
    $("#search").click();
}
/** Fin Bandeja de denominadores **/
