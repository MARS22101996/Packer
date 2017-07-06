using TestStack.BDDfy;

namespace NotificationService.IntegrationTests.Stories
{
    [Story(
        Title = "User adds watcher to ticket",
        AsA = "User",
        IWant = "to add watcher to my ticket",
        SoThat = "I can check watcher if he exists in database and then add him")]
    public class AddWatcherToTicket { }

    [Story(
       Title = "User gets all watchers of ticket",
       AsA = "User",
       IWant = "to get all watchers of my ticket",
       SoThat = "I can get  all watchers if they exist in database")]
    public class GetAllWatchersOfTicket { }

    [Story(
       Title = "User gets specific user of ticket",
       AsA = "User",
       IWant = "to get specific user of my ticket",
       SoThat = "I can get this user if he exists in database")]
    public class GetSpecificWatcherOfTicket { }

    [Story(
        Title = "User removes watcher of ticket",
        AsA = "User",
        IWant = "to remove watcher from my ticket",
        SoThat = "I can get this watcher if he exist in database and then remove him")]
    public class RemoveWatchersOfTicket { }
}
