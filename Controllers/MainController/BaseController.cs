using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ChessTournamentCalendarBackend.API.Controllers;

[ApiController] 
public abstract class BaseController : ControllerBase 
{
    
    protected internal bool IsAuthorizedToAccessUser(Guid requestedUserId)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(currentUserIdClaim)) 
            return false;

        var currentUserId = Guid.Parse(currentUserIdClaim);

        return currentUserId == requestedUserId || User.IsInRole("Admin");
    }

    
    protected Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdString) ? Guid.Empty : Guid.Parse(userIdString);
    }
}