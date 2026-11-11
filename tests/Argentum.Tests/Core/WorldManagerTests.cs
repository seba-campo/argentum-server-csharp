using Argentum.Core.Enums;
using Argentum.Core.Maps;
using Argentum.Core.Spatial;
using Argentum.Core.World;

namespace Argentum.Tests.Core;

public class WorldManagerTests
{
    [Fact]
    public void AddMap_ShouldRegisterMapCorrectly()
    {
        var world = new WorldManager();
        var map1 = new Map(1, "Ullathorpe");

        world.AddMap(map1);

        Assert.Equal(1, world.MapCount);
        Assert.Same(map1, world.GetMap(1));
    }

    [Fact]
    public void AddMap_DuplicateId_ShouldThrowException()
    {
        var world = new WorldManager();
        var map1 = new Map(1, "Ullathorpe");
        var mapDuplicate = new Map(1, "Ullathorpe Clon");

        world.AddMap(map1);

        Assert.Throws<InvalidOperationException>(() => world.AddMap(mapDuplicate));
    }

    [Fact]
    public void SpawnPlayer_OnWalkableTile_ShouldSucceedAndOccupyTile()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        world.AddMap(map);

        var spawnPos = new Position(1, 50, 50);
        bool success = world.SpawnPlayer(10, spawnPos);

        Assert.True(success);
        Assert.Equal(1, world.ActivePlayersCount);
        Assert.Equal(spawnPos, world.GetPlayerPosition(10));

        var tile = map.GetTile(50, 50);
        Assert.True(tile.IsOccupied);
        Assert.Equal(10, tile.OccupantId);
        Assert.Equal(TileOccupantType.Player, tile.OccupantType);
    }

    [Fact]
    public void SpawnPlayer_OnUnregisteredMap_ShouldFail()
    {
        var world = new WorldManager();
        var pos = new Position(99, 50, 50);

        bool success = world.SpawnPlayer(1, pos);

        Assert.False(success);
        Assert.Equal(0, world.ActivePlayersCount);
    }

    [Fact]
    public void SpawnPlayer_OnBlockedTile_ShouldFail()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        map.GetTile(50, 50).IsBlocked = true;
        world.AddMap(map);

        bool success = world.SpawnPlayer(1, new Position(1, 50, 50));

        Assert.False(success);
        Assert.Equal(0, world.ActivePlayersCount);
    }

    [Fact]
    public void SpawnPlayer_WhenAlreadySpawned_ShouldRelocatePlayer()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        world.AddMap(map);

        var pos1 = new Position(1, 50, 50);
        var pos2 = new Position(1, 51, 50);

        world.SpawnPlayer(5, pos1);
        world.SpawnPlayer(5, pos2);

        Assert.Equal(1, world.ActivePlayersCount);
        Assert.Equal(pos2, world.GetPlayerPosition(5));
        Assert.False(map.GetTile(50, 50).IsOccupied);
        Assert.True(map.GetTile(51, 50).IsOccupied);
        Assert.Equal(5, map.GetTile(51, 50).OccupantId);
    }

    [Fact]
    public void DespawnPlayer_ShouldRemoveFromMapAndActiveList()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        world.AddMap(map);

        var pos = new Position(1, 50, 50);
        world.SpawnPlayer(7, pos);

        bool despawnSuccess = world.DespawnPlayer(7);

        Assert.True(despawnSuccess);
        Assert.Equal(0, world.ActivePlayersCount);
        Assert.Null(world.GetPlayerPosition(7));
        Assert.False(map.GetTile(50, 50).IsOccupied);
    }

    [Fact]
    public void DespawnPlayer_NonExistent_ShouldReturnFalse()
    {
        var world = new WorldManager();
        bool success = world.DespawnPlayer(999);
        Assert.False(success);
    }

    [Fact]
    public void MovePlayer_ToWalkableTile_ShouldUpdatePositionAndMapTiles()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        world.AddMap(map);

        var startPos = new Position(1, 50, 50);
        world.SpawnPlayer(1, startPos);

        // Mover hacia el Norte (Y - 1 = 49)
        var newPos = world.MovePlayer(1, Heading.North);

        Assert.NotNull(newPos);
        Assert.Equal(new Position(1, 50, 49), newPos.Value);
        Assert.Equal(newPos.Value, world.GetPlayerPosition(1));

        // Tile anterior libre, nuevo tile ocupado
        Assert.False(map.GetTile(50, 50).IsOccupied);
        Assert.True(map.GetTile(50, 49).IsOccupied);
        Assert.Equal(1, map.GetTile(50, 49).OccupantId);
    }

    [Fact]
    public void MovePlayer_ToBlockedTile_ShouldReturnNullAndKeepPosition()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        map.GetTile(51, 50).IsBlocked = true; // Bloqueamos el Este
        world.AddMap(map);

        var startPos = new Position(1, 50, 50);
        world.SpawnPlayer(1, startPos);

        // Intentar mover al Este
        var result = world.MovePlayer(1, Heading.East);

        Assert.Null(result);
        Assert.Equal(startPos, world.GetPlayerPosition(1));
        Assert.True(map.GetTile(50, 50).IsOccupied);
    }

    [Fact]
    public void MovePlayer_OutOfBounds_ShouldReturnNullAndKeepPosition()
    {
        var world = new WorldManager();
        var map = new Map(1, "Ullathorpe");
        world.AddMap(map);

        var edgePos = new Position(1, 1, 1);
        world.SpawnPlayer(1, edgePos);

        // Intentar caminar hacia el Norte desde Y=1
        var result = world.MovePlayer(1, Heading.North);

        Assert.Null(result);
        Assert.Equal(edgePos, world.GetPlayerPosition(1));
    }

    [Fact]
    public void GetNearbyPlayers_ShouldFilterByMapRangeAndExclusion()
    {
        var world = new WorldManager();
        var map1 = new Map(1, "Ullathorpe");
        var map2 = new Map(2, "Banderbill");
        world.AddMap(map1);
        world.AddMap(map2);

        // Jugador 1 en centro (1, 50, 50)
        world.SpawnPlayer(1, new Position(1, 50, 50));

        // Jugador 2 cerca (1, 52, 52) -> distancia Chebyshev = 2 (dentro de rango 15)
        world.SpawnPlayer(2, new Position(1, 52, 52));

        // Jugador 3 lejos en el mismo mapa (1, 90, 90) -> distancia Chebyshev = 40 (fuera de rango)
        world.SpawnPlayer(3, new Position(1, 90, 90));

        // Jugador 4 en otro mapa (2, 50, 50)
        world.SpawnPlayer(4, new Position(2, 50, 50));

        var centerPos = new Position(1, 50, 50);

        // Sin excluir
        var nearbyAll = world.GetNearbyPlayers(centerPos, visionRange: 15);
        Assert.Contains(1, nearbyAll);
        Assert.Contains(2, nearbyAll);
        Assert.DoesNotContain(3, nearbyAll);
        Assert.DoesNotContain(4, nearbyAll);

        // Excluyendo a sí mismo (Jugador 1)
        var nearbyOthers = world.GetNearbyPlayers(centerPos, visionRange: 15, excludePlayerId: 1);
        Assert.DoesNotContain(1, nearbyOthers);
        Assert.Contains(2, nearbyOthers);
    }
}
