
function PrintTestViewModel(app, dataModel) {
	var self = this;
	//self.selectedCountryId = ko.observable("");
	//self.selectedInstitutionId = ko.observable("");
	self.Report = ko.observable("ExportLab");
	self.Year = ko.observable(new Date().getFullYear());
	self.Month = ko.observable("");
	self.SE = ko.observable("");
	self.StartDate = ko.observable(null);
	self.EndDate = ko.observable(null);

	self.RendertoPDF = function (UrlPDF) {
	    var pdf = document.getElementById('embed_PDF');
	    var xhr = new XMLHttpRequest();
        pdf.src = "";
	    xhr.onreadystatechange = function () {
	        if (this.readyState == 4 && this.status == 200) {
	            //console.log(this.response, typeof this.response);
	            //pdf = document.getElementById('embed_PDF');
	            var url = window.URL || window.webkitURL;
	            pdf.src = url.createObjectURL(this.response);
	        }
	    }
	    xhr.open('GET', UrlPDF);
	    xhr.responseType = 'blob';
	    xhr.send();
	    $('.bs-example-modal-lg').modal('toggle');


	}


	self.Generate = function (SampleNumber, data ) {
	    console.log(app.Views.Home.selectedInstitutionId());
	    var namevalues = { Report: SampleNumber == "record" ? "Cases" : "ExportLab", CountryID: app.Views.Home.selectedCountryId() ? app.Views.Home.selectedCountryId() : app.Views.Home.UsrCountry(),  InstitutionID: app.Views.Home.selectedInstitutionId() > 0  ? app.Views.Home.selectedInstitutionId() :  SampleNumber == "record" ? "" : app.Views.Lab.LabsResult()[0].Id,  RecordID: app.Views.Contact.Id(), NumSample: SampleNumber > 0 ? SampleNumber : 1 }

	    if (SampleNumber == "record") {
            self.RendertoPDF(app.dataModel.getPrintTest + "?" + $.param(namevalues, true));

	    } else if (app.Views.Lab.validate()) {
	        app.Views.Lab.SaveLab();
	        console.log(app.dataModel.getPrintTest + "?" + $.param(namevalues, true))
	        window.open(app.dataModel.getPrintTest + "?" + $.param(namevalues, true), "_blank");
	    }
	    
	};

	self.GeneratebyList = function (record_id) {
	    console.log(app.Views.Home.selectedInstitutionId());
	    var namevalues = { Report: "Cases", CountryID: app.Views.Home.selectedCountryId() ? app.Views.Home.selectedCountryId() : app.Views.Home.UsrCountry(), InstitutionID: "" , RecordID: record_id, NumSample: "" }

	        self.RendertoPDF(app.dataModel.getPrintTest + "?" + $.param(namevalues, true));

	};
};

app.addViewModel({
	name: "PrintTest",
	bindingMemberName: "printtest",
	factory: PrintTestViewModel
});