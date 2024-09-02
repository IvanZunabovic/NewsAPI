using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IClaimsService
{
    Task CreateClaims(IHttpContextAccessor contextAccessor, string userFullName);
    Task SignoutAsync(IHttpContextAccessor httpContextAccessor);
}
