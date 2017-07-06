(function($) {

    var selectors = {
        allUsersUrl: '#all-users-source-url',
        userInput: '#user-input'
    };

    $(function() {
        $.ajax({
            url: $(selectors.allUsersUrl).val(),
            type: 'GET',
            datatype: 'html',
            success: setAutoComplete
        });
    });

    function setAutoComplete(jsonData) {

        var substringMatcher = function (strs) {
            return function findMatches(q, cb) {
                var matches = [];
                substrRegex = new RegExp(q, 'i');

                $.each(strs, function (i, str) {
                    if (substrRegex.test(str)) {
                        matches.push(str);
                    }
                });
                cb(matches);
            };
        };

        $('#user-input .typeahead').typeahead({
                hint: true,
                highlight: true,
                minLength: 1
            },
            {
                name: 'emails',
                source: substringMatcher(jsonData)
            });
    };
})(jQuery);