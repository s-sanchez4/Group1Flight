// 1. Define the logic
$.validator.addMethod("futuredate", function (value, element) {
    if (!value) return true;
    var inputDate = new Date(value);
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    return inputDate >= today;
});

// 2. Add the adapter
$.validator.unobtrusive.adapters.add("futuredate", [], function (options) {
    options.rules["futuredate"] = true;
    options.messages["futuredate"] = options.message;
});

// 3. FORCE immediate validation on change/blur
$(document).ready(function () {
    // This targets the specific input by its data attribute
    $(document).on("blur change", 'input[data-val-futuredate]', function () {
        $(this).valid(); // This forces the red error to toggle immediately
    });
});