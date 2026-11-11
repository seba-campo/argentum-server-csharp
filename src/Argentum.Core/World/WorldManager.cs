using Argentum.Core.Enums;
using Argentum.Core.Maps;
using Argentum.Core.Spatial;

namespace Argentum.Core.World;

/// <summary>
/// Gestiona la totalidad del mundo de juego: los mapas cargados,
/// y la ubicación/spawn/despawn de jugadores y entidades.
/// Reemplaza las variables y módulos globales de VB6 (MapData, ModMap, ModAreas, Modulo_UsUaRiOs).
/// </summary>
public sealed class WorldManager
{
    /// <summary>
    /// Radio de visión estándar en casillas (corresponde a la mitad de la pantalla de cliente en AO: 23x18 -> radio ~15).
    /// </summary>
    public const int DefaultVisionRange = 15;

    private readonly Dictionary<short, Map> _maps = new();
    private readonly Dictionary<int, Position> _playerPositions = new();
    private readonly object _syncRoot = new();

    /// <summary>
    /// Cantidad de mapas actualmente registrados.
    /// </summary>
    public int MapCount
    {
        get
        {
            lock (_syncRoot)
            {
                return _maps.Count;
            }
        }
    }

    /// <summary>
    /// Cantidad de jugadores actualmente ubicados en el mundo.
    /// </summary>
    public int ActivePlayersCount
    {
        get
        {
            lock (_syncRoot)
            {
                return _playerPositions.Count;
            }
        }
    }

    /// <summary>
    /// Registra un mapa en el mundo.
    /// </summary>
    public void AddMap(Map map)
    {
        ArgumentNullException.ThrowIfNull(map);

        lock (_syncRoot)
        {
            if (!_maps.TryAdd(map.Id, map))
            {
                throw new InvalidOperationException($"El mapa con ID {map.Id} ya está registrado en el mundo.");
            }
        }
    }

    /// <summary>
    /// Obtiene un mapa por su identificador.
    /// Retorna null si el mapa no existe.
    /// </summary>
    public Map? GetMap(short mapId)
    {
        lock (_syncRoot)
        {
            return _maps.GetValueOrDefault(mapId);
        }
    }

    /// <summary>
    /// Spawnea o posiciona a un jugador en el mapa especificado.
    /// Si el jugador ya estaba en el mundo, se remueve de su posición anterior.
    /// Retorna true si pudo posicionarse exitosamente, o false si la casilla o mapa no son válidos/caminables.
    /// </summary>
    public bool SpawnPlayer(int playerId, Position targetPos)
    {
        lock (_syncRoot)
        {
            if (!_maps.TryGetValue(targetPos.Map, out var map))
            {
                return false;
            }

            if (!map.IsWalkable(targetPos.X, targetPos.Y))
            {
                return false;
            }

            // Si el jugador ya estaba posicionado, removerlo de la posición anterior
            if (_playerPositions.TryGetValue(playerId, out var oldPos))
            {
                if (_maps.TryGetValue(oldPos.Map, out var oldMap))
                {
                    var oldTile = oldMap.GetTile(oldPos.X, oldPos.Y);
                    if (oldTile.IsOccupied && oldTile.OccupantId == playerId)
                    {
                        oldTile.ClearOccupant();
                    }
                }
            }

            var targetTile = map.GetTile(targetPos.X, targetPos.Y);
            targetTile.SetOccupant(playerId, TileOccupantType.Player);
            _playerPositions[playerId] = targetPos;
            return true;
        }
    }

    /// <summary>
    /// Remueve a un jugador del mundo (desconexión o muerte).
    /// </summary>
    public bool DespawnPlayer(int playerId)
    {
        lock (_syncRoot)
        {
            if (!_playerPositions.Remove(playerId, out var currentPos))
            {
                return false;
            }

            if (_maps.TryGetValue(currentPos.Map, out var map))
            {
                var tile = map.GetTile(currentPos.X, currentPos.Y);
                if (tile.IsOccupied && tile.OccupantId == playerId)
                {
                    tile.ClearOccupant();
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Obtiene la posición actual de un jugador si está en el mundo.
    /// </summary>
    public Position? GetPlayerPosition(int playerId)
    {
        lock (_syncRoot)
        {
            return _playerPositions.TryGetValue(playerId, out var pos) ? pos : null;
        }
    }

    /// <summary>
    /// Intenta mover a un jugador hacia la dirección indicada (Heading).
    /// Si el movimiento es válido y la casilla de destino está libre, actualiza el mapa
    /// y retorna la nueva posición. Si no puede moverse, retorna null.
    /// </summary>
    public Position? MovePlayer(int playerId, Heading heading)
    {
        lock (_syncRoot)
        {
            if (!_playerPositions.TryGetValue(playerId, out var currentPos))
            {
                return null;
            }

            var nextPos = currentPos.Step(heading);
            if (nextPos == currentPos)
            {
                // El paso no cambió las coordenadas (límite del mapa alcanzado)
                return null;
            }

            if (!_maps.TryGetValue(currentPos.Map, out var map))
            {
                return null;
            }

            if (!map.TryMoveOccupant(currentPos, nextPos))
            {
                return null;
            }

            _playerPositions[playerId] = nextPos;
            return nextPos;
        }
    }

    /// <summary>
    /// Obtiene los identificadores de todos los jugadores ubicados en el mismo mapa
    /// y dentro del rango de visión Chebyshev respecto a una posición de referencia.
    /// Permite excluir a un jugador en particular (por ejemplo, al que realiza la acción).
    /// </summary>
    public IReadOnlyList<int> GetNearbyPlayers(Position centerPos, int visionRange = DefaultVisionRange, int? excludePlayerId = null)
    {
        lock (_syncRoot)
        {
            var nearby = new List<int>();

            foreach (var (playerId, pos) in _playerPositions)
            {
                if (excludePlayerId.HasValue && playerId == excludePlayerId.Value)
                {
                    continue;
                }

                if (centerPos.IsInRange(pos, visionRange))
                {
                    nearby.Add(playerId);
                }
            }

            return nearby;
        }
    }
}
