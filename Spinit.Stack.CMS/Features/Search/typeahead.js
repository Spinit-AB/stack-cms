import 'corejs-typeahead';
var Bloodhound = require('bloodhound-js');

var engine = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
        url: '/Umbraco/Api/Search/Content?query=%QUERY',
        wildcard: '%QUERY'
    }
});

engine.initialize();

$('#search-form input').typeahead({
    hint: true,
    highlight: true,
    minLength: 1,

},
    {
        display: 'name',
        source: engine
    }
).bind('typeahead:select', function (ev, suggestion) {
    window.location.href = suggestion.url;
});;