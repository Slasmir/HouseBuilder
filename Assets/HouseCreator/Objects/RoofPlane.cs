using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofPlane
{

    Vector3 Origin;
    Vector3 Size;
    Vector3 Dir;

    public RoofPlane(Vector3 _origin, Vector3 _size)
    {
        Origin = _origin;
        Size = _size;
        FindDirection();
    }

    void FindDirection()
    {
        if (Size.x > Size.z)
        {
            Dir = new Vector3(1, 0, 0);
        }
        else if (Size.x == Size.z)
        {
            if (Random.Range(0, 1) == 0)
            {
                Dir = new Vector3(1, 0, 0);
            }
            else
            {
                Dir = new Vector3(0, 0, 1);
            }
        }
        else
        {
            Dir = new Vector3(0, 0, 1);
        }
    }

    public float GetArea()
    {
        return Size.x * Size.z;
    }

    Rect CreateRectFromRoofPlane(RoofPlane rp)
    {
        return new Rect(rp.Origin.x, rp.Origin.z, rp.Size.x, rp.Size.z);
    }

    public bool RoofPlanesOverlap(RoofPlane other)
    {
        Rect rect1 = CreateRectFromRoofPlane(this);
        Rect rect2 = CreateRectFromRoofPlane(other);

        return rect1.Overlaps(rect2);
    }

    public GameObject CreateRoof(Transform parent, HouseCreatorCollection collection)
    {
        //Outlier in case the roof is one 1x1
        if (GetArea() == 1) return CreateOneAreaRoof(parent, collection);

        GameObject Roof = new GameObject("roof");
        Roof.transform.position = parent.transform.position;

        List<GridPoint> PointsToSpawn = new List<GridPoint>();

        float TestSize;
        Vector3 NonTestedSize;
        Vector3 FlipScale = new Vector3(-1, 1, 1);

        //Takes care of X Directed Roofs
        if (Dir == new Vector3(1, 0, 0))
        {
            TestSize = Size.x;
            NonTestedSize = new Vector3(0, 0, Size.z);
            FlipScale = new Vector3(-1, 1, 1);
        }
        else
        {
            TestSize = Size.z;
            NonTestedSize = new Vector3(Size.x, 0, 0);
            Dir *= -1;
            Origin = Origin + new Vector3(0,0, Size.z);
        }

        //Generating Roofs on one side
        for (int i = 0; i <= TestSize - 1f; i++)
        {
            HouseCreatorBase.PointType Type = HouseCreatorBase.PointType.Roof;
            if (i == 0 || i == TestSize - 1)
                Type = HouseCreatorBase.PointType.RoofEnd;

            GridPoint gp;
            if (i == 0)
            {
                gp = new GridPoint(Type, Origin + Dir * 1f);
                gp.ForcedScale = FlipScale;
            }
            else
            {
                gp = new GridPoint(Type, Origin + Dir * i);
            }
            gp.Dir = Dir;
            PointsToSpawn.Add(gp);
        }


        if (PointsToSpawn.Count != TestSize)
        {
            GridPoint gp = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Dir * TestSize - Dir * .5f);
            gp.Dir = Dir;
            PointsToSpawn.Add(gp);
        }


        //Generating Roofs on the other side
        for (int i = (int)Mathf.Floor(TestSize); i > 0; i--)
        {
            HouseCreatorBase.PointType Type = HouseCreatorBase.PointType.Roof;
            if (i == 1 || (i == Mathf.Floor(TestSize) && Mathf.Floor(TestSize) == TestSize))
                Type = HouseCreatorBase.PointType.RoofEnd;

            GridPoint gp;
            if ((i == Mathf.Floor(TestSize) && Mathf.Floor(TestSize) == TestSize))
            {
                gp = new GridPoint(Type, Origin + NonTestedSize + Dir * (i - 1));
                gp.ForcedScale = FlipScale;
            }
            else
            {
                gp = new GridPoint(Type, Origin + NonTestedSize + Dir * i);
            }

            gp.Dir = Dir * -1f;
            PointsToSpawn.Add(gp);
        }

        if (Mathf.Floor(TestSize) != TestSize)
        {
            GridPoint gp = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Dir * TestSize + NonTestedSize - Dir * 0.5f);
            gp.ForcedScale = FlipScale;
            gp.Dir = Dir * -1f;
            PointsToSpawn.Add(gp);
        }


        foreach (GridPoint gp in PointsToSpawn)
        {
            gp.CreateGridObject(Roof.transform, collection, false);
        }

        Roof.transform.SetParent(parent);
        return Roof;
    }

    private GameObject CreateOneAreaRoof(Transform parent, HouseCreatorCollection collection)
    {
        GameObject Roof = new GameObject("roof");
        Roof.transform.position = parent.transform.position;

        List<GridPoint> PointsToSpawn = new List<GridPoint>();
        //        0-----1
        //        |     |
        //        |     |
        //        2-----3

        Vector3 Offset;
        if(Dir == new Vector3(1, 0, 0))
        {
            Offset = new Vector3(0, 0, 1);
        }
        else
        {
            Offset = new Vector3(1, 0, 0);
        }

        GridPoint gp0 = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Dir * 0.5f);
        gp0.Dir = Dir;
        gp0.ForcedScale = new Vector3(-1, 1, 1);
        PointsToSpawn.Add(gp0);

        GridPoint gp1 = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Dir * 0.5f);
        gp1.Dir = Dir;
        PointsToSpawn.Add(gp1);

        GridPoint gp2 = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Offset + Dir * 0.5f);
        gp2.Dir = Dir * -1;
        PointsToSpawn.Add(gp2);

        GridPoint gp3 = new GridPoint(HouseCreatorBase.PointType.HalfRoofEnd, Origin + Offset + Dir * 0.5f);
        gp3.Dir = Dir * -1;
        gp3.ForcedScale = new Vector3(-1, 1, 1);
        PointsToSpawn.Add(gp3);

        foreach (GridPoint gp in PointsToSpawn)
        {
            gp.CreateGridObject(Roof.transform, collection, false);
        }

        Roof.transform.SetParent(parent);

        return Roof;
    }

    public void DrawDebug(Transform parent)
    {
        DrawRoofPlane(parent);
    }


    private void DrawRoofPlane(Transform parent)
    {
        if (Dir == new Vector3(1, 0, 0))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(Origin + parent.position + (Size / 2f) + Vector3.up * .1f, Vector3.Scale(Dir, Size) + new Vector3(0.075f, 0.075f, 0.075f));
            Gizmos.color = Color.red;
            Gizmos.DrawCube(Origin + parent.position + (Size / 2f) + Vector3.up * .05f, Size + new Vector3(0, .03f, 0));
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(Origin - new Vector3(0, 0, Size.z) + parent.position + (Size / 2f) + Vector3.up * .1f, Vector3.Scale(Dir, Size) + new Vector3(0.075f, 0.075f, 0.075f));
            Gizmos.color = Color.red;
            Gizmos.DrawCube(Origin - new Vector3(0,0,Size.z)  + parent.position + (Size / 2f) + Vector3.up * .05f, Size + new Vector3(0, .03f, 0));
        }
    }
}
