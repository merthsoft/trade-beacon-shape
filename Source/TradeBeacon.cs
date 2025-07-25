using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Merthsoft.DesignatorShapes.TradeBeacon;

public static class TradeBeacon
{
    public static float Radius = 7.9f;

    private static IEnumerable<IntVec3> RadialCellsAround(IntVec3 center, float radius) 
        => GenRadial.RadialCellsAround(center, radius, true);

    private static IEnumerable<IntVec3> GenRadialCircle(IntVec3 vert1, float radius, bool filled)
    {
        var innerCells = RadialCellsAround(vert1, radius);

        return filled ? innerCells : FillCorners(RadialCellsAround(vert1, radius + 1).Except(innerCells), vert1);
    }

    private static IEnumerable<IntVec3> FillCorners(IEnumerable<IntVec3> shape, IntVec3 center)
    {
        foreach (var cell in shape)
        {
            yield return cell;
            if (cell.x == center.x || cell.z == center.z)
                continue;
            var quadrant = cell.x < center.x && cell.z > center.z ? 0 :
                           cell.x < center.x && cell.z < center.z ? 1 :
                           cell.x > center.x && cell.z < center.z ? 2 :
                           cell.x > center.x && cell.z > center.z ? 3 : throw new Exception($"{cell.x},{cell.y} : {center.x},{center.y}");
            switch (quadrant)
            {
                case 0:
                    if (shape.Contains(cell + IntVec3.NorthEast) && !shape.Contains(cell + IntVec3.East) && !shape.Contains(cell + IntVec3.North))
                        yield return cell + IntVec3.North;
                    break;
                case 1:
                    if (shape.Contains(cell + IntVec3.SouthEast) && !shape.Contains(cell + IntVec3.East) && !shape.Contains(cell + IntVec3.South))
                        yield return cell + IntVec3.South;
                    break;
                case 2:
                    if (shape.Contains(cell + IntVec3.SouthWest) && !shape.Contains(cell + IntVec3.West) && !shape.Contains(cell + IntVec3.South))
                        yield return cell + IntVec3.South;
                    break;
                case 3:
                    if (shape.Contains(cell + IntVec3.NorthWest) && !shape.Contains(cell + IntVec3.West) && !shape.Contains(cell + IntVec3.North))
                        yield return cell + IntVec3.North;
                    break;
            }
        }
    }

    public static IEnumerable<IntVec3> Filled(IntVec3 vert1) 
        => GenRadialCircle(vert1, Radius, true);

    public static IEnumerable<IntVec3> Outline(IntVec3 vert1)
        => GenRadialCircle(vert1, Radius, false);
}
