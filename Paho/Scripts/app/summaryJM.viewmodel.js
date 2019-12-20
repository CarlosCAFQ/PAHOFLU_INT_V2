
//function SummaryYearItem(data) {
//    var self = this;
//    self.ColHospTST = data.ColHospTST;
//    self.ColUCITST = data.ColUCITST;
//    self.ColFalleTST = data.ColFalleTST;
//    self.ColEpiYear = data.EpiYear;
//    self.ColEpiWeek = data.EpiWeek;
//    self.ColEpiYearWeek = data.EpiYear + "-" + data.EpiWeek;
//    self.ColETINumST = data.ColETINumST;
//    self.ColETIDenoST = data.ColETIDenoST;
//    self.ColETINumEmerST = data.ColETINumEmerST;
//    self.ColNeuTST = data.ColNeuTST;
//    self.ColVentTST = data.ColVentTST;
//    self.StartDateOfWeek = data.StartDateOfWeek;

//    self.ColETIFST = ko.observable("");
//    self.ColETIMST = ko.observable("");

//    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado

//    self.EnableCHI = ko.computed(function () {
//        return (self.UsrCountry() != 7) ? true : false;
//    }, self);

//    self.DisableCHI = ko.computed(function () {
//        return (self.UsrCountry() == 7) ? true : false;
//    }, self);

//    self.ActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? true : false;
//    }, self);

//    self.NoActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? false : true;
//    }, self);

//    self.ActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? true : false;
//    }, self);

//    self.NoActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? false : true;
//    }, self);

//    self.ActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? true : true;
//    }, self);

//    self.NoActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? false : true;
//    }, self);

//    self.ActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? true : false;
//    }, self);

//    self.NoActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? false : true;
//    }, self);

//    self.ActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? true : false;
//    }, self);

//    self.NoActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? false : true;
//    }, self);
//}

//function SummayItem(data) {
//    var self = this;
//    self.Id = data.Id;
//    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
//    self.CaseSummaryId = data.CaseSummaryId;
//    self.AgeGroup = data.AgeGroup;

//    //if (self.UsrCountry() == 7 || self.UsrCountry() == 3) {
//    //    self.AgeGroupDescription = AgeGroupDescriptionCHI[parseInt(self.AgeGroup) - 1];
//    //} else if (self.UsrCountry() == 25 || self.UsrCountry() == 11 || self.UsrCountry() == 18) {
//    //    self.AgeGroupDescription = AgeGroupDescriptionSUR[parseInt(self.AgeGroup) - 1];
//    //} else {
//    //    self.AgeGroupDescription = AgeGroupDescription[parseInt(self.AgeGroup) - 1];
//    //}

//    self.AgeGroupDescription = CatAgeGroup[parseInt(self.AgeGroup) - 1].AgeGroup;

//    self.EnableCHI = ko.computed(function () {
//        return (self.UsrCountry() != 7) ? true : false;
//    }, self);

//    self.DisableCHI = ko.computed(function () {
//        return (self.UsrCountry() == 7) ? true : false;
//    }, self);

//    self.ActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? true : false;
//    }, self);

//    self.NoActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? false : true;
//    }, self);

//    self.ActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? true : false;
//    }, self);

//    self.NoActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? false : true;
//    }, self);

//    self.ActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? true : true;
//    }, self);

//    self.NoActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? false : true;
//    }, self);

//    self.ActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? true : false;
//    }, self);

//    self.NoActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? false : true;
//    }, self);

//    self.ActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? true : false;
//    }, self);

//    self.NoActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? false : true;
//    }, self);

//    self.ETINumFem = ko.observable(data.ETINumFem);
//    self.ETINumMaso = ko.observable(data.ETINumMaso);
//    self.ETINumST = ko.observable(data.ETINumST);
//    self.ETINumEmerST = ko.observable(data.ETINumEmerST);
//    self.ETIDenoST = ko.observable(data.ETIDenoST);

//    self.HospFem = ko.observable(data.HospFem);
//    self.HospMaso = ko.observable(data.HospMaso);
//    if (self.DisableCHI() || self.ActiveBOL()) {
//        self.HospST = ko.computed(function () {
//            return parseInt(self.HospFem()) + parseInt(self.HospMaso());
//        }, self);

