using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint {

    public HouseCreatorBase.PointType Type;
    public Vector3 Location;
    public Vector3 Dir;
    public Vector3 ForcedScale = Vector3.one;
    public bool IsCorner = false;
    public bool IsTop = false;
    public bool IsSmall = false;

    public bool IsHalf()
    {
        if (Type == HouseCreatorBase.PointType.HalfWall) return true;
        if (Type == HouseCreatorBase.PointType.HalfRoof) return true;
        if (Type == HouseCreatorBase.PointType.HalfRoofEnd) return true;

        return false;
    }

    public bool IsRoofType()
    {
        if (Type == HouseCreatorBase.PointType.HalfRoof) return true;
        if (Type == HouseCreatorBase.PointType.HalfRoofEnd) return true;
        if (Type == HouseCreatorBase.PointType.Roof) return true;
        if (Type == HouseCreatorBase.PointType.RoofEnd) return true;
        if (Type == HouseCreatorBase.PointType.None_SkipRoof) return true;

        return false;
    }

    public bool IsWallType()
    {
        if (Type == HouseCreatorBase.PointType.Wall) return true;
        if (Type == HouseCreatorBase.PointType.HalfWall) return true;
        if (Type == HouseCreatorBase.PointType.None_SkipWall) return true;

        return false;
    }

    public bool IsPointValid()
    {
        if (Type == HouseCreatorBase.PointType.None) return false;
        if (Type == HouseCreatorBase.PointType.Filler) return false;
        if (Type == HouseCreatorBase.PointType.None_SkipRoof) return false;
        if (Type == HouseCreatorBase.PointType.None_SkipWall) return false;

        return true;
    }

    public bool IsPointPlaceable()
    {
        if (Type != HouseCreatorBase.PointType.None) return false;
        if (Type != HouseCreatorBase.PointType.Filler)return false;
        if (Type != HouseCreatorBase.PointType.Unsorted) return false;
        if (Type == HouseCreatorBase.PointType.None_SkipRoof) return false;
        if (Type == HouseCreatorBase.PointType.None_SkipWall) return false;

        return true;
    }

    public GridPoint(HouseCreatorBase.PointType PT, Vector3 L)
    {
        Type = PT;
        Location = L;
        Dir = Vector3.zero;
    }

    public GridPoint GetNeighbor(Vector3 dir, List<GridPoint> grid) { return GetNeighbor(dir, grid, false); }
    public GridPoint GetNeighbor(Vector3 dir, List<GridPoint> grid, bool RespectHalfs)
    {

        if (RespectHalfs)
        {
            Vector3 RealDir = dir;

            if (IsHalf())
            {
                RealDir *= .5f;
            }

            return GetNeighbor(RealDir, grid);
        }
        else
        {
            foreach (GridPoint p in grid)
            {
                if (p.Location == Location + dir)
                {
                    return p;
                }
            }
        }

        return null;
    }

    public HouseCreatorBase.PointType GetNeighborType(Vector3 dir, List<GridPoint> grid)
    {
        GridPoint gp = GetNeighbor(dir, grid);
        if (gp != null)
            return gp.Type;
        else
            return HouseCreatorBase.PointType.None;
    }



    public GameObject CreateGridObject(Transform parent, HouseCreatorCollection selectedCollection, bool IgnoreRoof = true)
    {
        if(!IsPointValid())
            return null;

        //TODO: FIX ROOF
        if (IgnoreRoof && IsRoofType())
            return null;

        GameObject GridObject = new GameObject("GridObject");

        MeshFilter mf = GridObject.AddComponent<MeshFilter>();
        MeshRenderer mr = GridObject.AddComponent<MeshRenderer>();

        GridObject.transform.position = Location + parent.position;
        GridObject.transform.SetParent(parent);
        GridObject.transform.localRotation = Quaternion.LookRotation(Dir);
        GridObject.transform.Rotate(new Vector3(0, -90f, 0));
        GridObject.transform.localScale = ForcedScale;

        Mesh m = selectedCollection.GetMeshBasedOnPointType(Type,IsTop, IsSmall);
        mf.mesh = m;

        mr.sharedMaterial = selectedCollection.DefaultMat;

        if (IsCorner && !IsRoofType())
        {
            GameObject cornerObject = CreateCornerObject(parent, selectedCollection, GridObject.transform);
            cornerObject.transform.SetParent(GridObject.transform);
        }

        return GridObject;
    }

    //TODO: Do this
    public GameObject CreateCornerObject(Transform parent, HouseCreatorCollection selectedCollection, Transform GridObject)
    {
        GameObject CornerObject = new GameObject("Corner Pillar");

        CornerObject.transform.position = Location + parent.position;
        CornerObject.transform.SetParent(GridObject, true);

        MeshFilter mf = CornerObject.AddComponent<MeshFilter>();
        MeshRenderer mr = CornerObject.AddComponent<MeshRenderer>();

        Mesh m = selectedCollection.GetRandomCorner();
        mf.mesh = m;

        mr.sharedMaterial = selectedCollection.DefaultMat;

        return CornerObject;
    }

    #region DrawDebug
    public void DrawDebug(Vector3 ParentPos)
    {

        DrawDebugType(ParentPos);
        DrawDirection(ParentPos);
    }

    void DrawDirection(Vector3 ParentPos)
    {
        if (Dir == Vector3.zero)
        {
            if (IsPointPlaceable())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(ParentPos + Location, Vector3.one * 0.2f);
            }
        }
        else
        {
            Vector3 Size = Vector3.Scale(Vector3.one * 0.25f, Dir) + Vector3.one * 0.02f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(ParentPos + Location + Dir * 0.125f, Size);
        }
    }

    void DrawDebugType(Vector3 ParentPos)
    {
        if (Type == HouseCreatorBase.PointType.Wall)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.HalfWall)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.Roof)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.HalfRoof)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.Unsorted)
        {
            Gizmos.color = new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.None_SkipWall)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.None_SkipRoof)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ParentPos + Location, .075f);
        }
        else if (Type == HouseCreatorBase.PointType.Filler)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(ParentPos + Location, .075f);
        }

        if (IsCorner == true)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(ParentPos + Location, new Vector3(.05f, .05f, .05f));
        }
    }
    #endregion
}
