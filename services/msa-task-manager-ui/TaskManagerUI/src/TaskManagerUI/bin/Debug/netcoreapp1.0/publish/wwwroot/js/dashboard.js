(function($, google) {
    if (google) {
        google.load("visualization",
            "1.0",
            {
                packages: ["corechart"],
                callback: function() {
                    showStatistic();
                }
            });
    }

    var selectors = {
        swapTeamButton: '[swap-team-button]',
        dataTeamId:'data-team-id',
        teamId: "#team-id",
        statisticUrl: "#statistic-url",
        statisticStartDate: "#start-date",
        selectedTeamName: '#selected-team-name'
    };
    

    function showStatistic() {
        $('#datepicker').datepicker({
            orientation: 'top auto'
        });

        $(selectors.statisticStartDate).on('change', getStatistic);
        $(selectors.swapTeamButton).on('click', changeTeam);
        getStatistic();
    }

    function changeTeam() {
        $(selectors.teamId).val($(this).attr(selectors.dataTeamId));
        $(selectors.selectedTeamName).html($(this).html());
        getStatistic();
    }

    function getStatistic(event) {
        if (event) {
            event.preventDefault();
        }
        
        var jsonData;

        var sendModel = {
            url: $(selectors.statisticUrl).val(),
            teamId: $(selectors.teamId).val(),
            startDate: $(selectors.statisticStartDate).val()
        };

        $.ajax({
            url: $(selectors.statisticUrl).val(),
            type: 'GET',
            data: {
                teamId: sendModel.teamId,
                startDate: sendModel.startDate
            }
        }).done(onStatisticReceived)
            .fail((error) => console.log(error));
    };

    function onStatisticReceived(jsonString) {
        jsonData = JSON.parse(jsonString.trim('"'));
        jsonData = JSON.parse(jsonData);

        if (jsonData == null) {
            cleanStatistic();
        } else {
            showTicketDates(jsonData);
            showStatuses(jsonData);
            showPriorities(jsonData);
            showTable(jsonData);
        }
    }

    function cleanStatistic() {
        $("[data-chartDate], " +
                "[data-chartStatus], " +
                "[data-chartPriority]")
            .html("");

        $("[data-is-statistic-container]").html("<h2>No data</h2>");
            //.html($("[data-NotFound]").html());
    }

    function showTable(jsonData) {
        var statisticTableTemplateHtml = $("[data-is-table-statistic]").html();
        var statisticTableContainer = $("[data-is-statistic-container]");

        var mustacheToHtml = Mustache.to_html(statisticTableTemplateHtml, jsonData);

        //var mustasheToHtml = (function (statisticTableTemplateHtml, jsonData) {
        //    Mustache.to_html(statisticTableTemplateHtml, jsonData);
        //})(statisticTableTemplateHtml, jsonData);

        statisticTableContainer.html(mustacheToHtml);
    }

    function showTicketDates(jsonData) {
        var data;
        var chart;
        var chartDiv;

        var options = {
            "title": "Report by date",
            "vAxis": { "title": "Count" },
            "hAxis": { "title": "Date" },
            "legend": "none",
            "colors": ["#ff4000"]
        };

        data = new google.visualization.DataTable();
        data.addColumn("string", "Date");
        data.addColumn("number", "Count");
        setData(data, jsonData.DateCountOfTicketsDictionary);
        chartDiv = $("[data-chartDate]")[0];
        chart = new google.visualization.AreaChart(chartDiv);
        chart.draw(data, options);
    }

    function showStatuses(jsonData) {
        var data;
        var chart;
        var chartDiv;

        var options = {
            "title": "Report by status",
            "height": 400,
            "pieHole": 0.4
        };

        data = new google.visualization.DataTable();
        data.addColumn("string", "Status");
        data.addColumn("number", "Count");
        setData(data, jsonData.StatusCountDictionary);
        chartDiv = $("[data-chartStatus]")[0];
        chart = new google.visualization.PieChart(chartDiv);
        chart.draw(data, options);
    }

    function showPriorities(jsonData) {
        var data;
        var chart;
        var chartDiv;

        var options = {
            "title": "Report by priority",
            "height": 400
        };

        data = new google.visualization.DataTable();
        data.addColumn("string", "Priority");
        data.addColumn("number", "Count");
        setData(data, jsonData.PriorityCountDictionary);
        chartDiv = $("[data-chartPriority]")[0];
        chart = new google.visualization.PieChart(chartDiv);
        chart.draw(data, options);
    }

    function setData(data, dictionary) {
        for (var propertyName in dictionary) {
            data.addRow(
                [propertyName, dictionary[propertyName]]
            );
        }
    }
})(jQuery, google);