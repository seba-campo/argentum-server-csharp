using Argentum.Core.Enums;
using Argentum.Core.Spatial;
using Xunit;

namespace Argentum.Tests.Core;

public class SpatialTests
{
    [Theory]
    [InlineData(Heading.North, 0, -1)]
    [InlineData(Heading.East, 1, 0)]
    [InlineData(Heading.South, 0, 1)]
    [InlineData(Heading.West, -1, 0)]
    public void Heading_ToOffset_ReturnsCorrectDelta(Heading heading, int expectedDx, int expectedDy)
    {
        var (dx, dy) = heading.ToOffset();
        Assert.Equal(expectedDx, dx);
        Assert.Equal(expectedDy, dy);
    }

    [Theory]
    [InlineData(Heading.North, Heading.South)]
    [InlineData(Heading.South, Heading.North)]
    [InlineData(Heading.East, Heading.West)]
    [InlineData(Heading.West, Heading.East)]
    public void Heading_GetOpposite_ReturnsOppositeDirection(Heading heading, Heading expectedOpposite)
    {
        Assert.Equal(expectedOpposite, heading.GetOpposite());
    }

    [Fact]
    public void Position_Validity_ValidatesMapAndCoordinateBounds()
    {
        // Válida: Ullathorpe centro
        var validPos = new Position(1, 50, 50);
        Assert.True(validPos.IsValid);

        // Inválida: Mapa 0
        var invalidMap = new Position(0, 50, 50);
        Assert.False(invalidMap.IsValid);

        // Inválida: X fuera de rango (0 o 101)
        var invalidXZero = new Position(1, 0, 50);
        var invalidXOverflow = new Position(1, 101, 50);
        Assert.False(invalidXZero.IsValid);
        Assert.False(invalidXOverflow.IsValid);

        // Inválida: Y fuera de rango (0 o 101)
        var invalidYZero = new Position(1, 50, 0);
        var invalidYOverflow = new Position(1, 50, 101);
        Assert.False(invalidYZero.IsValid);
        Assert.False(invalidYOverflow.IsValid);
    }

    [Fact]
    public void Position_Step_MovesCorrectlyAcrossCardinalDirections()
    {
        var origin = new Position(1, 50, 50);

        var north = origin.Step(Heading.North);
        Assert.Equal(new Position(1, 50, 49), north);

        var south = origin.Step(Heading.South);
        Assert.Equal(new Position(1, 50, 51), south);

        var east = origin.Step(Heading.East);
        Assert.Equal(new Position(1, 51, 50), east);

        var west = origin.Step(Heading.West);
        Assert.Equal(new Position(1, 49, 50), west);
    }

    [Fact]
    public void Position_Step_ClampsAtMapBoundaries()
    {
        var topBorder = new Position(1, 50, 1);
        var cannotStepNorth = topBorder.Step(Heading.North);
        Assert.Equal(topBorder, cannotStepNorth); // No debe salir a Y=0

        var leftBorder = new Position(1, 1, 50);
        var cannotStepWest = leftBorder.Step(Heading.West);
        Assert.Equal(leftBorder, cannotStepWest); // No debe salir a X=0
    }

    [Fact]
    public void Position_ChebyshevDistance_CalculatesTileRange()
    {
        var posA = new Position(1, 50, 50);
        var posB = new Position(1, 55, 52); // dx = 5, dy = 2 -> Chebyshev = 5

        Assert.Equal(5, posA.ChebyshevDistanceTo(posB));
        Assert.True(posA.IsInRange(posB, range: 5));
        Assert.False(posA.IsInRange(posB, range: 4));

        // Diferente mapa -> distancia infinita
        var posOtherMap = new Position(2, 50, 50);
        Assert.Equal(int.MaxValue, posA.ChebyshevDistanceTo(posOtherMap));
        Assert.False(posA.IsInRange(posOtherMap, range: 100));
    }
}
