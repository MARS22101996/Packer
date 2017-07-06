var assignTicketDetails = (function ($, mustache) {
    var selectors = {
        details: '[data-ticket-details]',
        template: '[data-ticket-details-template]',
        container: '[data-ticket-details-container]',
        addComment: '[data-ticket-details-add-comment]',
        addCommentText: '[data-ticket-details-add-comment-text]',
        removeComment: '[data-ticket-details-comment-remove]',
        commentsTemplate: '[data-ticket-details-comments-template]',
        commentsContainer: '[data-ticket-details-comments-container]',
        modalId: '#modal-ticket-details',
        addCommentForm: 'form[name=\'add-comment\']'
    };

    var attributes = {
        url: 'data-url',
        addComment: 'data-add-comment-url',
        removeComment: 'data-remove-comment-url',
        removeCommentId: 'data-ticket-details-comment-id',
        teamId: 'data-ticket-details-team-id',
        ticketId: 'data-ticket-details-ticket-id'
    }

    var config = {
        addCommentUrl: null,
        removeCommentUrl: null,
        ticketDetailsUrl: null,
        teamId: null,
        ticketId: null
    }

    $(function () {
        $(selectors.details).on('click', showTicketDetails);
    });

    function assignClickEvent(){
        $(selectors.details).on('click', showTicketDetails);
    }

    function showTicketDetails(event) {
        event.preventDefault();
        config.addCommentUrl = $(this).attr(attributes.addComment);
        config.removeCommentUrl = $(this).attr(attributes.removeComment);
        config.ticketDetailsUrl = $(this).attr(attributes.url);
        config.teamId = $(this).attr(attributes.teamId);
        config.ticketId = $(this).attr(attributes.ticketId);

        $.ajax({
              url: config.ticketDetailsUrl,
                type: 'GET',
                data: {
                    teamId: config.teamId,
                    ticketId: config.ticketId
                }
            })
            .done(onTicketDetailsReceived)
            .fail(error => { console.log(error) });
    }

    function onTicketDetailsReceived(jsonData) {
        if (jsonData != null) {
            jsonData.ticket.ticketAssignee = jsonData.ticket.assignee;
            delete jsonData.ticket.assignee;
            generateModal(jsonData);
            updateComments(jsonData);
            $(selectors.modalId).modal('show');
        }
    }

    function generateModal(jsonData) {
        var template = $(selectors.template).html();
        var container = $(selectors.container);
        var mustacheToHtml = mustache.to_html(template, jsonData);

        container.html(mustacheToHtml);
        $(selectors.addComment).on('click', addComment);
    }

    function addComment() {
        $(selectors.addCommentForm).validate({
            rules: {
                comment: {
                    required: true
                }
            },
            messages: {
                comment: {
                    required: "Please enter comment"
                }
            },
            submitHandler: submitAddComment
        });
    }

    function submitAddComment() {
        var model = {
            Text: $(selectors.addCommentText).val(),
            TeamId: config.teamId,
            TicketId: config.ticketId
        };

        $.ajax({
            type: 'POST',
            url: config.addCommentUrl,
            headers: {
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(model)
            })
            .complete(onCommentsChange);
    }

    function onCommentsChange() {
        $.ajax({
                url: config.ticketDetailsUrl,
                type: 'GET',
                data: {
                    teamId: config.teamId,
                    ticketId: config.ticketId
                }
            })
            .done(onCommentsReceived)
            .fail(function (error) { console.log(error) });
    }

    function onCommentsReceived(jsonData) {
        if (jsonData != null) {
            updateComments(jsonData);
        }
    }

    function updateComments(jsonData) {
        var template = $(selectors.commentsTemplate).html();
        var container = $(selectors.commentsContainer);
        var mustacheToHtml = mustache.to_html(template, jsonData);

        container.html(mustacheToHtml);
        $(selectors.addCommentText).val('');
        $(selectors.removeComment).on('click', removeComment);
    }

    function removeComment(event) {
        event.preventDefault();
        $.ajax({
                type: 'GET',
                url: config.removeCommentUrl,
                data: {
                    commentId: $(this).attr(attributes.removeCommentId),
                    teamId: config.teamId,
                    ticketId: config.ticketId
                }
            })
            .complete(onCommentsChange);
    }

    return assignClickEvent;

})(jQuery, Mustache);