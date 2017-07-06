using TestStack.BDDfy;

namespace UserService.IntegrationTests
{
    [Story(
    AsA = "As an application administrator",
    IWant = "I want to add and remove role of specified user",
    SoThat = "So that I can manage his rights")]
    public class AddRemoveUserRoleStory { }

    [Story(
    AsA = "As an application administrator",
    IWant = "I want to add and remove roles",
    SoThat = "So that I can manage users rights using them")]
    public class AddRemoveRolesStory { }

    [Story(
    AsA = "As an application administrator",
    IWant = "I want to get info about application roles",
    SoThat = "So that I can use it for my own purpose")]
    public class GetRoleStory { }

    [Story(
    AsA = "As an application user",
    IWant = "I want to get info about some user in application",
    SoThat = "So that I can use it for my own purpose")]
    public class GetUserStory { }

    [Story(
    AsA = "As an unregistered user",
    IWant = "I want to be registered",
    SoThat = "So that I can have an account")]
    public class RegisterStory { }
}