//    } else {
//        self.HospST = ko.observable(data.HospST);
//    }

//    self.UCIFem = ko.observable(data.UCIFem);
//    self.UCIMaso = ko.observable(data.UCIMaso);
//    if (self.DisableCHI() || self.ActiveBOL()) {
//        self.UCIST = ko.computed(function () {
//            return parseInt(self.UCIFem()) + parseInt(self.UCIMaso());
//        }, self);
//    } else {
//        self.UCIST = ko.observable(data.UCIST);
//    }

//    self.DefFem = ko.observable(data.DefFem);
//    self.DefMaso = ko.observable(data.DefMaso);
//    if (self.DisableCHI() || self.ActiveBOL()) {
//        self.DefST = ko.computed(function () {
//            return parseInt(self.DefFem()) + parseInt(self.DefMaso());
//        }, self);
//    } else {
//        self.DefST = ko.observable(data.DefST);
//    }

//    self.NeuFem = ko.observable(data.NeuFem);
//    self.NeuMaso = ko.observable(data.NeuMaso);
//    self.NeuST = ko.observable(data.NeuST);

//    self.CCSARIFem = ko.observable(data.CCSARIFem);
//    self.CCSARIMaso = ko.observable(data.CCSARIMaso);
//    self.CCSARIST = ko.observable(data.CCSARIST);

//    self.VentFem = ko.observable(data.VentFem);
//    self.VentMaso = ko.observable(data.VentMaso);
//    self.VentST = ko.observable(data.VentST);

//    self.MakeValueOfSummayItem = function () {
//        return {
//            Id: self.Id,
//            CaseSummaryId: self.CaseSummaryId,
//            AgeGroup: self.AgeGroup,
//            ETINumFem: typeof (self.ETINumFem()) !== "number" ? parseInt(self.ETINumFem()) : self.ETINumFem(),
//            ETINumMaso: typeof (self.ETINumMaso()) !== "number" ? parseInt(self.ETINumMaso()) : self.ETINumMaso(),
//            ETINumST: typeof (self.ETINumST()) !== "number" ? parseInt(self.ETINumST()) : self.ETINumST(),
//            ETINumEmerST: typeof (self.ETINumEmerST()) !== "number" ? parseInt(self.ETINumEmerST()) : self.ETINumEmerST(),
//            ETIDenoST: typeof (self.ETIDenoST()) !== "number" ? parseInt(self.ETIDenoST()) : self.ETIDenoST(),
//            HospFem: typeof (self.HospFem()) !== "number" ? parseInt(self.HospFem()) : self.HospFem(),
//            HospMaso: typeof (self.HospMaso()) !== "number" ? parseInt(self.HospMaso()) : self.HospMaso(),
//            HospST: typeof (self.HospST()) !== "number" ? parseInt(self.HospST()) : self.HospST(),
//            UCIFem: typeof (self.UCIFem()) !== "number" ? parseInt(self.UCIFem()) : self.UCIFem(),
//            UCIMaso: typeof (self.UCIMaso()) !== "number" ? parseInt(self.UCIMaso()) : self.UCIMaso(),
//            UCIST: typeof (self.UCIST()) !== "number" ? parseInt(self.UCIST()) : self.UCIST(),
//            DefFem: typeof (self.DefFem()) !== "number" ? parseInt(self.DefFem()) : self.DefFem(),
//            DefMaso: typeof (self.DefMaso()) !== "number" ? parseInt(self.DefMaso()) : self.DefMaso(),
//            DefST: typeof (self.DefST()) !== "number" ? parseInt(self.DefST()) : self.DefST(),
//            NeuFem: typeof (self.NeuFem()) !== "number" ? parseInt(self.NeuFem()) : self.NeuFem(),
//            NeuMaso: typeof (self.NeuMaso()) !== "number" ? parseInt(self.NeuMaso()) : self.NeuMaso(),
//            NeuST: typeof (self.NeuST()) !== "number" ? parseInt(self.NeuST()) : self.NeuST(),
//            CCSARIFem: typeof (self.CCSARIFem()) !== "number" ? parseInt(self.CCSARIFem()) : self.CCSARIFem(),
//            CCSARIMaso: typeof (self.CCSARIMaso()) !== "number" ? parseInt(self.CCSARIMaso()) : self.CCSARIMaso(),
//            CCSARIST: typeof (self.CCSARIST()) !== "number" ? parseInt(self.CCSARIST()) : self.CCSARIST(),
//            VentFem: typeof (self.VentFem()) !== "number" ? parseInt(self.VentFem()) : self.VentFem(),
//            VentMaso: typeof (self.VentMaso()) !== "number" ? parseInt(self.VentMaso()) : self.VentMaso(),
//            VentST: typeof (self.VentST()) !== "number" ? parseInt(self.VentST()) : self.VentST()
//        };
//    }
//}

