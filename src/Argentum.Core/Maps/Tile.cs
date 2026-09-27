using Argentum.Core.Spatial;

namespace Argentum.Core.Maps;

/// <summary>
/// Tipo de ocupante presente en un casillero del mapa.
/// </summary>
public enum TileOccupantType : byte
{
    None = 0,
    Player = 1,
    Npc = 2
}

/// <summary>
/// Representa un casillero individual de un mapa en Argentum Online (t_MapBlock).
/// </summary>
public sealed class Tile
{
    /// <summary>
    /// Indica si el terreno está bloqueado naturalmente (árboles, montañas, agua sin barco, paredes).
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Tipo de entidad que ocupa actualmente este casillero.
    /// </summary>
    public TileOccupantType OccupantType { get; private set; } = TileOccupantType.None;

    /// <summary>
    /// Identificador único del jugador o criatura que ocupa el casillero (0 si está libre).
    /// </summary>
    public int OccupantId { get; private set; }

    /// <summary>
    /// Destino de teletransporte si este casillero es un portal, entrada a dungeon o cambio de mapa.
    /// </summary>
    public Position? Exit { get; set; }

    /// <summary>
    /// Indica si el casillero está ocupado por un jugador o criatura.
    /// </summary>
    public bool IsOccupied => OccupantType != TileOccupantType.None;

    /// <summary>
    /// Indica si un personaje puede ingresar y pararse en este casillero.
    /// Para ser caminable, no debe estar bloqueado por terreno ni ocupado por otra entidad.
    /// </summary>
    public bool IsWalkable => !IsBlocked && !IsOccupied;

    /// <summary>
    /// Establece el ocupante del casillero.
    /// </summary>
    public void SetOccupant(int id, TileOccupantType type)
    {
        OccupantId = id;
        OccupantType = type;
    }

    /// <summary>
    /// Libera el casillero.
    /// </summary>
    public void ClearOccupant()
    {
        OccupantId = 0;
        OccupantType = TileOccupantType.None;
    }
}
