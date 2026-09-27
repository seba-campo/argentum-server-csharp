namespace Argentum.Core.Enums;

/// <summary>
/// Orientación cardinal de personajes, criaturas y proyectiles en el mundo de Argentum Online.
/// En el protocolo de red viaja como byte (1..4).
/// </summary>
public enum Heading : byte
{
    North = 1,
    East = 2,
    South = 3,
    West = 4
}

/// <summary>
/// Métodos de extensión y utilidades para Heading.
/// </summary>
public static class HeadingExtensions
{
    /// <summary>
    /// Devuelve el desplazamiento (delta X, delta Y) correspondiente a la dirección.
    /// Recordar: en los mapas 2D de AO, el Norte decrementa Y, el Sur incrementa Y.
    /// </summary>
    public static (int DeltaX, int DeltaY) ToOffset(this Heading heading) => heading switch
    {
        Heading.North => (0, -1),
        Heading.East => (1, 0),
        Heading.South => (0, 1),
        Heading.West => (-1, 0),
        _ => (0, 0)
    };

    /// <summary>
    /// Devuelve la dirección contraria (reversa).
    /// </summary>
    public static Heading GetOpposite(this Heading heading) => heading switch
    {
        Heading.North => Heading.South,
        Heading.South => Heading.North,
        Heading.East => Heading.West,
        Heading.West => Heading.East,
        _ => heading
    };

    /// <summary>
    /// Valida si el valor de byte corresponde a un Heading válido (1 a 4).
    /// </summary>
    public static bool IsValid(byte value) => value is >= 1 and <= 4;
}