//function SummaryJMViewModel(app, dataModel) {
//    var self = this;

//    /////////////var msgSavedData = app.dataModel.MsgSavedData;      //#### CAFQ 

//    var date_format_moment = app.dataModel.date_format_moment;
//    var date_format_ISO = app.dataModel.date_format_ISO;
//    var date_format_ = app.dataModel.date_format_;
//    var date_format_DatePicker = app.dataModel.date_format_DatePicker;

//    self.Id = "";
//    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
//    self.selectedCountryId = ko.observable("");
//    self.countries = ko.observableArray(countries);
//    self.activecountries = ko.computed(function () {
//        return $.grep(self.countries(), function (v) {
//            return v.Active === true;
//        });
//    });
//    self.selectedHospitalId = ko.observable("");
//    self.selectedHospitalId.subscribe(function (HospitalSelect) {
//        self.ChangeHospitalAndDate();
//    });

//    self.hospitals = ko.observableArray(institutions);
//    self.HospitalDate = ko.observable(null);
//    self.HospitalEW = ko.observable("");
//    self.HospitalYE = ko.observable("");

//    self.ColHospTST01 = ko.observable("");
//    self.ColUCITST01 = ko.observable("");
//    self.ColFalleTST01 = ko.observable("");

//    self.ColETIFST = ko.observable("");
//    self.ColETIMST = ko.observable("");

//    self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
//        if ($("#" + FieldDate).val() != "") {
//            var date_ew = new Date($("#" + FieldDate).datepicker('getDate', {
//                dateFormat: date_format_DatePicker
//            }));
//            var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
//            var weekno = moment(date_ew).week();
//            var weeknoISO = moment(date_ew).isoWeek();

//            if (date_ew == null) {

//                FieldAct(null);
//                FieldActYear(null);

//            } else {

//                if (fwky_date > 3) {
//                    var month = 11, day = 31;
//                    var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

//                    if (weekno == 1 && moment(date_ew).month() == 0) {
//                        var fwky_date_ant = new Date(moment(date_ew).year() - 1, 0, 1).getDay();
//                        var fwdoyant = moment(end_date_year_ant).isoWeek();
//                        if (fwky_date_ant > 3) {

//                            FieldAct(fwdoyant - 1);

//                        } else {

//                            if (weekno == 1 && moment(date_ew).month() == 0 && fwky_date_ant <= 3) {
//                                FieldAct(53);
//                                fwdoyant = 53;
//                            }
//                            else
//                                FieldAct(fwdoyant);
//                        }
//                        if (FieldActYear != "")
//                            if (fwdoyant == 52 || fwdoyant == 53)
//                                FieldActYear(date_ew.getFullYear() - 1);
//                            else
//                                FieldActYear(date_ew.getFullYear());
//                    }
//                    else if (weekno == 1 && moment(date_ew).month() != 0) {
//                        FieldAct(moment(date_ew).isoWeek() - 1);
//                        if (FieldActYear != "")
//                            FieldActYear(date_ew.getFullYear());
//                    }
//                    else {
//                        FieldAct(weekno - 1);
//                        if (FieldActYear != "")
//                            FieldActYear(date_ew.getFullYear());
//                    }
//                } else {
//                    if (weekno == 1 && moment(date_ew).month() == 11) {
//                        var fwky_date_prox = new Date(moment(date_ew).getFullYear() + 1, 0, 1).getDay();

