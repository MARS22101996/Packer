$(function() {
    $("[data-add-tag-btn]")
        .click(function() {
            $(".tag")
                .each(function(i, item) {
                    if ($(item).parents(".tags-input").length) {
                        $("<input>")
                            .attr({
                                type: "hidden",
                                name: "Tags",
                                value: item.textContent
                            })
                            .appendTo("form[data-createTicketForm]");
                    }
                });
        });
});

var data = JSON.stringify($(".tagsinput-alltags").tagsinput("items").itemsArray);
var alltags = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace("name"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: $.map($.parseJSON(data),
        function(tag) {
            return {
                name: tag
            };
        })
});
alltags.initialize();

$("[data-tagInput]")
    .tagsinput({
        typeaheadjs: {
            name: "alltags",
            displayKey: "name",
            valueKey: "name",
            source: alltags.ttAdapter()
        }
    });

$(".twitter-typeahead").css("display", "inline");