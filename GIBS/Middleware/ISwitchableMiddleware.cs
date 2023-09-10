namespace GIBS.Middleware;

/// <summary>
/// Allows <typeparamref name="TMiddleware"/> to be used like a switch case along with its other implementations
/// when injected as <see cref="IEnumerable{T}"/> of <see cref="ISwitchableMiddleware{TMiddleware, TSwitch}"/>.
/// </summary>
/// <typeparam name="TMiddleware">Type of the middleware intended to be used like a switch case.</typeparam>
/// <typeparam name="TSwitch">Type of the object used to determine the matching <typeparamref name="TMiddleware"/> implementation.</typeparam>
public interface ISwitchableMiddleware<TMiddleware, TSwitch>
    : IMiddleware, ISwitchableService<ISwitchableService<TMiddleware, TSwitch>, TSwitch>
    where TMiddleware : class, IMiddleware
{
}

internal static class SwitchableMiddlewareExtensions
{
    internal static TMiddleware? Switch<TMiddleware, TSwitch>(
        this IEnumerable<ISwitchableMiddleware<TMiddleware, TSwitch>> implementations,
        TSwitch switchObject) where TMiddleware : class, IMiddleware
        => (TMiddleware?)implementations.FirstOrDefault(implementation => implementation.Matches(switchObject));
}
