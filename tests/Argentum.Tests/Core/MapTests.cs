using Argentum.Core.Maps;
using Argentum.Core.Spatial;
using Xunit;

namespace Argentum.Tests.Core;

public class MapTests
{
    [Fact]
    public void Map_Initialization_CreatesFullWalkableGrid()
    {
        var map = new Map(1, "Ullathorpe");

        Assert.Equal(1, map.Id);
        Assert.Equal("Ullathorpe", map.Name);

        // Los extremos 1..100 deben existir y ser caminables por defecto
        Assert.True(map.IsWalkable(1, 1));
        Assert.True(map.IsWalkable(100, 100));
        Assert.True(map.IsWalkable(50, 50));

        // Fuera de límites no debe ser caminable
        Assert.False(map.IsWalkable(0, 50));
        Assert.False(map.IsWalkable(101, 50));
    }

    [Fact]
    public void Tile_BlockedTerrain_CannotBeWalked()
    {
        var map = new Map(1, "Ullathorpe");
        var pos = new Position(1, 45, 60);

        Assert.True(map.IsWalkable(pos));

        // Bloquear por árbol / pared
        map.GetTile(pos).IsBlocked = true;

        Assert.False(map.IsWalkable(pos));
    }

    [Fact]
    public void Tile_OccupiedByEntity_BlocksMovement()
    {
        var map = new Map(1, "Ullathorpe");
        var tile = map.GetTile(50, 50);

        Assert.False(tile.IsOccupied);
        Assert.True(tile.IsWalkable);

        // Parar a un jugador con ID 42 en el casillero
        tile.SetOccupant(42, TileOccupantType.Player);

        Assert.True(tile.IsOccupied);
        Assert.Equal(42, tile.OccupantId);
        Assert.Equal(TileOccupantType.Player, tile.OccupantType);
        Assert.False(tile.IsWalkable);

        // Liberar el casillero
        tile.ClearOccupant();
        Assert.False(tile.IsOccupied);
        Assert.True(tile.IsWalkable);
    }

    [Fact]
    public void Map_TryMoveOccupant_MovesEntityBetweenTilesSuccessfully()
    {
        var map = new Map(1, "Ullathorpe");
        var startPos = new Position(1, 50, 50);
        var targetPos = new Position(1, 50, 51);

        // Colocar jugador en startPos
        map.GetTile(startPos).SetOccupant(99, TileOccupantType.Player);

        // Ejecutar movimiento
        bool moved = map.TryMoveOccupant(startPos, targetPos);

        Assert.True(moved);
        Assert.False(map.GetTile(startPos).IsOccupied);
        Assert.True(map.GetTile(targetPos).IsOccupied);
        Assert.Equal(99, map.GetTile(targetPos).OccupantId);
    }

    [Fact]
    public void Map_TryMoveOccupant_FailsIfTargetTileIsBlocked()
    {
        var map = new Map(1, "Ullathorpe");
        var startPos = new Position(1, 50, 50);
        var targetPos = new Position(1, 50, 51);

        // Colocar jugador en startPos
        map.GetTile(startPos).SetOccupant(99, TileOccupantType.Player);

        // Bloquear destino (árbol o pared)
        map.GetTile(targetPos).IsBlocked = true;

        // Intentar mover hacia la pared
        bool moved = map.TryMoveOccupant(startPos, targetPos);

        Assert.False(moved);
        Assert.True(map.GetTile(startPos).IsOccupied); // Permanece en el origen
        Assert.False(map.GetTile(targetPos).IsOccupied);
    }
}
