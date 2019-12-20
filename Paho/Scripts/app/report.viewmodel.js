function ReportViewModel(app, dataModel) {
    var self = this;
    self.selectedCountryId = ko.observable("");
    self.countries = ko.observableArray(countries);
    //self.hospitals = ko.observableArray(hospitals);
    //self.labs = ko.observableArray(labs);
    self.displayFilters = ko.observable(true);
    self.CountryID = ko.observable("");
    self.HospitalID = ko.observable("");
    self.LabID = ko.observable("");
    self.Name = ko.observable("");
    self.NationalID = ko.observable("");
    self.CaseStatus= ko.observable("");
    self.RStartDate = ko.observable(null);
    self.REndDate = ko.observable(null);
    self.HStartDate = ko.observable(null);
    self.HEndDate = ko.observable(null);
    self.Flucases = ko.observableArray([]);
    self.activecountries = ko.computed(function () {
        return $.grep(self.countries(), function (v) {
            return v.Active === true;
        });
    });
    self.ReloadInstitutions = function () {
        if (typeof self.CountryID() === "undefined") {
            return;
        }
        self.loadInstitutions();
        $("#HospitalsGroup").show();
    };
    self.loadInstitutions = function () {
        $.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
            self.hospitals(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
    };
    self.GetFlucases = function () {
        //alert("aqui");
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: app.dataModel.getFlucasesUrl,
            data: JSON.stringify({
                    Name: self.Name(),
                    NationalID: self.NationalID(),
                    CaseStatus: self.CaseStatus(),
                    HospitalID: self.HospitalID(),            
                    RStartDate: self.RStartDate(),
                    REndDate: self.REndDate(),
                    HStartDate: self.HStartDate(),
                    HEndDate: self.HEndDate(),
                    CountryID: "1"
            }),
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {               
                if (data.length > 0) {
                    self.Flucases(data);
                    $("#flucases").show();
                }
            },
        })
         .fail(function (jqXHR, textStatus, errorThrown) {
             alert(errorThrown);
         })
    };
 };

app.addViewModel({
    name: "Report",
    bindingMemberName: "report",
    factory: ReportViewModel
});