//                        if (fwky_date_prox > 3) {
//                            FieldAct(53);
//                            FieldActYear(date_ew.getFullYear());
//                        } else {
//                            FieldAct(weekno);
//                            FieldActYear(date_ew.getFullYear() + 1);
//                        }
//                    } else {
//                        FieldAct(weekno);
//                        if (FieldActYear != "")
//                            FieldActYear(date_ew.getFullYear());
//                    }
//                }
//            }
//        }
//    };

//    self.HospitalDate.subscribe(function (HospitalDate) {
//        self.ChangeHospitalAndDate();
//        self.CalculateEW("HospitalDate", self.HospitalEW, self.HospitalYE);
//    });

//    self.SummayItems = ko.observableArray([]);
//    self.SummaryForYearItems = ko.observableArray([]);

//    self.EnableCHI = ko.computed(function () {
//        return (self.UsrCountry() != 7) ? true : false;
//    }, self);

//    self.DisableCHI = ko.computed(function () {
//        return (self.UsrCountry() == 7) ? true : false;

//    }, self);

//    self.ActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? true : false;
//    }, self);

//    self.NoActiveBOL = ko.computed(function () {
//        return (self.UsrCountry() == 3) ? false : true;
//    }, self);

//    self.ActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? true : false;
//    }, self);

//    self.NoActiveSUR = ko.computed(function () {
//        return (self.UsrCountry() == 25) ? false : true;
//    }, self);

//    self.ActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? true : true;
//    }, self);

//    self.NoActiveJAM = ko.computed(function () {
//        return (self.UsrCountry() == 17) ? false : true;
//    }, self);

//    self.ActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? true : false;
//    }, self);

//    self.NoActiveCOS = ko.computed(function () {
//        return (self.UsrCountry() == 9) ? false : true;
//    }, self);

//    self.ActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? true : false;
//    }, self);

//    self.NoActiveCAY = ko.computed(function () {
//        return (self.UsrCountry() == 119) ? false : true;
//    }, self);

//    self.PickFirstDay = function (date) {
//        var day = date.getDay();
//        return [day == 0, " "];
//    };

//    self.PickLastDay = function (date) {
//        var day = date.getDay();
//        return [day == 6, " "];
//    };

//    self.AddHospital = function (element, index, array) {
//        self.hospitals.push({ Id: element.Id, Name: element.Name });
//    };

//    self.ReloadHospitals = function () {
//        if (typeof self.selectedCountryId === "undefined") {
//            return;
//        }
//        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
//            self.hospitals(data);
//        })
//        .fail(function (jqXHR, textStatus, errorThrown) {
//            alert(errorThrown);
//        })
//    };

//    self.AddSummayItem = function (element, index, array) {
//        self.SummayItems.push(new SummayItem(element));
//    };

//    self.AddSummaryForYearItems = function (element, index, array) {
//        self.SummaryForYearItems.push(new SummaryYearItem(element));
//    };

//    // Esto es para calcular los totales de las columnas
//    self.ColETIDenoST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.ETIDenoST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColETINumST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.ETINumST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColETINumEmerST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.ETINumEmerST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColHospFST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.HospFem());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColHospMST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.HospMaso());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColHospTST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.HospST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColUCIFST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.UCIFem());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColUCIMST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.UCIMaso());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColUCITST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.UCIST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColFalleFST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.DefFem());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColFalleMST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.DefMaso());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColFalleTST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.DefST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColNeuTST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.NeuST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColIRAGTST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.CCSARIST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    self.ColVentTST = ko.computed(function () {
//        var numberofitems = 0;
//        ko.utils.arrayForEach(self.SummayItems(), function (r) {
//            numberofitems += parseInt(r.VentST());
//        });
//        return parseInt(numberofitems);
//    }, self);

//    // Aquí termina el calculo de los totales

