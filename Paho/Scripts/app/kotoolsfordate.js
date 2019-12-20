ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var $el = $(element);

        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || { changeYear: true, dateFormat: "dd/mm/yy" };
        $el.datepicker(options);

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {     
            var observable = valueAccessor();
            console.log("start");
            observable($el.datepicker("getDate"));
            console.log(observable);
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $el.datepicker("destroy");
        });

    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            $el = $(element),
            current = $el.datepicker("getDate");

        if (value - current !== 0) {
            $el.datepicker("setDate", value);
        }
    }
};

ko.bindingHandlers.numericValue = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var underlyingObservable = valueAccessor();
        console.log(underlyingObservable);
        var interceptor = ko.dependentObservable({
            read: ko.utils.unwrapObservable(underlyingObservable),
            write: function (value) {
                if (!isNaN(value)) {
                    console.log(value);
                    underlyingObservable(parseInt(value));
                }
            }
        });
        ko.bindingHandlers.value.init(element, function () {
            return
            interceptor
        }, allBindingsAccessor);
    },
    update: ko.bindingHandlers.value.update
};

ko.extenders.required = function (target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    //define a function to do validation
    function validate(newValue) {
        target.hasError(newValue ? false : true);
        target.validationMessage(newValue ? "" : overrideMessage || "Este campo es requerido");
    }

    //initial validation
    validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};