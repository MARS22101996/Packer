$(".pagination")
        .on("click",
            "a[data-type='paging']",
            function () {
                $("[name=Page]").val($(this).attr("data-addr"));
                $("#filter-form").submit();
            });