//    self.MakeValuesOfSummayItems = function () {
//        var index;
//        var ValuesOfSummayItems = [];
//        for (index = 0; index < self.SummayItems().length; ++index) {
//            ValuesOfSummayItems.push(self.SummayItems()[index].MakeValueOfSummayItem());
//        };
//        //console.log(ValuesOfSummayItems);
//        return ValuesOfSummayItems;
//    };

//    self.ChangeHospitalAndDate = function () {
//        self.SummayItems.removeAll();
//        $("#TotalSummary").hide();
//        $("#LabelSummary").hide();
//        $("#ButtonSummary").hide();
//    };

//    self.CancelarItems = function () {
//        self.SummayItems.removeAll();

//        if ($.isFunction(self.Id)) {
//            self.Id = ko.observable("");
//            self.Id("");
//        }
//        else {
//            self.Id = "";
//        }

//        self.selectedHospitalId("");
//        self.HospitalDate("");
//        self.HospitalEW("");
//        self.HospitalYE("");
//        $("#TotalSummary").hide();
//        $("#LabelSummary").hide();
//        $("#ButtonSummary").hide();
//    };

//    //self.CalculateEW = function (FieldDate, FieldAct, FieldActYear) {
//    //    if ($("#" + FieldDate).val() != "") {
//    //        var date_ew = moment(date_format_moment, $("#" + FieldDate).val()).toDate();
//    //        var fwky_date = new Date(moment(date_ew).year(), 0, 1).getDay();
//    //        var weekno = moment(date_ew).week();
//    //        var weeknoISO = moment(date_ew).isoWeek();

//    //        if (fwky_date > 3) {
//    //            var month = 11, day = 31;
//    //            var end_date_year_ant = new Date(moment(date_ew).year() - 1, month, day--);

//    //            if (weekno == 1 && moment(date_ew).month() == 0) {
//    //                var fwky_date_ant = new Date(moment(date_ew).year() - 1, 0, 1).getDay();
//    //                var fwdoyant = moment(end_date_year_ant).isoWeek();
//    //                if (fwky_date_ant > 3) {

//    //                    FieldAct(fwdoyant - 1);

//    //                } else {

//    //                    if (weekno == 1 && moment(date_ew).month() == 0 && fwky_date_ant <= 3) {
//    //                        FieldAct(53);
//    //                        fwdoyant = 53;
//    //                    }
//    //                    else
//    //                        FieldAct(fwdoyant);
//    //                }
//    //                if (FieldActYear != "")
//    //                    if (fwdoyant == 52 || fwdoyant == 53)
//    //                        FieldActYear(date_ew.getFullYear() - 1);
//    //                    else
//    //                        FieldActYear(date_ew.getFullYear());
//    //            }
//    //            else if (weekno == 1 && moment(date_ew).month() != 0) {
//    //                FieldAct(moment(date_ew).isoWeek() - 1);
//    //                if (FieldActYear != "")
//    //                    FieldActYear(date_ew.getFullYear());
//    //            }
//    //            else {
//    //                FieldAct(weekno - 1);
//    //                if (FieldActYear != "")
//    //                    FieldActYear(date_ew.getFullYear());
//    //            }
//    //        } else {
//    //            console.log("aqui " + weekno);
//    //            if (weekno == 1 && moment(date_ew).month() == 11) {
//    //                var fwky_date_prox = new Date(moment(date_ew).year() + 1, 0, 1).getDay();

//    //                if (fwky_date_prox > 3) {
//    //                    FieldAct(53);
//    //                    FieldActYear(date_ew.getFullYear());
//    //                } else {
//    //                    FieldAct(weekno);
//    //                    FieldActYear(date_ew.getFullYear() + 1);
//    //                }

//    //            } else {
//    //                FieldAct(weekno);
//    //                if (FieldActYear != "")
//    //                    FieldActYear(date_ew.getFullYear());
//    //            }
//    //        }

//    //    }
//    //};



