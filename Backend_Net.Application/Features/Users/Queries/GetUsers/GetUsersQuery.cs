using Backend_Net.Application.Common.Models;
using MediatR;

namespace Backend_Net.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<BaseResponse<List<UserDto>>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
}