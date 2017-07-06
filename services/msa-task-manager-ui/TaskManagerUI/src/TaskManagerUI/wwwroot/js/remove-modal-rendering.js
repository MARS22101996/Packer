var assignTicketDelete = (function ($, mustache) {
    var selectors = {
        deleteTicketLink: '[delete-ticket]',
        template: '[remove-ticket-modal]',
        container: '[remove-modal-data]',
        modalId: '#modal-remove'
    }

    var attributes = {
        teamId: 'data-team-remove-id',
        ticketName: 'data-ticket-remove-name',
        ticketId: 'data-ticket-remove-id',
        url: 'data-url'
    }

    var model = {
        ticket: {
            teamId: null,
            ticketId: null,
            ticketName: null
        },
        ticketDeleteUrl: null
    }

    $(function() {
        $(selectors.deleteTicketLink).on('click', showRemoveTicketModal);
    });

    function assignClickEvent() {
        $(selectors.deleteTicketLink).on('click', showRemoveTicketModal);
    }

    function showRemoveTicketModal() {
        model.ticketDeleteUrl = $(this).attr(attributes.url);
        model.ticket.ticketId = $(this).attr(attributes.ticketId);
        model.ticket.ticketName = $(this).attr(attributes.ticketName);
        model.ticket.teamId = $(this).attr(attributes.teamId);

        generateModal(model);
        $(selectors.modalId).modal('show');
    }

    function generateModal(model) {
        var deleteModalTemplate = $(selectors.template).html();
        var mustacheToHtml = mustache.to_html(deleteModalTemplate, model);
        $(selectors.container).html(mustacheToHtml);
    }

    return assignClickEvent;

})(jQuery, Mustache);