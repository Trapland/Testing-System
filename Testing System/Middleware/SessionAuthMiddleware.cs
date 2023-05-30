using System.Security.Claims;
using Testing_System.Data;
using Testing_System.Data.Entity;

namespace Testing_System.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<SessionAuthMiddleware> logger, DataContext dataContext)
        {
            String? userId = context.Session.GetString("authUserId");
            String? userStatus = context.Session.GetString("userStatus");

            if (userId is not null)
            {
                try
                {
                    if(userStatus == "student")
                    {
                        Student? authUser = dataContext.Students.Find(Guid.Parse(userId));
                        if (authUser is not null)
                        {
                            context.Items.Add("authUser", authUser);
                            Claim[] claims = new Claim[]
                            {
                            new Claim(ClaimTypes.Sid, userId),
                            new Claim(ClaimTypes.Name, authUser.Name),
                            new Claim(ClaimTypes.NameIdentifier, authUser.Login),
                            new Claim(ClaimTypes.UserData, authUser.Avatar ?? String.Empty)
                            };
                            var principal = new ClaimsPrincipal(
                                new ClaimsIdentity(claims,
                                nameof(SessionAuthMiddleware)));
                            context.User = principal;
                        }
                    }
                    else if(userStatus == "teacher")
                    {
                        Teacher? authUser = dataContext.Teachers.Find(Guid.Parse(userId));
                        if (authUser is not null)
                        {
                            context.Items.Add("authUser", authUser);
                            Claim[] claims = new Claim[]
                            {
                            new Claim(ClaimTypes.Sid, userId),
                            new Claim(ClaimTypes.Name, authUser.Name),
                            new Claim(ClaimTypes.NameIdentifier, authUser.Login),
                            new Claim(ClaimTypes.UserData, authUser.Avatar ?? String.Empty)
                            };
                            var principal = new ClaimsPrincipal(
                                new ClaimsIdentity(claims,
                                nameof(SessionAuthMiddleware)));
                            context.User = principal;
                        }
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "SessionAuthMiddleware");
                }
            }
            await _next(context);
        }
    }

    public static class SessionAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}