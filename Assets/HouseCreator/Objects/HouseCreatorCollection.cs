using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HouseSet", menuName = "HouseCreator/Create New Set", order = 1)]
public class HouseCreatorCollection : ScriptableObject
{

    public Mesh[] FullWall, HalfWall, FullRoofEnd, HalfRoofEnd, FullRoofEndTop, HalfRoofEndTop, FullRoofCenter, HalfRoofCenter, FullRoofCenterTop, HalfRoofCenterTop, FullRoofTopper, FullRoofTopperEnd, HalfRoofTopper, HalfRoofTopperEnd, FullRoofStopper, HalfRoofStopper, Corner;
    public Material DefaultMat;

    public Mesh GetMeshBasedOnPointType(HouseCreatorBase.PointType pointType, bool IsTop = false, bool IsSmall = false)
    {
        switch (pointType)
        {
            case HouseCreatorBase.PointType.Wall:
                return GetRandomFullWall();

            case HouseCreatorBase.PointType.HalfWall:
                return GetRandomHalfWall();

            case HouseCreatorBase.PointType.RoofEnd:
                if (IsSmall)
                    return GetRandomFullRoofTopperEnd();
                if (IsTop)
                    return GetRandomFullRoofEndTop();
                else
                    return GetRandomFullRoofEnd();

            case HouseCreatorBase.PointType.HalfRoofEnd:
                if (IsSmall)
                    return GetRandomHalfRoofTopperEnd();
                if (IsTop)
                    return GetRandomHalfRoofEndTop();
                else
                    return GetRandomHalfRoofEnd();

            case HouseCreatorBase.PointType.Roof:
                if (IsSmall)
                    return GetRandomFullRoofTopper();
                if (IsTop)
                    return GetRandomFullRoofCenterTop();
                else
                    return GetRandomFullRoofCenter();

            case HouseCreatorBase.PointType.HalfRoof:
                if (IsSmall)
                    return GetRandomHalfRoofTopper();
                if (IsTop)
                    return GetRandomHalfRoofCenterTop();
                else 
                    return GetRandomHalfRoofCenter();

            case HouseCreatorBase.PointType.RoofStopper:
                return GetRandomFullRoofStopper();

            case HouseCreatorBase.PointType.HalfRoofStopper:
                return GetRandomHalfRoofStopper();

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

    public Mesh GetRandomFullRoofStopper()
    {
        return FullRoofStopper[Random.Range(0, FullRoofStopper.Length - 1)];
    }

    public Mesh GetRandomHalfRoofStopper()
    {
        return HalfRoofStopper[Random.Range(0, HalfRoofStopper.Length - 1)];
    }

    public Mesh GetRandomFullRoofCenterTop()
    {
        return FullRoofCenterTop[Random.Range(0, FullRoofCenterTop.Length - 1)];
    }

    public Mesh GetRandomHalfRoofCenterTop()
    {
        return HalfRoofCenterTop[Random.Range(0, HalfRoofCenterTop.Length - 1)];
    }

    public Mesh GetRandomFullRoofTopper()
    {
        return FullRoofTopper[Random.Range(0, FullRoofTopper.Length - 1)];
    }
    
    public Mesh GetRandomHalfRoofTopper()
    {
        return HalfRoofTopper[Random.Range(0, HalfRoofTopper.Length - 1)];
    }

    public Mesh GetRandomFullRoofTopperEnd()
    {
        return FullRoofTopperEnd[Random.Range(0, FullRoofTopperEnd.Length - 1)];
    }
    
    public Mesh GetRandomHalfRoofTopperEnd()
    {
        return HalfRoofTopperEnd[Random.Range(0, HalfRoofTopperEnd.Length - 1)];
    }
}

