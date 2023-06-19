using Pathfinding;
using UnityEngine;

[Beebyte.Obfuscator.Skip]
public static class CommonUtility
{
    public static Vector3 OnMovablePlane(this Vector3 position, bool restrainHeight = true, bool distanceXZ = false)
    {
        if (AstarPath.active == null)
            return position;

        NNInfo nninfo = AstarPath.active.GetNearest(position, new NNConstraint() { distanceXZ = distanceXZ });
        Vector3 pos = nninfo.position;
        if (!restrainHeight)
        {
            pos.y = position.y;
        }
        return pos;
    }

    public static float GetTerrainHeight(Vector3 position)
    {
        NNInfo nninfo = AstarPath.active.GetNearest(position, new NNConstraint() { distanceXZ = true });
        return nninfo.position.y;
    }
}
