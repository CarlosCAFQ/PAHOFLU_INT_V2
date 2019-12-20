$(function () {
    console.log("app.initialize");
    app.initialize();

    

    //var vm = {
    //    //firstName: ko.observable(),
    //    //people: ko.observableArray(),
    //    //addPerson: function () {
    //    //    vm.people.push({ firstName: vm.firstName() });
    //    //    vm.firstName('');
    //    //}
    //};

    // Activate Knockout
    ko.validation.init({ grouping: { observable: false } });
    ko.applyBindings(app);
});
