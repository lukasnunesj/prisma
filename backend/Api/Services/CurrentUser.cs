using backend.Api.Extensions;
using Microsoft.AspNetCore.Http;

namespace backend.Api.Services
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var user = (_httpContextAccessor.HttpContext?.User) ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
                return user.GetUserId();
            }
        }
    }
}
