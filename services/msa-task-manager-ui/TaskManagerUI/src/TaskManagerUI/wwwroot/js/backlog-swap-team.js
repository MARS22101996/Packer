var refreshTicketBoard = (function ($, moustache) {
    var selectors = {
        swapTeamButton: '[swap-team-button]',
        filterTicketsButton: '#apply-filter-tickets-button',
        template: '[backlog-ticket-card-template]',
        container: '#backlogTicketContainer',
        selectedTeamId: '#selected-team-id',
        selectedTeamName: '#selectedTeamName',
        ticketDetailsUrl: '#ticket-details-url',
        deleteTicketUrl: '#delete-ticket-url',
        editTicketUrl: '#update-ticket-url',
        addCommenttUrl: '#add-comment-url',
        deleteCommnetUrl: '#delete-comment-url',
        swapTeamUrl: '#swap-team-url',
        swapTeamId: 'data-team-id',
        filterFormTeamId: '#filter-form-team-id'
    };

    var filterSelectors = {
        dateFrom: '#dateFrom',
        dateTo: '#dateTo',
        selectedStatuses: 'statusCheckbox:checkbox:checked',
        selectedPriorities: 'priorityCheckbox'
    }

    var config = {
        swapTeamUrl: null,
        swapTeamName: null
    };

    var data = {
        tickets: null,
        deleteTicketUrl: null,
        ticketUpdateUrl: null,
        ticketDetailsUrl: null,
        addCommentUrl: null,
        deleteCommentUrl: null
    };

    $(function () {
        config.swapTeamUrl = $(selectors.swapTeamUrl).html();
        $(selectors.swapTeamButton).on('click', swapTeam);
        $(selectors.filterTicketsButton).on('click', filterTickets);
    });

    function swapTeam() {
        var teamId = $(selectors.selectedTeamId).html();
        config.swapTeamName = $(selectors.selectedTeamName).html();
        if (this.localName === 'a') {
            config.swapTeamName = $(this).html();
            teamId = $(this).attr(selectors.swapTeamId);
        }

        $.ajax({
            url: config.swapTeamUrl,
            type: 'GET',
            data: {
                teamId: teamId
            }
            })
            .success(displayTicketsForTeam)
            .fail(error => { console.log(error) });
    }

    function filterTickets() {
        var teamId = $(selectors.selectedTeamId).html();
        var checkedStatuses = new Array();
        $('input:checkbox.statusCheckbox').each(function () {
            if (this.checked) {
                checkedStatuses.push($(this).val());
            }
        });

        var checkedPriorities = new Array();
        $('input:checkbox.priorityCheckbox').each(function () {
            if (this.checked) {
                checkedPriorities.push($(this).val());
            }
        });

        $.ajax({
            url: config.swapTeamUrl,
            type: 'GET',
            dataType: 'json',
            traditional: true,
            data: {
                DateFrom: $(filterSelectors.dateFrom).val(),
                DateTo: $(filterSelectors.dateTo).val(),
                TeamId: teamId,
                SelectedStatuses: checkedStatuses,
                SelectedPriorities: checkedPriorities,
                teamId: teamId
            }
            })
            .success(filterTicketsForTeam)
            .fail(error => { console.log(error) });
    };

    function filterTicketsForTeam(jsonData) {
        if (jsonData != null) {
            resetContainer();

            var ticketTemplate = $(selectors.template).html();
            var container = $(selectors.container);

            data.ticketDetailsUrl = $(selectors.ticketDetailsUrl).html();
            data.deleteTicketUrl = $(selectors.deleteTicketUrl).html();
            data.ticketUpdateUrl = $(selectors.editTicketUrl).html();
            data.addCommentUrl = $(selectors.addCommenttUrl).html();
            data.deleteCommentUrl = $(selectors.deleteCommnetUrl).html();

            data.tickets = jsonData.tickets;
            var ticketsMustacheToHtml = moustache.to_html(ticketTemplate, data);
            container.html(ticketsMustacheToHtml);

            assignTicketDelete();
            assignTicketDetails();
            assignEditTicket();
        }
    }

    function displayTicketsForTeam(jsonData) {
        if (jsonData != null) {
            resetContainer();
            $(selectors.selectedTeamId).html(jsonData.teamId);
            $(selectors.filterFormTeamId).val(jsonData.teamId);

            var ticketTemplate = $(selectors.template).html();
            var container = $(selectors.container);

            data.ticketDetailsUrl = $(selectors.ticketDetailsUrl).html();
            data.deleteTicketUrl = $(selectors.deleteTicketUrl).html();
            data.ticketUpdateUrl = $(selectors.editTicketUrl).html();
            data.addCommentUrl = $(selectors.addCommenttUrl).html();
            data.deleteCommentUrl = $(selectors.deleteCommnetUrl).html();

            data.tickets = jsonData.tickets;
            var ticketsMustacheToHtml = moustache.to_html(ticketTemplate, data);
            container.html(ticketsMustacheToHtml);

            setTeamList();
            assignTicketDelete();
            assignTicketDetails();
            assignEditTicket();
        }
    }

    function setTeamList() {
        $('#selectedTeamName').html(config.swapTeamName);
    }

    function resetContainer() {
        var container = $(selectors.container);
        container.html('');
    }

    return swapTeam;

})(jQuery, Mustache);