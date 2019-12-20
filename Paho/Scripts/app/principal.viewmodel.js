
function PrincipalViewModel(app, dataModel) {
	var self = this;
	self.selectedCountryId = ko.observable(CountryID);
	self.countries = ko.observableArray(countries);
	self.institutions = ko.observableArray(institutions);
	self.regions = ko.observableArray(regions);
	self.selectedInstitutionId = ko.observable("");
	self.selectedRegionId = ko.observable("");
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
		$.getJSON(app.dataModel.getInstitutionsUrl, { CountryID: self.selectedCountryId(), RegionID: self.selectedRegionId() }, function (data, status) {
			self.institutions(data);
		})
            .fail(function (jqXHR, textStatus, errorThrown) {
            	alert("loadInstitutions" + " " + errorThrown);
            });
	};
	self.loadRegions = function () {
	    $.getJSON(app.dataModel.getRegionsUrl, { CountryID: self.selectedCountryId() }, function (data, status) {
	        self.regions(data);
	    })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert("loadRegions" + " " + errorThrown);
            });
	};
	self.Report = ko.observable("Cases");
	self.Year = ko.observable(new Date().getFullYear());
	self.Month = ko.observable("");
	self.SE = ko.observable("");
	self.StartDate = ko.observable(null);
	self.EndDate = ko.observable(null);
	self.Generate = function (data_generate, event) {
	    var namevalues = { Report: self.Report(), CountryID: self.selectedCountryId() ? self.selectedCountryId() : CountryID, HospitalID: self.selectedInstitutionId(), Year: self.Year(), Month: self.Month(), SE: self.SE(), StartDate: self.StartDate() ? moment(self.StartDate(), format_date) : null, EndDate: self.EndDate() ? moment(self.EndDate(), format_date) : null }

	    $.getJSON(app.dataModel.getExportarGraphics, namevalues, function (data, status) {
	        alert(data);
	        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                //alert(errorThrown);
                console.log(errorThrown);
            });
	};
};

app.addViewModel({
	name: "Principal",
	bindingMemberName: "principal",
	factory: PrincipalViewModel
});