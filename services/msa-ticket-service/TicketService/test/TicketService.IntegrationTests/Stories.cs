using TestStack.BDDfy;

namespace TicketService.IntegrationTests
{
    [Story(Title = "User manages ticket links",
       AsA = "True user",
       IWant = "to be able to manage ticket links",
       SoThat = "I can manage ticket links for the better good")]
    public class ManageTicketLink
    {
    }

    [Story(Title = "User gets help tags",
        AsA = "Law-abiding user",
        IWant = "to add tags to ticket while creating a new ticket",
        SoThat = "I can use tips for already created tags")]
    public class UserGetsHelpTags
    {
    }

    [Story(Title = "User receives tickets",
        AsA = "As a busy User",
        IWant = "I want to get all tickets",
        SoThat = "So that I use them for my own purposes")]
    public class UserGetsTickets
    {
    }

    [Story(Title = "User manages tickets",
        AsA = "As a busy user",
        IWant = "I want to manage tickets",
        SoThat = "I can update the board content")]
    public class UserManagesTickets
    {
    }

    [Story(Title = "User monitors ticket links",
        AsA = "Honourable user",
        IWant = "to monitor links between tickets",
        SoThat = "I can understand which tickets either linked or not")]
    public class UserMonitorsTicketLinks
    {
    }

    [Story(Title = "User gets comments",
        AsA = "Happy user",
        IWant = "To be able to read comments",
        SoThat = "I can see them")]
    public class UserReadsComments
    {
    }

    [Story(Title = "User manages comments",
        AsA = "Happy user",
        IWant = "To manage comments",
        SoThat = "I can add and delete comments")]
    public class UserManagesComments
    {
    }

    [Story(Title = "User manages ticket assignees",
    AsA = "Happy user",
    IWant = "To manage ticket assignees",
    SoThat = "I can add and delete assignees from ticket")]
    public class UserManagesAssignees
    {
    }
}