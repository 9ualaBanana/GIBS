using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIBS.Commands;

[ApiController]
[Route($"/{PathFragment}")]
public class CommandsController : ControllerBase
{
    internal const string PathFragment = "command";

    /// <remarks>
    /// Explicitly requested via DI because some <see cref="Command"/>s are <see cref="IAuthorizationPolicyProtected"/> and
    /// their <see cref="AuthorizationPolicy"/> must be explicitly passed to <see cref="IAuthorizationService"/> during imperative authorization.
    /// </remarks>
    readonly IAuthorizationService _authorizationService;
    readonly Command _receivedCommand;

    readonly ILogger<CommandsController> _logger;

    public CommandsController(
        IAuthorizationService authorizationService,
        Command.Received receivedCommand,
        ILogger<CommandsController> logger)
    {
        _authorizationService = authorizationService;
        _receivedCommand = receivedCommand.Get();
        _logger = logger;
    }

    [HttpPost]
    public async Task Handle([FromServices] IEnumerable<CommandHandler> commandHandlers)
    {
        if (commandHandlers.Switch(_receivedCommand) is CommandHandler command)
        {
            if (command is IAuthorizationPolicyProtected command_)
                if (!await UserIsAuthorizedToCall(command_))
                { _logger.LogTrace("User is not authorized to use {Command} command", _receivedCommand.Prefixed); return; }

            await command.HandleAsync();
        }
        else _logger.LogTrace("{Command} command is unknown", _receivedCommand.Prefixed);


        async Task<bool> UserIsAuthorizedToCall(IAuthorizationPolicyProtected command)
        {
            AuthorizationResult authorization = await _authorizationService.AuthorizeAsync(User, command, command.AuthorizationPolicy);
            return authorization.Succeeded;
        }
    }
}
