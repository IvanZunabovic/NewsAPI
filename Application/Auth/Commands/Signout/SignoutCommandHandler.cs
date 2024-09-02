using Application.Abstractions.Messaging;
using Application.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Auth.Commands.Signout;

public sealed class SignoutCommandHandler : ICommandHandler<SignoutCommand>
{
    private readonly IClaimsService _claimsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SignoutCommandHandler(IClaimsService claimsService, IHttpContextAccessor httpContextAccessor)
    {
        _claimsService = claimsService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(SignoutCommand request, CancellationToken cancellationToken)
    {
        await _claimsService.SignoutAsync(_httpContextAccessor);
        return Result.Success();
    }
}
