var refreshTicketBoard = (function ($, moustache) {
    var selectors = {
        swapTeamButton: '[swap-team-button]',
        template: '[ticket-card-template]',
        newContainer: '[new-container]',
        inProgessContainer: '[in-progress-container]',
        doneContainer: '[done-container]',
        selectedTeamId: '#selected-team-id',
        swapTeamUrl: '#swap-team-url',
        selectedTeamName: '#selectedTeamName'
    };

    var attributes = {
        ticketDetailsUrl: 'ticket-details-url',
        ticketUpdateUrl: 'update-ticket-url',
        addCommentUrl: 'add-ticket-comment-url',
        removeCommentUrl: 'remove-ticket-comment-url',
        deleteTicketUrl: 'delete-ticket-url',
        editTicketStatusUrl: 'data-ticket-edit-status-url',
        teamId: 'data-team-id'
    };

    var config = {
        swapTeamUrl: null,
        swapTeamName: null
    };

    var data = {
        tickets: null,
        planned: false,
        inProgress: false,
        done: false,
        ticketDetailsUrl: null,
        addCommentUrl: null,
        removeCommentUrl: null,
        deleteTicketUrl: null,
        editStatusUrl: null,
        ticketUpdateUrl: null
    };

    $(function () {
        config.swapTeamUrl = $(selectors.swapTeamUrl).html();
        $(selectors.swapTeamButton).on('click', swapTeam);
    });

    function swapTeam() {
        var teamId = $(selectors.selectedTeamId).html();
        config.swapTeamName = $(selectors.selectedTeamName).html();
        if (this.localName === 'a') {
            config.swapTeamName = $(this).html();
            teamId = $(this).attr(attributes.teamId);
        }

        $('#ticketSearchSelectedTeamId').val(teamId);

        $.ajax({
            url: config.swapTeamUrl,
            data: {
                teamId: teamId
            },
            type: 'GET'

            })
            .success(displayTicketsForTeam)
            .fail(error => { console.log(error) });
    }

    function displayTicketsForTeam(jsonData) {
        if (jsonData != null) {
            resetContainers();
            $(selectors.selectedTeamId).html(jsonData.selectedTeam.id);
            var ticketTemplate = $(selectors.template).html();
            var newContainer = $(selectors.newContainer);
            var inProgressContainer = $(selectors.inProgessContainer);
            var doneContainer = $(selectors.doneContainer);

            data.ticketDetailsUrl = $(selectors.swapTeamButton).attr(attributes.ticketDetailsUrl);
            data.addCommentUrl = $(selectors.swapTeamButton).attr(attributes.addCommentUrl);
            data.removeCommentUrl = $(selectors.swapTeamButton).attr(attributes.removeCommentUrl);
            data.deleteTicketUrl = $(selectors.swapTeamButton).attr(attributes.deleteTicketUrl);
            data.editStatusUrl = $(selectors.swapTeamButton).attr(attributes.editTicketStatusUrl);
            data.ticketUpdateUrl = $(selectors.swapTeamButton).attr(attributes.ticketUpdateUrl);

            data.tickets = jsonData.selectedTeamTickets.plannedTasks;
            if (data.tickets.length !== 0) {
                data.planned = true;
                var plannedTicketsMustacheToHtml = moustache.to_html(ticketTemplate, data);
                newContainer.html(plannedTicketsMustacheToHtml);
                resetDataPriority();
            }

            data.tickets = jsonData.selectedTeamTickets.progressTasks;
            if (data.tickets.length !== 0) {
                data.inProgress = true;
                var inProgressTicketsMustacheToHtml = moustache.to_html(ticketTemplate, data);
                inProgressContainer.html(inProgressTicketsMustacheToHtml);
                resetDataPriority();
            }

            data.tickets = jsonData.selectedTeamTickets.doneTasks;
            if (data.tickets.length !== 0) {
                data.done = true;
                var doneTicketsMustacheToHtml = moustache.to_html(ticketTemplate, data);
                doneContainer.html(doneTicketsMustacheToHtml);
                resetDataPriority();
            }

            setTeamList();

            assignTicketDelete();
            assignTicketDetails();
            assignEditTicket();
        }
    }

    function setTeamList() {
        $('#selectedTeamName').html(config.swapTeamName);
    }

    function resetDataPriority() {
        data.planned = false;
        data.inProgress = false;
        data.done = false;
    }

    function resetContainers() {
        var newContainer = $(selectors.newContainer);
        var inProgressContainer = $(selectors.inProgessContainer);
        var doneContainer = $(selectors.doneContainer);

        newContainer.html('');
        inProgressContainer.html('');
        doneContainer.html('');
    }

    return swapTeam;
})(jQuery, Mustache);