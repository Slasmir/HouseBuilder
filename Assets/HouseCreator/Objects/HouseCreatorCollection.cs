using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HouseSet", menuName = "HouseCreator/Create New Set", order = 1)]
public class HouseCreatorCollection : ScriptableObject
{

    public Mesh[] FullWall, HalfWall, FullRoofEnd, HalfRoofEnd, FullRoofEndTop, HalfRoofEndTop, FullRoofCenter, HalfRoofCenter, Corner;
    public Material DefaultMat;

    public Mesh GetMeshBasedOnPointType(HouseCreatorBase.PointType pointType)
    {
        switch (pointType)
        {
            case HouseCreatorBase.PointType.Wall:
                return GetRandomFullWall();
            case HouseCreatorBase.PointType.HalfWall:
                return GetRandomHalfWall();
            case HouseCreatorBase.PointType.RoofEnd:
                return GetRandomFullRoofEnd();
            case HouseCreatorBase.PointType.HalfRoofEnd:
                return GetRandomHalfRoofEnd();
            case HouseCreatorBase.PointType.Roof:
                return GetRandomFullRoofCenter();
            case HouseCreatorBase.PointType.HalfRoof:
                return GetRandomHalfRoofCenter();
            default:
                return null;
        }


    }

    public Mesh GetRandomFullWall()
    {
        return FullWall[Random.Range(0, FullWall.Length - 1)];
    }

    public Mesh GetRandomHalfWall()
    {
        return HalfWall[Random.Range(0, HalfWall.Length - 1)];
    }

    public Mesh GetRandomFullRoofEnd()
    {
        return FullRoofEnd[Random.Range(0, FullRoofEnd.Length - 1)];
    }

    public Mesh GetRandomHalfRoofEnd()
    {
        return HalfRoofEnd[Random.Range(0, HalfRoofEnd.Length - 1)];
    }

    //Toimplement
    public Mesh GetRandomFullRoofEndTop()
    {
        return FullRoofEndTop[Random.Range(0, FullRoofEndTop.Length - 1)];
    }

    //toimplement
    public Mesh GetRandomHalfRoofEndTop()
    {
        return HalfRoofEndTop[Random.Range(0, HalfRoofEndTop.Length - 1)];
    }

    public Mesh GetRandomFullRoofCenter()
    {
        return FullRoofCenter[Random.Range(0, FullRoofCenter.Length - 1)];
    }

    public Mesh GetRandomHalfRoofCenter()
    {
        return HalfRoofCenter[Random.Range(0, HalfRoofCenter.Length - 1)];
    }

    public Mesh GetRandomCorner()
    {
        return Corner[Random.Range(0, Corner.Length - 1)];
    }
}
