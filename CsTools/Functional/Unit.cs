namespace CsTools;

/// <summary>
/// A real void return value
/// </summary>
public readonly record struct Unit()
{
    public static Unit Value { get; } = new Unit();
}