//    self.GetYearSummaryForYearItems = function () {
//        console.log("->A1");
//        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "") {
//            //$.postJSON(app.dataModel.getSummaryForYearUrl, { hospitalId: self.selectedHospitalId() })
//            $.postJSON(app.dataModel.getSummaryForYearUrlJM, { hospitalId: self.selectedHospitalId() })
//               .success(function (data, textStatus, jqXHR) {
//                   $("#LabelBandeja").show();
//                   $("#TotalBandeja").show();
//                   self.SummaryForYearItems([]);
//                   data.forEach(self.AddSummaryForYearItems);
//                   //console.log(data);
//                   //self.SummayItems([]);
//                   //data.forEach(self.AddSummayItem);                             
//               })
//               .fail(function (jqXHR, textStatus, errorThrown) {
//                   alert(errorThrown);
//               })
//        }
//    };

//    self.GetSummaryExcel = function () {
//        //var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate()).format(date_format_moment) : null, EndDate: self.EndDate() ? moment(self.EndDate()).format(date_format_moment) : null, ReportCountry: self.selectedReportCountryId() }
//        //if (self.validate() == true)
//        var namevalues = { hospitalId: self.selectedHospitalId(), hospitalDate: moment(self.HospitalDate()).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() };
//        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
//            window.open(app.dataModel.getSummayDetailsExcel + "?" + $.param(namevalues, true), "_blank");
//        } else {
//            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
//                //alert("Seleccionar un establecimiento es requerido");
//                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
//                    alert("Hospital is required");
//                else
//                    alert("Seleccionar un establecimiento es requerido");
//            }

//            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
//                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
//                    alert("Date is required");
//                else
//                    alert("La fecha es requerida");
//            }
//        }
//    }

//    self.GetSummayItems = function () {
//        if ((typeof self.selectedHospitalId() != "undefined") && self.selectedHospitalId() != "" && (typeof self.HospitalDate() != "undefined") && self.HospitalDate() != "") {
//            $.postJSON(app.dataModel.getSummayDetailsUrl, { hospitalId: self.selectedHospitalId(), hospitalDate: moment(self.HospitalDate()).format(date_format_ISO), EpiWeek: self.HospitalEW(), EpiYear: self.HospitalYE() })
//            .success(function (data, textStatus, jqXHR) {
//                self.SummayItems([]);
//                data.forEach(self.AddSummayItem);
//                $("#LabelSummary").show();
//                $("#TotalSummary").show();
//                $("#ButtonSummary").show();
//            })
//            .fail(function (jqXHR, textStatus, errorThrown) {
//                alert(errorThrown);
//            })
//        } else {
//            if ((typeof self.selectedHospitalId() == "undefined") || self.selectedHospitalId() == "") {
//                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
//                    alert("Hospital is required");
//                else
//                    alert("Seleccionar un establecimiento es requerido");
//            }

//            if ((typeof self.HospitalDate() == "undefined") || self.HospitalDate() == "") {
//                if (self.UsrCountry() == 25 || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
//                    alert("Date is required");
//                else
//                    alert("La fecha es requerida");
//            }
//        }
//    };

//    self.SaveSummayItems = function () {
//        //console.log(self.MakeValuesOfSummayItems());
//        $.ajax({
//            type: 'POST',
//            dataType: 'json',
//            url: app.dataModel.saveSummayDetailsUrl,
//            data: JSON.stringify({ casesummaryDetails: self.MakeValuesOfSummayItems() }),
//            contentType: 'application/json; charset=utf-8',
//            success: function (data, textStatus, jqXHR) {
//                if ((self.UsrCountry() == 25) || (self.UsrCountry() == 17) || (self.UsrCountry() == 119))
//                    alert("Data saved...");
//                    ///////alert(msgSavedData);
//                else
//                    alert(data);
//                self.CancelarItems();
//            },
//        })
//    };

//    return self;
//};

//app.addViewModel({
//    name: "SummaryJM",
//    bindingMemberName: "summaryJM",
//    factory: SummaryJMViewModel
//});

///** Bandeja de denominadores **/
//function showEpiWeek(data, event) {
//    console.log(data.StartDateOfWeek);
//    var date = moment.unix(data.StartDateOfWeek).utc().format(moment_date_format);
//    console.log(date);
//    $("#HospitalDate").val(date);
//    $("#HospitalDate").change();
//    $("#search").click();
//}
///** Fin Bandeja de denominadores **/