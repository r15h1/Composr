/*code for cookbook theme autocomplete*/

$('#q').autocomplete({
    paramName: 'q',
    minChars: 3,
    serviceUrl: '/api/autocomplete',
    onSelect: function (suggestion) {
        //alert('You selected: ' + suggestion.value + ', ' + suggestion.data);
        window.location.href = suggestion.data;
    },
    formatResult: function (suggestion, currentValue) {
        if (suggestion.value == "Display all results")
            return '<a href="#">' + $.Autocomplete.formatResult(suggestion, currentValue) + '</a>';

        return $.Autocomplete.formatResult(suggestion, currentValue);
    }
});

$("body").keypress(function () {
    $("#q").focus();
});

$("#SearchFocusLink").click(function () {
    $("#q").focus();
});