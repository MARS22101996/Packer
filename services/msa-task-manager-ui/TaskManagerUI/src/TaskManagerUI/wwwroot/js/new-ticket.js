var assignEditTicket = (function ($, mustache) {
    var selectors = {
        new: '[data-ticket-new]',
        template: '[data-ticket-new-template]',
        container: '[data-ticket-new-container]',
        selectionTemplate: '[data-ticket-new-selection-template]',
        selectionContainer: '[data-ticket-new-selection-container]',
        addTag: '[data-ticket-new-add-tag]',
        addAssignee: '[data-ticket-new-add-assignee]',
        addLinked: '[data-ticket-new-add-linked]',
        removeTag: '[data-ticket-new-remove-tag]',
        removeAssignee: '[data-ticket-new-remove-assignee]',
        removeLinked: '[data-ticket-new-remove-linked]',
        nameValidation: '[data-ticket-new-name-validation]',
        modalId: '#modal-ticket-new',
        newTagVal: '#new-ticket-tag',
        newAssigneeVal: '#new-ticket-assignee',
        newLinkedVal: '#new-ticket-linked',
        addTicket: '#ticket-add',
        addTicketName: '#ticket-add-name',
        addTicketDescription: '#ticket-add-description',
        addTicketPriority: '#ticket-add-priority',
        selectedTeamId: '#selected-team-id',
        form: 'form[name=\'save-ticket\']'
    };

    var attributes = {
        url: 'data-url',
        teamId: 'data-ticket-new-team-id',
        ticketId: 'data-ticket-update-ticket-id'
    };

    var config = {
        teamId: null,
        ticketNewUrl: null,
        updateTicketId: null
    };

    var data = {};

    $(function () {
        $(selectors.new).on('click', showTicketNew);
    });

    function assignEditClickEvent() {
        $(selectors.new).off();
        $(selectors.new).on('click', showTicketNew);
    }

    function showTicketNew(event) {
        event.preventDefault();
        config.teamId = $(this).attr(attributes.teamId);
        config.updateTicketId = $(this).attr(attributes.ticketId);
        config.ticketNewUrl = $(this).attr(attributes.url);

        $.ajax({
                url: config.ticketNewUrl,
                type: 'GET',
                data: {
                    teamId: config.teamId,
                    ticketId: config.updateTicketId
                }
            })
            .success(onTicketAddDataReceived)
            .fail(error => { console.log(error) });
    }

    function onTicketAddDataReceived(jsonData) {
        if (jsonData != null) {
            data = jsonData;
            generateModal(jsonData);
            $(selectors.modalId).modal('show');
            $(selectors.addTicket).on('click', addTicket);

            $(selectors.addTicketDescription).val(jsonData.text);
            $(selectors.addTicketName).val(jsonData.name);
        }
    }

    function generateModal(jsonData) {
        var template = $(selectors.template).html();
        var container = $(selectors.container);
        var priority = jsonData.priority;
        if (priority === 0) {
            priority = 'Low';
        }

        jsonData.options =
        [
            { val: 1, txt: 'Low', sel: priority === 'Low' },
            { val: 2, txt: 'Medium', sel: priority === 'Medium' },
            { val: 3, txt: 'High', sel: priority === 'High'},
            { val: 4, txt: 'Critical', sel: priority === 'Critical' }
            ];
        jsonData.edit = config.updateTicketId;
        var mustacheToHtml = mustache.to_html(template, jsonData);

        container.html(mustacheToHtml);
        updateSelection();
    }

    function addTag() {
        var tag = $(selectors.newTagVal).val();
        if ($.inArray(tag, data.tags) === -1 && tag) {
            data.tags.push($(selectors.newTagVal).val());
        }
        updateSelection();
    }

    function removeTag() {
        data.tags.splice($.inArray($(this).attr('tag'), data.tags), 1);
        updateSelection();
    }

    function addAssignee() {
        var assignees = $.grep(data.assignees, function (assignee) {
            return assignee.id === $(selectors.newAssigneeVal).val();
        });
        data.assignee = assignees[0];
        updateSelection();
    }

    function removeAssignee() {
        data.assignee = null;
        updateSelection();
    }

    function addLinked() {
        if ($(selectors.newLinkedVal).val()) {
            var ticket = $.grep(data.allTickets,
                function(t) {
                    return t.id === $(selectors.newLinkedVal).val();
                });
            if ($.inArray(ticket[0], data.linkedTickets) === -1) {
                data.linkedTickets.push(ticket[0]);
            }
            updateSelection();
        }
    }

    function removeLinked() {
        var id = $(this).attr('ticket');
        var ticket = $.grep(data.linkedTickets, function (t) {
            return t.id === id;
        });
        data.linkedTickets.splice($.inArray(ticket[0], data.linkedTickets), 1);
        updateSelection();
    }

    function updateSelection() {
        var template = $(selectors.selectionTemplate).html();
        var container = $(selectors.selectionContainer);
        if (data.tags.length) {
            data.tagsExist = true;
        }
        if (data.assignee) {
            data.assigneeExist = true;
        }
        if (data.linkedTickets.length) {
            data.linkedExist = true;
        }
        var mustacheToHtml = mustache.to_html(template, data);

        container.html(mustacheToHtml);
        $(selectors.addTag).on('click', addTag);
        $(selectors.addAssignee).on('click', addAssignee);
        $(selectors.addLinked).on('click', addLinked);

        $(selectors.removeTag).on('click', removeTag);
        $(selectors.removeAssignee).on('click', removeAssignee);
        $(selectors.removeLinked).on('click', removeLinked);
    }

    function addTicket() {
        $(selectors.form).validate({
            rules: {
                ticketName: {
                    required: true
                },
                ticketDescription: {
                    required: true
                }
            },
            messages: {
                ticketName: {
                    required: "Please enter ticket name"
                },
                ticketDescription: {
                    required: "Please enter ticket description"
                }
            },
            submitHandler: submitAddTicket
        });
    }

    function submitAddTicket() {
        var teamId = $(selectors.selectedTeamId).html();
        if (teamId) {
            config.teamId = teamId;
        }
        var model = {
            Text: $(selectors.addTicketDescription).val(),
            Name: $(selectors.addTicketName).val(),
            Priority: $(selectors.addTicketPriority).val(),
            Tags: data.tags,
            Assignee: data.assignee,
            LinkedTickets: data.linkedTickets,
            TeamId: config.teamId,
            Status: data.status,
            Id: config.updateTicketId
        };

        $.ajax({
            type: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            url: config.ticketNewUrl,
            data: JSON.stringify(model),
            dataType: 'json'
            })
            .success(onAddComplete);
    }

    function onAddComplete() {
        $(selectors.modalId).modal('hide');
        refreshTicketBoard();
    }

    return assignEditClickEvent;
})(jQuery, Mustache);