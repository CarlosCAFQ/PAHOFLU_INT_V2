
function TicketViewModel(app, dataModel) {
    var self = this;
    self.Id = "";
    self.UsrCountry = ko.observable(selcty); // Pais del usuario logueado
    self.ticketSubject = ko.observable("");
    self.ticketMsg = ko.observable("");
    self.ticketStatus = ko.observable("");
    self.ticketPriority = ko.observable("");
    self.ticketID = ko.observable("");    

    self.SendTicket = function () {        
        if ((typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "") {
            
            var formData = new FormData();
            
           
            $('#attachmentLoader').show();
            $('#newTicket').prop('disabled', true);
            formData.append("FileUpload", $('#ticketFile')[0].files[0]);
            formData.append("FileUpload", $('#ticketFile2')[0].files[0]);
            formData.append("FileUpload", $('#ticketFile3')[0].files[0]);            

            formData.append("UsrCountry", self.UsrCountry());
            formData.append("ticketSubject", self.ticketSubject());
            formData.append("ticketMsg", self.ticketMsg());
            console.log(formData);
            
            $.ajax({
                url: app.dataModel.createTicketUrl,
                type: 'POST',
                async:true,
                data: formData,
                processData: false,
                contentType: false,// not json               
                complete: function (data) {
                    console.log(data);
                    $('#attachmentLoader').css('display', 'none');
                    if (data.responseText == "1") {
                        alert("Ticket ingresado exitósamente");
                        CleanForm();
                        GetTicketList();
                    }
                    else {
                        alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    }

                },
                error: function (response) {
                    console.log(response.responseText);
                    $('#newTicket').prop('disabled', true);
                    alert(response.responseText);
                    alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    $('#SubjectTicket').val('');
                    $('#DescriptionTicket').val('');
                }
            });
            
        }
        else {            
            alert(msgTicketMissingValues);
        }
    };

    self.EditTicket = function () {
        var formData = new FormData();        
        if ((typeof self.ticketID() != "undefined") && self.ticketID() != "" && (typeof self.ticketSubject() != "undefined") && self.ticketSubject() != "" && (typeof self.ticketMsg() != "undefined") && self.ticketMsg() != "" && (typeof self.ticketStatus() != "undefined") && self.ticketStatus() != "0" && (typeof self.ticketPriority() != "undefined") && self.ticketPriority() != "0") {
            $('#attachmentLoader').show();
            $('#newTicket').prop('disabled', true);
            $('#editTicket').prop('disabled', true);            
            formData.append("UsrCountry", self.UsrCountry());
            formData.append("ticketSubject", self.ticketSubject());
            formData.append("ticketMsg", self.ticketMsg());
            formData.append("ticketStatus", self.ticketStatus());
            formData.append("ticketPriority", self.ticketPriority());
            formData.append("ticketID", self.ticketID());
            console.log(formData);

            $.ajax({
                url: app.dataModel.editTicketUrl,
                type: 'POST',
                async: true,
                data: formData,
                processData: false,
                contentType: false,// not json               
                complete: function (data) {
                    console.log(data);                    
                    $('#attachmentLoader').css('display', 'none');
                    if (data.responseText == "1") {
                        alert("Ticket actualizado");                        

                        CleanForm();
                        GetTicketList();
                        
                    }
                    else {
                        alert("El ticket no se pudo ingresar, por favor, envíe un correo electrónico al administrador del sistema");
                    }

                },
                error: function (response) {
                    console.log(response.responseText);
                    $('#newTicket').prop('disabled', true);
                    alert(response.responseText);
                    alert("El ticket no se pudo actualizar, por favor, envíe un correo electrónico al administrador del sistema");
                    $('#SubjectTicket').val('');
                    $('#DescriptionTicket').val('');
                }
            });            
        }
        else {            
            alert(msgTicketMissingValues); alert("v");
        }
    };



    return self;
};


app.addViewModel({
    name: "Ticket",
    bindingMemberName: "ticket",
    factory: TicketViewModel
});


