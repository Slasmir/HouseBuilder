using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofPlane  {

    Vector3 Origin;
    Vector3 Size;

    public RoofPlane (Vector3 _origin, Vector3 _size)
    {
        Origin = _origin;
        Size = _size;
    }


    public void DrawDebug(Transform parent)
    {
        DrawRoofPlane(parent);
    }


    private void DrawRoofPlane(Transform parent)
    {
        float Areal = Size.x * Size.z;
        float offset = Areal * 0.1f;
        Vector3 Offset3dD = offset * Vector3.up;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Origin + parent.position + (Size / 2f) + Offset3dD, Size + new Vector3(0, .03f, 0));
    }
}
