namespace Todo.Backend.User.Services.RequestResponse;

using global::Todo.Contracts.Enums;

public class GetUserRoleByEmailResponse
{
    public UserRoles Role { get; set; }
}