using Argentum.Core.Spatial;

namespace Argentum.Core.Maps;

/// <summary>
/// Representa un mapa completo en el mundo de Argentum Online (cuadrícula de 100x100 casillas).
/// </summary>
public sealed class Map
{
    public const int Width = 100;
    public const int Height = 100;

    // Array 101x101 para respetar la indexación 1-based clásica de Argentum Online (X: 1..100, Y: 1..100)
    private readonly Tile[,] _tiles = new Tile[Width + 1, Height + 1];

    public short Id { get; }
    public string Name { get; }

    public Map(short id, string name)
    {
        Id = id;
        Name = name;

        // Inicializar todas las casillas vacías por defecto
        for (int x = 1; x <= Width; x++)
        {
            for (int y = 1; y <= Height; y++)
            {
                _tiles[x, y] = new Tile();
            }
        }
    }

    /// <summary>
    /// Valida si las coordenadas están dentro de los límites del mapa (1 a 100).
    /// </summary>
    public static bool IsValidCoordinate(byte x, byte y) =>
        x is >= Position.MinCoordinate and <= Position.MaxCoordinate &&
        y is >= Position.MinCoordinate and <= Position.MaxCoordinate;

    /// <summary>
    /// Obtiene la casilla correspondiente a una coordenada (X, Y).
    /// </summary>
    public Tile GetTile(byte x, byte y)
    {
        if (!IsValidCoordinate(x, y))
        {
            throw new ArgumentOutOfRangeException($"Coordenadas ({x}, {y}) fuera de los límites del mapa {Id}.");
        }

        return _tiles[x, y];
    }

    /// <summary>
    /// Obtiene la casilla correspondiente a una posición.
    /// </summary>
    public Tile GetTile(Position pos)
    {
        if (pos.Map != Id)
        {
            throw new ArgumentException($"La posición pertenece al mapa {pos.Map}, pero este mapa es {Id}.");
        }

        return GetTile(pos.X, pos.Y);
    }

    /// <summary>
    /// Determina si una coordenada es transitable (dentro de los límites, no bloqueada y libre de ocupantes).
    /// </summary>
    public bool IsWalkable(byte x, byte y)
    {
        if (!IsValidCoordinate(x, y))
        {
            return false;
        }

        return _tiles[x, y].IsWalkable;
    }

    /// <summary>
    /// Determina si una posición es transitable.
    /// </summary>
    public bool IsWalkable(Position pos) => pos.Map == Id && IsWalkable(pos.X, pos.Y);

    /// <summary>
    /// Intenta mover un ocupante desde una posición de origen hacia una posición de destino.
    /// Valida que el destino sea caminable y realiza la transferencia atómica de casilleros.
    /// </summary>
    public bool TryMoveOccupant(Position from, Position to)
    {
        if (from.Map != Id || to.Map != Id)
        {
            return false;
        }

        if (!IsValidCoordinate(from.X, from.Y) || !IsValidCoordinate(to.X, to.Y))
        {
            return false;
        }

        Tile originTile = _tiles[from.X, from.Y];
        Tile targetTile = _tiles[to.X, to.Y];

        if (!originTile.IsOccupied || !targetTile.IsWalkable)
        {
            return false;
        }

        targetTile.SetOccupant(originTile.OccupantId, originTile.OccupantType);
        originTile.ClearOccupant();
        return true;
    }
}
