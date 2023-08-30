

using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;

namespace WonderfulRabbitsApi.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateToken(token);
            if (userId != null)
            {
                try
                {
                    // attach user to context on successful jwt validation
                    context.Items["User"] = await userService.GetUserAsync(userId.Value);
                }
                catch
                {
                    throw new AppException("Authorization Error: The user could not be found.");
                }
            }

            await _next(context);
        }
    }
}
