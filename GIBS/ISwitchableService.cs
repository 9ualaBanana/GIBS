namespace GIBS;

/// <summary>
/// Allows <typeparamref name="TService"/> to be used like a switch case along with its other implementations
/// when injected as <see cref="IEnumerable{T}"/> of <see cref="ISwitchableService{TService, TSwitch}"/>.
/// </summary>
/// <typeparam name="TService">Type of the service intended to be used like a switch case.</typeparam>
/// <typeparam name="TSwitch">Type of the object used to determine the matching <typeparamref name="TService"/> implementation.</typeparam>
public interface ISwitchableService<TService, TSwitch> where TService : class
{
    /// <summary>
    /// Determines whether this <typeparamref name="TService"/> implementation should be chosen by
    /// <see cref="SwitchableServiceExtensions.Switch{TService, TSwitch}(IEnumerable{ISwitchableService{TService, TSwitch}}, TSwitch)"/>
    /// based on data stored inside <paramref name="switchObject"/>.
    /// </summary>
    /// <param name="switchObject">The object containing data that can be used to determine if this <typeparamref name="TService"/> implementation should be chosen</param>
    /// <returns>
    /// <see langword="true"/> if this <typeparamref name="TService"/> should be chosen; <see langword="false"/> otherwise.
    /// </returns>
    bool Matches(TSwitch switchObject);
}

internal static class SwitchableServiceExtensions
{
    internal static TService? Switch<TService, TSwitch>(
        this IEnumerable<ISwitchableService<TService, TSwitch>> implementations,
        TSwitch switchObject) where TService : class
        => (TService?)implementations.FirstOrDefault(implementation => implementation.Matches(switchObject));
}
