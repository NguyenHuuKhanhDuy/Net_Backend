using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend_Net.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler 
    : IRequestHandler<GetUsersQuery, BaseResponse<List<UserDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetUsersQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseResponse<List<UserDto>>> Handle(
        GetUsersQuery request, 
        CancellationToken cancellationToken)
    {

        var q = _uow.User.GetAll().OrderByDescending(x => x.Email);

        var total = await q.CountAsync(cancellationToken);

        var data = await q
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(x => new UserDto
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName
            })
            .ToListAsync(cancellationToken);

        var paging = new PagingInfo
        {
            Page = request.Page,
            Limit = request.Limit,
            TotalItem = total,
            TotalPage = (int)Math.Ceiling((double)total / request.Limit)
        };

        return BaseResponse<List<UserDto>>.Ok(data, paging);
    }
}