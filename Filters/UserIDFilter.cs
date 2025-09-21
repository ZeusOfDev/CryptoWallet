using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class UserIdFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userId != null && int.TryParse(userId, out int parsedUserID))
        {
            // Add UserId to Action Arguments if the action has a parameter named "userId"
            if (context.ActionArguments.ContainsKey("UserID"))
            {
                context.ActionArguments["UserID"] = parsedUserID;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // do nothing after the action
    }
}
