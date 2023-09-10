using Microsoft.AspNetCore.Mvc;

namespace GIBS.Messages;

[ApiController]
[Route($"/{PathFragment}")]
public class MessagesController : ControllerBase
{
    internal const string PathFragment = "message";

    [HttpPost]
    public async Task Handle([FromServices] IEnumerable<MessageHandler> messageHandlers)
    {
        if (messageHandlers.Switch(HttpContext.GetUpdate().Message!) is MessageHandler messageHandler)
            await messageHandler.HandleAsync();
    }
}
