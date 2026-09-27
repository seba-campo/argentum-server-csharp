using Argentum.Core.Enums;

namespace Argentum.Core.Spatial;

/// <summary>
/// Representa una posición inmutable en el mundo de Argentum Online (t_WorldPos: Mapa, X, Y).
/// Implementado como readonly record struct para cero asignaciones en memoria (zero-allocation).
/// </summary>
public readonly record struct Position(short Map, byte X, byte Y)
{
    public const byte MinCoordinate = 1;
    public const byte MaxCoordinate = 100;

    /// <summary>
    /// Indica si las coordenadas se encuentran dentro de los límites válidos de un mapa de AO (1 a 100).
    /// </summary>
    public bool IsValid =>
        Map > 0 &&
        X is >= MinCoordinate and <= MaxCoordinate &&
        Y is >= MinCoordinate and <= MaxCoordinate;

    /// <summary>
    /// Calcula la nueva posición al avanzar un paso hacia una dirección.
    /// Si el movimiento sale de los límites del mapa, devuelve la posición sin cambio.
    /// </summary>
    public Position Step(Heading heading)
    {
        var (dx, dy) = heading.ToOffset();
        int newX = X + dx;
        int newY = Y + dy;

        if (newX is < MinCoordinate or > MaxCoordinate || newY is < MinCoordinate or > MaxCoordinate)
        {
            return this;
        }

        return new Position(Map, (byte)newX, (byte)newY);
    }

    /// <summary>
    /// Calcula la distancia Chebyshev (distancia máxima en casilla) respecto a otra posición.
    /// Esta es la métrica de distancia utilizada para el campo de visión, ataques a distancia y hechizos en AO.
    /// </summary>
    public int ChebyshevDistanceTo(Position other)
    {
        if (Map != other.Map)
        {
            return int.MaxValue;
        }

        return Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
    }

    /// <summary>
    /// Indica si otra posición se encuentra dentro del rango de visión o interacción en el mismo mapa.
    /// </summary>
    public bool IsInRange(Position other, int range)
    {
        return Map == other.Map && ChebyshevDistanceTo(other) <= range;
    }

    /// <summary>
    /// Determina la orientación cardinal principal para mirar o avanzar hacia una posición objetivo.
    /// </summary>
    public Heading? DirectionTo(Position target)
    {
        if (Map != target.Map || this == target)
        {
            return null;
        }

        int dx = target.X - X;
        int dy = target.Y - Y;

        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            return dx > 0 ? Heading.East : Heading.West;
        }

        return dy > 0 ? Heading.South : Heading.North;
    }

    public override string ToString() => $"[Map: {Map}, X: {X}, Y: {Y}]";
}
