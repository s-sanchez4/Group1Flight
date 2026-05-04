// 1. Define the logic with the maxYears parameter
$.validator.addMethod("futuredate", function (value, element, params) {
    if (!value) return true; 

    var inputDate = new Date(value);
    var today = new Date();
    
    // Get the max years from the HTML attribute injected by IClientModelValidator
    var maxYears = $(element).attr("data-val-futuredate-maxyears") || 3;
    
    var maxDate = new Date();
    maxDate.setFullYear(today.getFullYear() + parseInt(maxYears));
    
    // Check if it's in the future AND not beyond the max limit
    return inputDate >= today.setHours(0,0,0,0) && inputDate <= maxDate;
});

// 2. Add the adapter (simpler version)
$.validator.unobtrusive.adapters.addBool("futuredate");

// 3. FORCE immediate validation (Keep your existing logic)
$(document).ready(function () {
    $(document).on("blur change", 'input[data-val-futuredate]', function () {
        $(this).valid(); 
    });
});