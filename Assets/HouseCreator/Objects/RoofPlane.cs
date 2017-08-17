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


    public void DrawRoofPlane(Transform parent)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Origin + parent.position, Size + new Vector3(0, .03f, 0));
    }
}
