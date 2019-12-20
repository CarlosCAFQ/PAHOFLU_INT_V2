
function FluidViewModel(app, dataModel) {
    var self = this;
    self.selectedCountryId =  ko.observable("");
    self.countries = ko.observableArray(countries);
    self.institutions = ko.observableArray(institutions);
    self.selectedInstitutionId = ko.observable("");
    self.activecountries = ko.computed(function () {
        return $.grep(self.countries(), function (v) {
            return v.Active === true;
        });
    });
    self.ReloadInstitutions = function () {
        if (typeof self.selectedCountryId() === "undefined") {
            return;
        }
        self.loadInstitutions();
        $("#HospitalsGroup").show();
    };
    self.loadInstitutions = function () {
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() }, function(data, status) {
                self.institutions(data);
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            });
    };
    self.Report = ko.observable("Cases");
    self.Year =  ko.observable("");
    self.WeekFrom =  ko.observable("");
    self.WeekTo =  ko.observable("");
    self.StartDate = ko.observable(null);
    self.EndDate = ko.observable(null);

    self.YearFrom = ko.observable(null);
    self.YearTo = ko.observable(null);

    self.validate = function (nextStep) {
        var msg = "";
        var selectCountryUsr = self.selectedCountryId() ? self.selectedCountryId() : CountryID

        if ($("#Hospitals").children().length == 2 && self.selectedInstitutionId() == null)
            msg += "\n" + msgValidationInstitutionRequired;

        if (self.Year() == "" && self.YearFrom() == "" && self.YearTo() == "")
            msg += "\n" + msgValidationFluidDate

        if (self.YearFrom() == "" && self.YearTo() != "")
            msg += "\n" + msgValidationFluidYearFrom

        if (self.YearFrom() != "" && self.YearTo() == "")
            msg += "\n" + msgValidationFluidYearTo

        if (msg !== "") { alert('Report FLUID:' + msg); return false; }

        return true;
    };

    self.exportar = function () {

        //if (self.selectedInstitutionId() > 0) {
        var namevalues = { CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), WeekFrom: self.WeekFrom(), WeekTo: self.WeekTo(), YearFrom: self.YearFrom(), YearTo: self.YearTo() }
            if (self.validate() == true)
                window.open(app.dataModel.getFluid + "?" + $.param(namevalues, true), "_blank");
        //} else {
        //    alert(msgValidateInstitution);
        //}


    };

    self.url = ko.computed(function () {
        var namevalues = { CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), WeekFrom: self.WeekFrom(), WeekTo: self.WeekTo(), YearFrom: self.YearFrom(), YearTo: self.YearTo() }
        return app.dataModel.getFluid + "?" + $.param(namevalues, true);
    });
 };

app.addViewModel({
    name: "Fluid",
    bindingMemberName: "fluid",
    factory: FluidViewModel
});